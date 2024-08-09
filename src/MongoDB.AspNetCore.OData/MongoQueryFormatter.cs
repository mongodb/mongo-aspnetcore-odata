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
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MongoDB.Bson;

namespace MongoDB.AspNetCore.OData;

public class MongoQueryFormatter : ExpressionVisitor
{
    private static readonly MethodInfo __bsonGetValueMethodInfo =
        typeof(BsonDocument).GetMethod("GetValue", new[] { typeof(string) });
    private static Expression __bsonDocs;
    private static ParameterExpression __it = Expression.Parameter(typeof(BsonDocument), "$it");

    private bool _insideContainer;
    private string _fieldName = string.Empty;

    public MongoQueryFormatter(IQueryable queryable)
    {
        __bsonDocs = queryable.Expression;
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        if (node.Method.Name == "Select")
        {
            return VisitSelect(node);
        }

        return base.VisitMethodCall(node);
    }

    private Expression VisitSelect(MethodCallExpression node)
    {
        if (MongoRewriteUtils.RemoveQuotes(node.Arguments[1]) is LambdaExpression lambda &&
            lambda.Body is MemberInitExpression lambdaBody &&
            lambdaBody.NewExpression.Type.Name.StartsWith("SelectSome"))
        {
            var source = __bsonDocs;
            var parameters = new ReadOnlyCollection<ParameterExpression>(new List<ParameterExpression> { __it });

            var newLambda = Expression.Lambda(lambdaBody, parameters);
            newLambda = Visit(newLambda) as LambdaExpression;

            var sourceType = source.Type.GetGenericArguments()[0];
            var newLambdaType = newLambda.ReturnType;

            return Expression.Call(
                MongoRewriteUtils.SelectMethodInfo.MakeGenericMethod(sourceType, newLambdaType), source, newLambda);
        }

        return base.VisitMethodCall(node);
    }

    protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
    {
        switch (node.Member.Name)
        {
            case "Container":
                _insideContainer = true;
                break;
            case "Name":
                _fieldName = ((ConstantExpression)node.Expression).Value as string;
                break;
            case "Value":
                if (_insideContainer)
                {
                    MethodCallExpression value =
                        Expression.Call(__it, __bsonGetValueMethodInfo, Expression.Constant(_fieldName));
                    Type toType = node.Expression.Type;
                    Expression valueConverted = Expression.Convert(value, toType);

                    _fieldName = null;
                    return Expression.Bind(node.Member, valueConverted);
                }

                break;
        }

        return base.VisitMemberAssignment(node);
    }
}
