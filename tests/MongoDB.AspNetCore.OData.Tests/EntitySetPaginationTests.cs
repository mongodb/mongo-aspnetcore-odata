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
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.AspNetCore.OData.Sample.WebApi.Models;

namespace MongoDB.AspNetCore.OData.Tests;

[TestClass]
public class EntitySetPaginationTests
{
    [TestMethod]
    public async Task Pagination()
    {
        var requestUrl = "/odata/cities";
        var firstPage = new HashSet<long>();
        var response_document = await TestServer.GetAndValidateODataRequestAsync<City>(
            requestUrl,
            "cities_structural", d => firstPage.Add(d.Id));

        var nextLink = response_document.RootElement.GetProperty("@odata.nextLink").GetString();
        _ = await TestServer.GetAndValidateODataRequestAsync<City>(
            nextLink,
            "cities_structural", d => !firstPage.Contains(d.Id));
    }
}
