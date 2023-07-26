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

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.AspNetCore.OData.Sample.WebApi.Models;

namespace MongoDB.AspNetCore.OData.Tests;

[TestClass]
public class EntitySetSkipTopTests
{
    [TestMethod]
    [DataRow(0, 10)]
    [DataRow(20, 5)]
    public async Task SkipTopAsync(int skip, int top)
    {
        var requestUrl = $"/odata/cities?$orderby=Id&$skip={skip}&$top={top}";
        var document = await TestServer.GetAndValidateODataRequestAsync<City>(
            requestUrl,
            "cities_structural",
            c => c.Id > skip && c.Id <= skip + top);

        var valueNode = document.RootElement.GetProperty("value");
        Assert.AreEqual(top, valueNode.GetArrayLength());
    }
}
