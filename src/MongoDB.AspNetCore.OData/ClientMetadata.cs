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
// ABOUTME: Extracts the IMongoClient from a Mongo queryable and tags it.

using System.Linq;
using System.Reflection;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver.Linq;

namespace MongoDB.AspNetCore.OData;

internal static class ClientMetadata
{
    private const string LibraryName = "odata";

    private static readonly LibraryInfo __libraryInfo = new(LibraryName, GetLibraryVersion());

    public static void Append(IQueryable queryable)
    {
        try
        {
            // AppendMetadata is a thread-safe no-op for a LibraryInfo the client already carries,
            // so this runs on every query but tags a given client only once.
            TryGetClient(queryable)?.AppendMetadata(__libraryInfo);
        }
        catch
        {
            // Handshake metadata is best-effort telemetry and must never break a query.
        }
    }

    // GetClient throws for a queryable that is not backed by the driver's LINQ provider.
    internal static IMongoClient TryGetClient(IQueryable queryable)
        => queryable?.Provider is IMongoQueryProvider ? queryable.GetClient() : null;

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
