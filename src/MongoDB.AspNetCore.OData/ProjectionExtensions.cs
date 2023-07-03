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
using Microsoft.OData.Edm;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDB.AspNetCore.OData;

internal static class ProjectionExtensions
{
    public static ProjectionDefinition<TSource> Combine<TSource>(this List<ProjectionDefinition<TSource>> definitions) =>
        definitions.Count == 1 ? definitions[0] : Builders<TSource>.Projection.Combine(definitions);

    public static ProjectionDefinition<TSource> Project<TSource>(
        this IEnumerable<IEdmStructuralProperty> properties,
        string path = "")
    {
        if (!properties.Any())
        {
            return new BsonDocumentProjectionDefinition<TSource>(new BsonDocument());
        }

        var result = new List<ProjectionDefinition<TSource>>();
        foreach (var prop in properties)
        {
            var name = string.IsNullOrEmpty(path) ? prop.Name : $"{path}.{prop.Name}";
            result.Add(Builders<TSource>.Projection.Include(name));
        }

        return result.Combine();
    }
}
