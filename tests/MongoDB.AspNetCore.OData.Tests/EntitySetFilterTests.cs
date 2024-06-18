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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.AspNetCore.OData.Sample.WebApi.Models;

namespace MongoDB.AspNetCore.OData.Tests;

[TestClass]
public class EntitySetFilterTests
{
    [TestMethod]
    [DynamicData(nameof(FilterTestCasesIndexes), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetFilterTestDisplayName))]
    public Task FilterAsync(int testCaseIndex)
    {
        var testCase = s_filterTestCases[testCaseIndex];
        return TestServer.GetAndValidateODataRequestAsync(testCase.Url, "cities_structural", testCase.ItemValidator);
    }

    public static string GetFilterTestDisplayName(MethodInfo methodInfo, object[] values)
        => s_filterTestCases[(int)values[0]].Name;

    public static IEnumerable<object[]> FilterTestCasesIndexes()
        => Enumerable.Range(0, s_filterTestCases.Length).Select(i => new object[] { i });

    private static readonly (string Name, string Url, Func<City, bool> ItemValidator)[] s_filterTestCases =
    {
        (
            "eq",
            "/odata/Cities?$filter=Name eq 'New York'",
            item => item.Name == "New York"
        ),
        (
            "ne",
            "/odata/Cities?$filter=CountryId ne 'US'",
            item => item.CountryId != "US"
        ),
        (
            "lt",
            "/odata/Cities?$filter=Id lt 10",
            item => item.Id < 10
        ),
        (
            "le",
            "/odata/Cities?$filter=Id le 10",
            item => item.Id <= 10
        ),
        (
            "gt",
            "/odata/Cities?$filter=Population gt 10000000",
            item => item.Population > 10000000
        ),
        (
            "ge",
            "/odata/Cities?$filter=Population ge 12121244",
            item => item.Population >= 12121244
        ),
        (
            "and",
            "/odata/Cities?$filter=CountryId eq 'CA' and Population gt 500000",
            item => item.Population > 500000 && item.CountryId == "CA"
        ),
        (
            "or",
            "/odata/Cities?$filter=Name eq 'Washington' or Name eq 'Ottawa'",
            item => item.Name == "Washington" || item.Name == "Ottawa"
        ),
        (
            "in",
            "/odata/Cities?$filter=Name in ('Washington', 'Ottawa')",
            item => item.Name == "Washington" || item.Name == "Ottawa"
        ),
        (
            "not",
            "/odata/Cities?$filter=not Name in ('Washington', 'Ottawa')",
            item => item.Name != "Washington" && item.Name != "Ottawa"
        ),
        (
            "startswith",
            "/odata/Cities?$filter=startswith(Name, 'New')",
            item => item.Name.StartsWith("New")
        ),
        (
            "endswith",
            "/odata/Cities?$filter=endswith(Name, 'ton')",
            item => item.Name.EndsWith("ton")
        ),
        (
            "contains",
            "/odata/Cities?$filter=contains(Name, 'lan')",
            item => item.Name.IndexOf("lan", StringComparison.InvariantCulture) > 0
        ),
        (
            "indexof",
            "/odata/Cities?$filter=indexof(Name, 'ton') eq 8",
            item => item.Name.IndexOf("ton", StringComparison.InvariantCulture) == 8
        ),
        (
            "length",
            "/odata/Cities?$filter=length(Name) eq 5",
            item => item.Name.Length == 5
        ),
        (
            "substring_start",
            "/odata/Cities?$filter=substring(Name, 4) eq 'York'",
            item => item.Name.Substring(4) == "York"
        ),
        (
            "substring_start_length",
            "/odata/Cities?$filter=substring(Name, 4, 2) eq 'Yo'",
            item => item.Name.Substring(4, 2) == "Yo"
        ),
        (
            "concat",
            "/odata/Cities?$filter=concat(Name, CountryId) eq 'WashingtonUS'",
            item => string.Concat(item.Name, item.CountryId) == "WashingtonUS"
        ),
        (
            "tolower",
            "/odata/Cities?$filter=tolower(Name) eq 'ottawa'",
            item => item.Name.ToLower() == "ottawa"
        ),
        (
            "toupper",
            "/odata/Cities?$filter=toupper(Name) eq 'OTTAWA'",
            item => item.Name.ToUpper() == "OTTAWA"
        ),
        (
            "matchesPattern",
            "/odata/Cities?$filter=matchesPattern(Name, '%5ENew.*k$')",
            item => Regex.IsMatch(item.Name, "^New.*k$")
        ),
        (
            "trim",
            "/odata/Cities?$filter=trim(Name) eq 'Ottawa'",
            item => item.Name.Trim() == "Ottawa"
        ),
        (
            "ceiling",
            "/odata/Cities?$filter=ceiling(Density) eq 1420",
            item => (int)Math.Ceiling(item.Density) == 1420
        ),
        (
            "floor",
            "/odata/Cities?$filter=floor(Density) eq 1419",
            item => (int)Math.Floor(item.Density) == 1419
        ),
        (
            "round",
            "/odata/Cities?$filter=round(Density) eq 1420",
            item => (int)Math.Round(item.Density, 0) == 1420
        ),
        (
            "add",
            "/odata/Cities?$filter=(Population add 100) eq 989667",
            item => item.Population + 100 == 989667
        ),
        (
            "sub",
            "/odata/Cities?$filter=(Population sub 100) eq 989467",
            item => item.Population - 100 == 989467
        ),
        (
            "negation",
            "/odata/Cities?$filter=(-Population) eq -989567",
            item => -item.Population == -989567
        ),
        (
            "mul",
            "/odata/Cities?$filter=(Population mul 10) eq 9895670",
            item => item.Population * 10 == 9895670
        ),
        (
            "div",
            "/odata/Cities?$filter=(Population div 10) ge 98956",
            item => item.Population / 10 >= 98956
        ),
        (
            "mod",
            "/odata/Cities?$filter=Id mod 10 eq 0",
            item => item.Id % 10 == 0
        ),
        (
            "all",
            "/odata/Cities?$filter=Tags/all(d:startswith(d, 'U'))",
            item => item.Tags.All(t => t.StartsWith("U"))
        ),
        (
            "any",
            "/odata/Cities?$filter=Tags/any(d:startswith(d, 'U'))",
            item => item.Tags.Any(t => t.StartsWith("U"))
        ),
        (
            "date",
            "/odata/Cities?$filter=date(Date) eq 2001-10-23",
            item => item.Date.ToUniversalTime().Date == new DateTime(2001, 10, 23)
        ),
        (
            "day",
            "/odata/Cities?$filter=day(Date) eq 23",
            item => item.Date.ToUniversalTime().Day == 23
        ),
        (
            "year",
            "/odata/Cities?$filter=year(Date) eq 2010",
            item => item.Date.ToUniversalTime().Year == 2010
        ),
        (
            "hour",
            "/odata/Cities?$filter=hour(Date) eq 11",
            item => item.Date.ToUniversalTime().Hour == 11
        ),
        (
            "minute",
            "/odata/Cities?$filter=minute(Date) eq 11",
            item => item.Date.ToUniversalTime().Minute == 11
        ),
        (
            "month",
            "/odata/Cities?$filter=month(Date) eq 11",
            item => item.Date.ToUniversalTime().Month == 11
        ),
        (
            "second",
            "/odata/Cities?$filter=second(Date) eq 11",
            item => item.Date.ToUniversalTime().Second == 11
        ),
        // Expression generated by the oData library for time is not translatable by Mongo LINQ Provider
        // (
        //     "time",
        //     "/odata/Cities?$filter=time(Date) eq 06:10:36",
        //     item => item.Date.ToUniversalTime().TimeOfDay == new TimeSpan(6, 10, 36)
        // ),
    };
}
