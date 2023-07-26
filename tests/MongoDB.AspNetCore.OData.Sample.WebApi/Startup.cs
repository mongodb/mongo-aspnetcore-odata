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

using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;
using MongoDB.AspNetCore.OData.Sample.WebApi.Models;
using MongoDB.Driver;

namespace MongoDB.AspNetCore.OData.Sample.WebApi;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        var connectionString = Configuration.GetSection("MongoDb").GetValue<string>("Connection");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Cannot read mongoDb connection settings");
        }

        services.AddSingleton<IMongoClient>(new MongoClient(connectionString));
        // Seeds the database with test data.
        services.AddSingleton<IHostedService, DatabaseInitializer>();

        var modelBuilder = new ODataConventionModelBuilder();
        modelBuilder.EntitySet<City>("Cities");
        modelBuilder.EntitySet<Region>("Regions");
        modelBuilder.EntitySet<CountryModel>("Countries");

        services.AddControllers().AddOData(
            options =>
            {
                options.Select().Filter().OrderBy().Expand().Count().SetMaxTop(100).AddRouteComponents(
                    "odata",
                    modelBuilder.GetEdmModel());
            });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
