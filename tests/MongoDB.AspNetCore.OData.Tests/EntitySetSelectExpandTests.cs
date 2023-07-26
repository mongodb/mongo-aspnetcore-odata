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
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.AspNetCore.OData.Sample.WebApi.Models;

namespace MongoDB.AspNetCore.OData.Tests;

[TestClass]
public class EntitySetSelectExpandTests
{
    [TestMethod]
    [DataRow("/odata/Countries", "countries_structural", DisplayName = "default")]
    [DataRow("/odata/Countries/?$select=*", "countries_structural", DisplayName = "all")]
    [DataRow("/odata/Countries?$select=Name", "countries_select(Name)", DisplayName = "select(single)")]
    [DataRow("/odata/Countries?$select=Id,Name", "countries_select(Id,Name)", DisplayName = "select(multiple)")]
    [DataRow("/odata/Countries?$select=Capital/Name", "countries_select(Capital-Name)", DisplayName = "select(nested)")]
    [DataRow("/odata/Countries?$expand=Regions", "countries_expand(Regions)", DisplayName = "expand(single)")]
    [DataRow("/odata/Countries?$expand=Regions,Cities", "countries_expand(Regions,Cities)", DisplayName = "expand(multiple)")]
    [DataRow("/odata/Countries?$expand=*", "countries_expand(Regions,Cities)", DisplayName = "expand(all)")]
    [DataRow("/odata/Countries?$expand=Regions($select=Name)", "countries_expand(Regions(select=Name))", DisplayName = "expand(nestedselect)")]
    [DataRow("/odata/Countries?$expand=Regions($count=true)", "countries_expand(Regions-count)", DisplayName = "expand(count)")]
    [DataRow("/odata/Countries?$expand=Regions/$ref", "countries_expand(Regions-ref)", DisplayName = "expand(ref)")]
    [DataRow("/odata/Countries?$expand=Regions,Cities/$ref", "countries_expand(Regions,Cities-ref)", DisplayName = "expand(mixed)")]
    [DataRow("/odata/Countries?$expand=*/$ref", "countries_expand(Regions-ref,Cities-ref)", DisplayName = "expand(all-ref)")]
    [DataRow("/odata/Countries?$select=Id&$expand=Regions", "countries_select(Id)expand(Regions)", DisplayName = "select-expand")]
    [DataRow("/odata/Countries?$select=Id&$expand=Regions($select=Name)",
        "countries_select(Id)expand(Regions(select=Name))", DisplayName = "select-expand(nestedselect)")]
    public Task SelectExpandAsync(string requestUrl, string schemaName)
        => TestServer.GetAndValidateODataRequestAsync(requestUrl, schemaName);

    [TestMethod]
    [DynamicData(nameof(FilteredExpandTestCasesIndexes), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetFilteredExpandTestDisplayName))]
    public Task FilteredExpandAsync(int testCaseIndex)
    {
        var testCase = s_filteredExpandTestCases[testCaseIndex];
        return TestServer.GetAndValidateODataRequestAsync(
            testCase.Url,
            "countries_expand(Cities)",
            (CountryModel country) => country.Cities.All(testCase.ItemValidator));
    }

    public static string GetFilteredExpandTestDisplayName(MethodInfo methodInfo, object[] values)
        => s_filteredExpandTestCases[(int)values[0]].Name;

    public static IEnumerable<object[]> FilteredExpandTestCasesIndexes()
        => Enumerable.Range(0, s_filteredExpandTestCases.Length).Select(i => new object[] { i });

    private static readonly (string Name, string Url, Func<City, bool> ItemValidator)[] s_filteredExpandTestCases =
    {
        (
            "expand_filtered",
            "odata/Countries?$expand=Cities($filter=Name eq 'Richmond')",
            (city) => city.Name == "Richmond"
        )
    };
}
