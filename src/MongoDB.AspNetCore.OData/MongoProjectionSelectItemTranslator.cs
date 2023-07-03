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

using System.Linq;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using MongoDB.Driver;

namespace MongoDB.AspNetCore.OData;

internal sealed class MongoProjectionSelectItemTranslator<TSource> : SelectItemTranslator<ProjectionDefinition<TSource>>
{
    private IEdmEntityType _currentEntityType;
    private string _currentPath = string.Empty;

    public MongoProjectionSelectItemTranslator(ODataQueryContext context)
    {
        _currentEntityType = context.NavigationSource.EntityType();
    }

    public override ProjectionDefinition<TSource> Translate(PathSelectItem item)
    {
        var path = BuildPath(item.SelectedPath);
        return Builders<TSource>.Projection.Include(path);
    }

    public override ProjectionDefinition<TSource> Translate(WildcardSelectItem item)
        => _currentEntityType.StructuralProperties().Project<TSource>(_currentPath);

    public override ProjectionDefinition<TSource> Translate(ExpandedNavigationSelectItem item)
    {
        ProjectionDefinition<TSource> result;
        var originalCurrentPath = _currentPath;
        var originalCurrentEntityType = _currentEntityType;
        _currentPath = BuildPath(item.PathToNavigationProperty);
        _currentEntityType = item.NavigationSource.EntityType();

        if (item.FilterOption != null || item.ComputeOption != null)
        {
            // Calculation and filtration will be done in-memory
            result = Builders<TSource>.Projection.Include(_currentPath);
        }
        else if (item.SelectAndExpand.AllSelected)
        {
            result = _currentEntityType.StructuralProperties().Project<TSource>(_currentPath);
        }
        else
        {
            result = item.SelectAndExpand.SelectedItems
                .Select(s => s.TranslateWith(this))
                .ToList()
                .Combine();
        }

        _currentPath = originalCurrentPath;
        _currentEntityType = originalCurrentEntityType;
        return result;
    }

    public override ProjectionDefinition<TSource> Translate(ExpandedCountSelectItem item)
    {
        var path = BuildPath(item.PathToNavigationProperty);
        return Builders<TSource>.Projection.Include(path);
    }

    public override ProjectionDefinition<TSource> Translate(ExpandedReferenceSelectItem item)
    {
        var path = BuildPath(item.PathToNavigationProperty);
        return item.NavigationSource.EntityType().DeclaredKey.Project<TSource>(path);
    }

    private string BuildPath(ODataPath path)
    {
        if (path.Count == 0)
        {
            return _currentPath;
        }

        if (path.Count == 1 && string.IsNullOrEmpty(_currentPath))
        {
            return path.FirstSegment.Identifier;
        }

        var result = path.Select(s => s.Identifier);
        if (!string.IsNullOrEmpty(_currentPath))
        {
            result = result.Prepend(_currentPath);
        }
        return string.Join('.', result);
    }
}
