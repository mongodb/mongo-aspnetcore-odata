﻿// Copyright 2023-present MongoDB Inc.
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

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using MongoDB.AspNetCore.OData.Sample.WebApi.Models;
using MongoDB.Driver;

namespace MongoDB.AspNetCore.OData.Sample.WebApi.Controllers;

public class CitiesController : ODataController
{
    private readonly IQueryable<City> _cities;

    public CitiesController(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("odata-examples");
        _cities = database.GetCollection<City>("cities").AsQueryable();
    }

    [MongoEnableQuery(PageSize = 20)]
    public ActionResult Get()
    {
        return Ok(_cities);
    }
}
