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

using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using MongoDB.Driver;

namespace MongoDB.AspNetCore.OData;

internal sealed class MongoProjectionSelectItemHandler<TSource> : SelectItemHandler
{
    private IEdmEntityType _currentEntityType;
    private string _currentPath = string.Empty;
    private readonly List<string> _includedPaths = new();

    public MongoProjectionSelectItemHandler(ODataQueryContext context)
    {
        _currentEntityType = context.NavigationSource.EntityType();
    }

    public override void Handle(PathSelectItem item)
    {
        var path = BuildPath(item.SelectedPath);
        IncludePath(path);
    }

    public override void Handle(WildcardSelectItem item) =>
        Include(_currentEntityType.StructuralProperties());

    public override void Handle(ExpandedNavigationSelectItem item)
    {
        var originalCurrentPath = _currentPath;
        var originalCurrentEntityType = _currentEntityType;
        _currentPath = BuildPath(item.PathToNavigationProperty);
        _currentEntityType = item.NavigationSource.EntityType();

        if (item.FilterOption != null || item.ComputeOption != null)
        {
            // Calculation and filtration will be done in-memory
            IncludePath(_currentPath);
        }
        else
        {
            if (item.SelectAndExpand.AllSelected)
            {
                Include(_currentEntityType.StructuralProperties());
            }

            foreach (var selectedItem in item.SelectAndExpand.SelectedItems)
            {
                selectedItem.HandleWith(this);
            }
        }

        _currentPath = originalCurrentPath;
        _currentEntityType = originalCurrentEntityType;
    }

    public override void Handle(ExpandedReferenceSelectItem item)
    {
        var path = BuildPath(item.PathToNavigationProperty);
        IncludePath(path);
    }

    public void Include(IEnumerable<IEdmStructuralProperty> properties)
    {
        foreach (var property in properties)
        {
            var path = property.Name;
            if (!string.IsNullOrEmpty(_currentPath))
            {
                path = $"{_currentPath}.{path}";
            }

            IncludePath(path);
        }
    }

    public ProjectionDefinition<TSource> ToProjectionDefinition()
    {
        if (_includedPaths.Count == 0)
        {
            return null;
        }

        return Builders<TSource>.Projection.Combine(_includedPaths.Select(p =>
            Builders<TSource>.Projection.Include(p)));
    }

    private void IncludePath(string path)
    {
        if (_includedPaths.Any(i => i == path || path.StartsWith($"{i}.")))
        {
            // No needs to add the path if it's already included or parent property(s) is already included:
            // path = "NestedObject.Name", when _includedPaths = [ "NestedObject" ]
            return;
        }

        _includedPaths.Add(path);

        // Need to remove any included properties that is nested to the just added one
        var pathPrefixToRemove = $"{path}.";
        _includedPaths.RemoveAll(i => i.StartsWith(pathPrefixToRemove));
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
