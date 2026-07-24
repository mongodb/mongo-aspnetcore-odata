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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;

namespace MongoDB.AspNetCore.OData.Tests;

[TestClass]
public class ClientMetadataTests
{
    private const string LibraryName = "odata";

    private static IQueryable<Item> CollectionQueryable(IMongoClient client)
        => client.GetDatabase("test").GetCollection<Item>("items").AsQueryable();

    [TestMethod]
    public void TryGetClient_returns_owning_client_for_collection_queryable()
    {
        var client = new MongoClient();

        var result = ClientMetadata.TryGetClient(CollectionQueryable(client));

        Assert.AreSame(client, result);
    }

    [TestMethod]
    public void TryGetClient_returns_null_for_non_mongo_queryable()
    {
        var queryable = new List<Item>().AsQueryable();

        var result = ClientMetadata.TryGetClient(queryable);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void Append_tags_client_with_odata_library_info()
    {
        var client = new MongoClient();

        ClientMetadata.Append(CollectionQueryable(client));

        var libraryInfo = AppendedLibraryInfos(client).SingleOrDefault(info => info.Name == LibraryName);
        Assert.IsNotNull(libraryInfo);
        Assert.IsFalse(string.IsNullOrEmpty(libraryInfo.Version));
    }

    [TestMethod]
    public void Append_tags_client_once_when_called_repeatedly()
    {
        var client = new MongoClient();
        var queryable = CollectionQueryable(client);

        ClientMetadata.Append(queryable);
        ClientMetadata.Append(queryable);

        Assert.AreEqual(1, AppendedLibraryInfos(client).Count(info => info.Name == LibraryName));
    }

    // Reads the library infos the driver will send in its handshake.
    private static LibraryInfo[] AppendedLibraryInfos(IMongoClient client)
    {
        var clientMetadata = GetPrivateField(client.Cluster, "_clientMetadata");
        return (LibraryInfo[])GetPrivateField(clientMetadata, "_libraryInfos");
    }

    private static object GetPrivateField(object instance, string name)
    {
        for (var type = instance.GetType(); type != null; type = type.BaseType)
        {
            var field = type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null)
            {
                return field.GetValue(instance);
            }
        }

        throw new InvalidOperationException($"Field '{name}' not found on '{instance.GetType()}'.");
    }

    private class Item
    {
        public int Id { get; set; }
    }
}
