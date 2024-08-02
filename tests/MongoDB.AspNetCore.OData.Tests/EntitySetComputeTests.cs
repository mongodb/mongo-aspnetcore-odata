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

namespace MongoDB.AspNetCore.OData.Tests;

[TestClass]
public class EntitySetComputeTests
{
    [TestMethod]
    [DataRow("/odata/Cities?$compute=Population div Density as Area&$select=Id,Area",
        "cities_compute(Area)select(Id,Area)", DisplayName = "compute_area_select2")]
    [DataRow("/odata/Cities?$compute=Population div Density as Area&$select=Id,Name,Area",
        "cities_compute(Area)select(Id,Name,Area)", DisplayName = "compute_area_select3")]
    [DataRow("/odata/Cities?$compute=Population div Density as Area,Population mul Density as PopulationDensity&$select=Id,Name,Area,PopulationDensity",
        "cities_compute(Area,PopulationDensity)select(Id,Name,Area,PopulationDensity)", DisplayName = "compute_twice_select4")]
    [DataRow("/odata/Cities?$compute=Population div Density as Area&$compute=Area mul Density as Population2&$select=Id,Area,Population,Population2",
        "cities_compute(Area,Population2)select(Id,Area,Population,Population2)", DisplayName = "compute_nested_select4")]
    [DataRow("/odata/Cities?$compute=Population div Density as Area",
        "cities_compute(Area)", DisplayName = "compute_area_select*")]
    [DataRow("/odata/Cities?$compute=Population div Density as Area&$select=Id",
        "cities_compute(Area)select(Id)", DisplayName = "compute_area_select1")]
    public Task ComputeAsync(string requestUrl, string schemaName)
        => TestServer.GetAndValidateODataRequestAsync(requestUrl, schemaName);

}
