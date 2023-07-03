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
using System.Collections;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.AspNetCore.OData.Sample.WebApi.Models;

namespace MongoDB.AspNetCore.OData.Tests;

[TestClass]
public class EntitySetOrderByTests
{
    [TestMethod]
    [DataRow("Date", false, DisplayName = "datetime, ascending")]
    [DataRow("Date", true, DisplayName = "datetime, descending")]
    [DataRow("Id", false, DisplayName = "long, ascending")]
    [DataRow("Id", true, DisplayName = "long, descending")]
    [DataRow("Name", false, DisplayName = "string, ascending")]
    [DataRow("Name", true, DisplayName = "string, descending")]
    [DataRow("Population", false, DisplayName = "int, ascending")]
    [DataRow("Population", true, DisplayName = "int, descending")]
    [DataRow("Region/Name", false, DisplayName = "nested property, ascending")]
    [DataRow("Region/Name", true, DisplayName = "nested property, descending")]
    public async Task OrderByAsync(string propertyPath, bool isDescending)
    {
        var orderByParameter = propertyPath;
        if (isDescending)
        {
            orderByParameter = $"{orderByParameter} desc";
        }

        Func<object, object, bool> itemsComparerValidator =
            isDescending
                ? (a, b) => Comparer.DefaultInvariant.Compare(a, b) <= 0
                : (a, b) => Comparer.DefaultInvariant.Compare(a, b) >= 0;

        var document = await TestServer.GetAndValidateODataRequestAsync(
            $"/odata/cities?$orderby={orderByParameter}&$expand=Region",
            "cities_expand(Region)");
        var valueNode = document.RootElement.GetProperty("value");
        if (valueNode.GetArrayLength() == 0)
        {
            Assert.Fail("Request result should not be empty");
        }

        object lastValue = null;
        foreach (var itemNode in valueNode.EnumerateArray())
        {
            var model = itemNode.Deserialize<City>();
            object value = model;
            foreach (var property in propertyPath.Split('/'))
            {
                value = value.GetType().GetProperty(property).GetValue(value);
            }

            if (lastValue != null)
            {
                Assert.IsTrue(itemsComparerValidator(value, lastValue),
                    $"Response items should be sorted by: {orderByParameter}");
            }

            lastValue = value;
        }
    }
}
