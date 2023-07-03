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

namespace MongoDB.AspNetCore.OData.Sample.WebApi.Models;

public class CountryModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public Capital Capital { get; set; }
    public Region[] Regions { get; set; }
    public int Population { get; set; }
    public int Area { get; set; }
    public IEnumerable<City> Cities { get; set; }
}
