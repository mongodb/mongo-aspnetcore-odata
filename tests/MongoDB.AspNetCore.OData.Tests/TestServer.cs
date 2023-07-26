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
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Json.Schema;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.AspNetCore.OData.Sample.WebApi;

namespace MongoDB.AspNetCore.OData.Tests;

[TestClass]
internal static class TestServer
{
    private static readonly ConcurrentDictionary<string, JsonSchema> s_schemaCache = new();
    private static WebApplicationFactory<Program> s_applicationFactory;
    private static HttpClient s_httpClient;

    [AssemblyInitialize]
    public static void TestServerInitialize(TestContext testContext)
    {
        s_applicationFactory = new WebApplicationFactory<Program>();
        DatabaseInitializer.Initialize(s_applicationFactory.Services);
        s_httpClient = s_applicationFactory.CreateClient();
    }

    [AssemblyCleanup]
    public static void TestServerInitializeCleanup()
    {
        s_httpClient.Dispose();
        s_applicationFactory.Dispose();
    }

    public static async Task<JsonDocument> GetAndValidateODataRequestAsync(string requestUrl, string schemaName)
    {
        await using var responseStream = await s_httpClient.GetStreamAsync(requestUrl);
        var document = await JsonDocument.ParseAsync(responseStream);
        await ValidateResponseSchemaAsync(document, schemaName);

        return document;
    }

    public static async Task<JsonDocument> GetAndValidateODataRequestAsync<TItem>(string requestUrl, string schemaName, Func<TItem, bool> itemValidator)
    {
        var document = await GetAndValidateODataRequestAsync(requestUrl, schemaName);

        var valueNode = document.RootElement.GetProperty("value");
        if (valueNode.GetArrayLength() == 0)
        {
            Assert.Fail("Request result should not be empty");
        }

        foreach (var itemNode in valueNode.EnumerateArray())
        {
            var item = itemNode.Deserialize<TItem>();
            Assert.IsTrue(itemValidator(item), $"Response item does not match validation criteria: {itemNode}");
        }

        return document;
    }

    private static async ValueTask<JsonSchema> LoadSchemaAsync(string schemaName)
    {
        if (s_schemaCache.TryGetValue(schemaName, out var schema))
        {
            return schema;
        }

        var assembly = Assembly.GetExecutingAssembly();
        using var schemaStream = assembly.GetManifestResourceStream($"MongoDB.AspNetCore.OData.Tests.Schemas.{schemaName}.json");

        schema = await JsonSchema.FromStream(schemaStream);
        s_schemaCache.TryAdd(schemaName, schema);
        return schema;
    }

    private static async ValueTask ValidateResponseSchemaAsync(JsonDocument document, string schemaName)
    {
        var schema = await LoadSchemaAsync(schemaName);
        var evaluationResult = schema.Evaluate(document, new EvaluationOptions {
            OutputFormat = OutputFormat.List
        });

        if (!evaluationResult.IsValid)
        {
            var errorMessages = evaluationResult
                .Details
                .Where(d => d.HasErrors)
                .Select(d => $"{d.EvaluationPath}: {string.Join(';', d.Errors.Values)}.");
            Assert.Fail($"Response JSON does not comply to the provided schema: {string.Join('\n', errorMessages)}.");
        }
    }
}
