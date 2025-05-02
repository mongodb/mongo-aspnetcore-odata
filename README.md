# MongoDB AspNetCore OData

[![MongoDB.AspNetCore.OData](https://img.shields.io/nuget/v/MongoDB.AspNetCore.OData.svg)](https://www.nuget.org/packages/MongoDB.AspNetCore.OData/)

Integration library built on top of [MongoDB.AspNetCore.OData](https://www.nuget.org/packages/MongoDB.AspNetCore.OData/) package to enable OData endpoints for MongoDB.

## Package installation
```sh
dotnet add package MongoDB.AspNetCore.OData
```

## Getting started
Simply follow the [Getting Started with ASP.NET Core OData 8](https://learn.microsoft.com/en-us/odata/webapi-8/getting-started) guide, but use `MongoEnableQuery` attribute instead of `EnableQuery` whenever you are exposing `IMongoQueryable` as OData endpoint.

Please also use [MongoDB.AspNetCore.OData.Sample.WebApi](https://github.com/mongodb/mongo-aspnetcore-odata/tree/main/tests/MongoDB.AspNetCore.OData.Sample.WebApi) as a sample.

## Contribute
Should you encounter any issue with the package, please create [Jira ticket](https://jira.mongodb.org/projects/ODATA) so we can investigate and improve the library.
