// Copyright 2023-present MongoDB Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.OData.Extensions;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Query.Container;
using Microsoft.OData.Edm;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MongoDB.AspNetCore.OData;

public sealed class MongoEnableQueryAttribute : EnableQueryAttribute
{
    private static readonly MethodInfo __applyPagingMethodInfo = typeof(MongoEnableQueryAttribute).GetMethod(
        nameof(ApplyPaging),
        BindingFlags.Static | BindingFlags.NonPublic);
    private static readonly MethodInfo __applyProjectionMethodInfo = typeof(MongoEnableQueryAttribute).GetMethod(
        nameof(ApplyProjection),
        BindingFlags.Static | BindingFlags.NonPublic);
    private static readonly MethodInfo __applySelectExpandMethodInfo = typeof(MongoEnableQueryAttribute).GetMethod(
        nameof(ApplySelectExpand),
        BindingFlags.Static | BindingFlags.NonPublic);
    private static readonly MongoExpressionRewriter __updater = new MongoExpressionRewriter();

    public MongoEnableQueryAttribute()
    {
        AllowedQueryOptions =
            AllowedQueryOptions.Filter |
            AllowedQueryOptions.Expand |
            AllowedQueryOptions.Select |
            AllowedQueryOptions.OrderBy |
            AllowedQueryOptions.Top |
            AllowedQueryOptions.Skip |
            AllowedQueryOptions.Count |
            AllowedQueryOptions.Compute;
    }

    public override IQueryable ApplyQuery(IQueryable queryable, ODataQueryOptions queryOptions)
    {
        if (queryable is not IMongoQueryable)
        {
            return base.ApplyQuery(queryable, queryOptions);
        }

        var ignoreQueryOptions = AllowedQueryOptions.Select | AllowedQueryOptions.Expand;
        var querySettings = ToQuerySettings(queryOptions.Request.GetTimeZoneInfo(), ignoreQueryOptions);
        queryable = queryOptions.ApplyTo(queryable, querySettings);

        queryable = RewriteExpression(queryable);

        if (queryOptions.Request.IsCountRequest())
        {
            // No need to apply projection and stream items for {entity_set}/$count request.
            return queryable;
        }

        queryable = ApplyTransformationMethod(__applyProjectionMethodInfo, queryable, queryOptions);
        if (PageSize > 0)
        {
            queryable = ApplyTransformationMethod(__applyPagingMethodInfo, queryable, PageSize);
        }

        if (queryOptions.SelectExpand != null)
        {
            queryable = ApplyTransformationMethod(__applySelectExpandMethodInfo, queryable, queryOptions);
        }

        return queryable;
    }

    private IQueryable RewriteExpression(IQueryable queryable)
    {
        // Use MongoExpressionRewriter to rewrite non-translatable methods if needed.
        var newExpr = __updater.Visit(queryable.Expression);
        return queryable.Provider.CreateQuery(newExpr);
    }

    public override void OnActionExecuted(ActionExecutedContext actionExecutedContext)
    {
        var originalMongoQueryable = (actionExecutedContext.Result as ObjectResult)?.Value as IMongoQueryable;
        base.OnActionExecuted(actionExecutedContext);

        var response = actionExecutedContext.HttpContext.Response;
        // Check whether the response is set and successful
        if (response?.IsSuccessStatusCode() == true && actionExecutedContext.Result is ObjectResult responseContent)
        {
            if (responseContent.Value is not IMongoQueryable mongoQueryable || originalMongoQueryable != mongoQueryable)
            {
                return;
            }

            // If response was not transformed by the base EnableQuery attribute,
            // we still want force apply projection to limit MQL request to structural properties only
            var queryOptions = (ODataQueryOptions)response.HttpContext.Items[nameof(ODataQueryOptions)];
            ApplyTransformationMethod(__applyProjectionMethodInfo, mongoQueryable, queryOptions);
        }
    }

    public override void ValidateQuery(HttpRequest request, ODataQueryOptions queryOptions)
    {
        base.ValidateQuery(request, queryOptions);

        // Store this into the http context in case we will need to enforce projection
        request.HttpContext.Items[nameof(ODataQueryOptions)] = queryOptions;
    }

    private static IQueryable ApplyProjection<T>(IMongoQueryable<T> queryable, ODataQueryOptions queryOptions)
    {
        var fieldProjects = new List<ProjectionDefinition<T>>();
        var entityType = queryOptions.Context.NavigationSource.EntityType();

        if (queryOptions.Compute != null || queryOptions.SelectExpand == null || queryOptions.SelectExpand.SelectExpandClause.AllSelected)
        {
            fieldProjects.Add(entityType.StructuralProperties().Project<T>());
        }

        if (queryOptions.SelectExpand != null)
        {
            foreach (var selectedItem in queryOptions.SelectExpand.SelectExpandClause.SelectedItems)
            {
                var translator = new MongoProjectionSelectItemTranslator<T>(queryOptions.Context);
                fieldProjects.Add(selectedItem.TranslateWith(translator));
            }
        }

        var projection = fieldProjects.Combine();
        var projectionStage = PipelineStageDefinitionBuilder.Project<T, T>(projection);
        return queryable.AppendStage(projectionStage);
    }

    private static IQueryable ApplyPaging<T>(IQueryable<T> queryable, int pageSize)
        => new TruncatedCollection<T>(queryable, pageSize).AsQueryable();

    private static IQueryable ApplySelectExpand<T>(IQueryable<T> queryable, ODataQueryOptions queryOptions)
    {
        // SelectExpand projection generated by oData library is not compatible with Mongo LINQ provider,
        // so we split the query to server and client side queries, where SelectExpand with be executed on the client side.
        var wrappedQueryable = queryable.ToArray().AsQueryable();

        var allowedOptions = AllowedQueryOptions.Select | AllowedQueryOptions.Expand | AllowedQueryOptions.Compute;
        var ignoreOption = AllowedQueryOptions.All & ~allowedOptions;
        return queryOptions.ApplyTo(wrappedQueryable, ignoreOption);
    }

    private static IQueryable ApplyTransformationMethod(MethodInfo method, params object[] parameters)
    {
        var queryable = parameters[0];
        if (!(queryable is IQueryable mongoQueryable))
        {
            throw new ArgumentException("IQueryable is expected.", nameof(queryable));
        }

        var genericMethod = method.MakeGenericMethod(mongoQueryable.ElementType);
        return (IQueryable)genericMethod.Invoke(null, parameters);
    }

    private ODataQuerySettings ToQuerySettings(TimeZoneInfo timeZoneInfo, AllowedQueryOptions ignoredQueryOptions)
        => new ODataQuerySettings
        {
            TimeZone = timeZoneInfo,
            EnsureStableOrdering = EnsureStableOrdering,
            EnableConstantParameterization = EnableConstantParameterization,
            HandleNullPropagation = HandleNullPropagation,
            HandleReferenceNavigationPropertyExpandFilter = HandleReferenceNavigationPropertyExpandFilter,
            EnableCorrelatedSubqueryBuffering = EnableCorrelatedSubqueryBuffering,
            IgnoredQueryOptions = ignoredQueryOptions
        };
}
