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

// ABOUTME: Appends this library's information to the MongoDB driver handshake metadata.
// ABOUTME: Extracts the IMongoClient from a Mongo queryable and tags it once per client.

using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;

namespace MongoDB.AspNetCore.OData;

internal static class ClientMetadata
{
    private const string LibraryName = "odata";

    private static readonly LibraryInfo __libraryInfo = new(LibraryName, GetLibraryVersion());
    private static readonly object __tagged = new();
    private static readonly ConditionalWeakTable<IMongoClient, object> __taggedClients = new();

    public static void Append(IQueryable queryable)
    {
        try
        {
            var client = TryGetClient(queryable);
            if (client == null)
            {
                return;
            }

            // ConditionalWeakTable runs the factory once per client, so a given client is tagged
            // only once even under concurrent requests.
            __taggedClients.GetValue(client, static c =>
            {
                c.AppendMetadata(__libraryInfo);
                return __tagged;
            });
        }
        catch
        {
            // Handshake metadata is best-effort telemetry and must never break a query. A driver
            // change that breaks client extraction is caught by ClientMetadataTests in CI.
        }
    }

    internal static IMongoClient TryGetClient(IQueryable queryable)
    {
        var provider = queryable?.Provider;
        if (provider == null)
        {
            return null;
        }

        // Only the driver's LINQ provider exposes a route to the client, and only via its internal
        // MongoQueryProvider<TDocument>.Collection property (a public getter on an internal type).
        var collectionProperty = provider.GetType().GetProperty("Collection");
        var collection = collectionProperty?.GetValue(provider);
        if (collection == null)
        {
            return null;
        }

        var databaseProperty = typeof(IMongoCollection<>)
            .MakeGenericType(queryable.ElementType)
            .GetProperty(nameof(IMongoCollection<object>.Database));

        var database = databaseProperty?.GetValue(collection) as IMongoDatabase;
        return database?.Client;
    }

    private static string GetLibraryVersion()
    {
        var assembly = typeof(ClientMetadata).Assembly;
        var informationalVersion = assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;

        if (!string.IsNullOrEmpty(informationalVersion))
        {
            // SourceLink appends build metadata as "+<commit sha>", which is not part of the version.
            var buildMetadataIndex = informationalVersion.IndexOf('+');
            return buildMetadataIndex < 0
                ? informationalVersion
                : informationalVersion.Substring(0, buildMetadataIndex);
        }

        return assembly.GetName().Version?.ToString() ?? "0.0.0";
    }
}
