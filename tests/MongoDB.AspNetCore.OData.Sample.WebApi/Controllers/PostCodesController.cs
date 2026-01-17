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

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace MongoDB.AspNetCore.OData.Sample.WebApi.Controllers;

// Controller code snippet
public class PostCodesController : ODataController
{
    private readonly IQueryable<PostCodeViewModel> _postCodes;

    public PostCodesController(IMongoClient client)
    {
        var database = client.GetDatabase("odata-examples");
        _postCodes = database.GetCollection<PostCodeViewModel>("postCodes").AsQueryable();
    }

    [MongoEnableQuery(PageSize = 10)]
    public ActionResult<IEnumerable<PostCodeViewModel>> Get()
    {
        return Ok(_postCodes);
    }
}

// Model code snippet:
public class PostCodeViewModel
{
    [BsonId]
    [IgnoreDataMember]
    public int Id { get; set; }

    public string Place { get; set; }

    [Key]
    [BsonElement("Code")]
    public string CodeId { get; set; }
}

