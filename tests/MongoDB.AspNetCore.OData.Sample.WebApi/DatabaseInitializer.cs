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

using System.Globalization;
using System.Linq.Expressions;
using MongoDB.AspNetCore.OData.Sample.WebApi.Models;
using MongoDB.Driver;

namespace MongoDB.AspNetCore.OData.Sample.WebApi;

internal static class DatabaseInitializer
{
    public static void Initialize(IServiceProvider services)
    {
        var mongoClient = services.GetRequiredService<IMongoClient>();
        var database = mongoClient.GetDatabase("odata-examples");

        BulkUpsert(database.GetCollection<Country>("countries"), Countries(), d => d.Id);
        BulkUpsert(database.GetCollection<City>("cities"), Cities(), d => d.Id);
        BulkUpsert(database.GetCollection<PostCode>("postCodes"), PostCodes(), d => d.Id);
    }

    private static void BulkUpsert<TDocument, TField>(
        IMongoCollection<TDocument> collection,
        IEnumerable<TDocument> entities,
        Expression<Func<TDocument, TField>> filterField)
    {
        var filterFieldValue = filterField.Compile();

        var bulkOperations = entities
            .Select(e =>
            {
                var filter = Builders<TDocument>.Filter.Eq(filterField, filterFieldValue(e));
                return new ReplaceOneModel<TDocument>(filter, e) { IsUpsert = true };
            });

        collection.BulkWrite(bulkOperations);
    }

    private static IEnumerable<PostCode> PostCodes() => new []
    {
        new PostCode { Id = 1, Code = "M4R", Place = "Central Toronto"},
        new PostCode { Id = 2, Code = "M5K", Place = "Downtown Toronto"},
        new PostCode { Id = 3, Code = "M5S", Place = "Downtown Toronto"},
        new PostCode { Id = 4, Code = "M4M", Place = "Toronto" },
        new PostCode { Id = 5, Code = "H1B", Place = "Montreal East" },
        new PostCode { Id = 6, Code = "H2V", Place = "Montreal" },
        new PostCode { Id = 7, Code = "V6Z", Place = "Vancouver" },
        new PostCode { Id = 8, Code = "T2P", Place = "Calgary City Centre" },
        new PostCode { Id = 9, Code = "T3M", Place = "Calgary Cranston" },
        new PostCode { Id = 10, Code = "T3N", Place = "Calgary Northeast" },
        new PostCode { Id = 11, Code = "T3R", Place = "Calgary Northwest" },
        new PostCode { Id = 12, Code = "T3K", Place = "Calgary" },
        new PostCode { Id = 13, Code = "T6M", Place = "Edmonton Southwest" },
        new PostCode { Id = 14, Code = "T6R", Place = "Edmonton (Riverbend)" },
        new PostCode { Id = 15, Code = "T6T", Place = "Edmonton (Meadows)" },
        new PostCode { Id = 16, Code = "T6X", Place = "Edmonton (Ellerslie)" },
        new PostCode { Id = 17, Code = "T6J", Place = "Edmonton (Kaskitayo)" },
        new PostCode { Id = 18, Code = "T5A", Place = "Edmonton" },

        new PostCode { Id = 19, Code = "K1Y", Place = "Ottawa West" },
        new PostCode { Id = 20, Code = "K1S", Place = "Ottawa (The Glebe / Ottawa South / Ottawa East)" },
        new PostCode { Id = 21, Code = "K1K", Place = "Ottawa (Overbrook)" },
        new PostCode { Id = 22, Code = "K1L", Place = "Ottawa (Vanier)" },
        new PostCode { Id = 23, Code = "K1Z", Place = "Ottawa (Westboro)" },
        new PostCode { Id = 24, Code = "K0A", Place = "Ottawa" },

    };

    private static IEnumerable<Country> Countries()
        => new[]
        {
            new Country
            {
                Id = "US",
                Name = "United States of America",
                Population = 331893745,
                Area = 9833520,
                Capital = new Capital
                {
                    CityId = 9,
                    Name = "Washington",
                    Region = new Region
                    {
                        Id = "DC",
                        Name = "District of Columbia"
                    },
                    Population = 4810669
                },
                Regions = new Region[]
                {
                    new() { Id = "AL", Name = "Alabama" },
                    new() { Id = "AK", Name = "Alaska" },
                    new() { Id = "AZ", Name = "Arizona" },
                    new() { Id = "AR", Name = "Arkansas" },
                    new() { Id = "CA", Name = "California" },
                    new() { Id = "CO", Name = "Colorado" },
                    new() { Id = "CT", Name = "Connecticut" },
                    new() { Id = "DE", Name = "Delaware" },
                    new() { Id = "FL", Name = "Florida" },
                    new() { Id = "GA", Name = "Georgia" },
                    new() { Id = "HI", Name = "Hawaii" },
                    new() { Id = "ID", Name = "Idaho" },
                    new() { Id = "IL", Name = "Illinois" },
                    new() { Id = "IN", Name = "Indiana" },
                    new() { Id = "IA", Name = "Iowa" },
                    new() { Id = "KS", Name = "Kansas" },
                    new() { Id = "KY", Name = "Kentucky" },
                    new() { Id = "LA", Name = "Louisiana" },
                    new() { Id = "ME", Name = "Maine" },
                    new() { Id = "MD", Name = "Maryland" },
                    new() { Id = "MA", Name = "Massachusetts" },
                    new() { Id = "MI", Name = "Michigan" },
                    new() { Id = "MN", Name = "Minnesota" },
                    new() { Id = "MS", Name = "Mississippi" },
                    new() { Id = "MO", Name = "Missouri" },
                    new() { Id = "MT", Name = "Montana" },
                    new() { Id = "NE", Name = "Nebraska" },
                    new() { Id = "NV", Name = "Nevada" },
                    new() { Id = "NH", Name = "New Hampshire" },
                    new() { Id = "NJ", Name = "New Jersey" },
                    new() { Id = "NM", Name = "New Mexico" },
                    new() { Id = "NY", Name = "New York" },
                    new() { Id = "NC", Name = "North Carolina" },
                    new() { Id = "ND", Name = "North Dakota" },
                    new() { Id = "OH", Name = "Ohio" },
                    new() { Id = "OK", Name = "Oklahoma" },
                    new() { Id = "OR", Name = "Oregon" },
                    new() { Id = "PA", Name = "Pennsylvania" },
                    new() { Id = "RI", Name = "Rhode Island" },
                    new() { Id = "SC", Name = "South Carolina" },
                    new() { Id = "SD", Name = "South Dakota" },
                    new() { Id = "TN", Name = "Tennessee" },
                    new() { Id = "TX", Name = "Texas" },
                    new() { Id = "UT", Name = "Utah" },
                    new() { Id = "VT", Name = "Vermont" },
                    new() { Id = "VA", Name = "Virginia" },
                    new() { Id = "WA", Name = "Washington" },
                    new() { Id = "WV", Name = "West Virginia" },
                    new() { Id = "WI", Name = "Wisconsin" },
                    new() { Id = "WY", Name = "Wyoming" },
                    new() { Id = "DC", Name = "District of Columbia" }
                }
            },
            new Country
            {
                Id = "CA",
                Name = "Canada",
                Population = 38246108,
                Area = 9984670,
                Capital = new Capital
                {
                    CityId = 1006,
                    Name = "Ottawa",
                    Region = new Region
                    {
                        Id = "ON",
                        Name = "Ontario"
                    },
                    Population = 989567
                },
                Regions = new Region[]
                {
                    new() { Id = "ON", Name = "Ontario" },
                    new() { Id = "QC", Name = "Quebec" },
                    new() { Id = "NS", Name = "Nova Scotia" },
                    new() { Id = "NB", Name = "New Brunswick" },
                    new() { Id = "MB", Name = "Manitoba" },
                    new() { Id = "BC", Name = "British Columbia" },
                    new() { Id = "PE", Name = "Prince Edward Island" },
                    new() { Id = "SK", Name = "Saskatchewan" },
                    new() { Id = "AB", Name = "Alberta" },
                    new() { Id = "NL", Name = "Newfoundland and Labrador" },
                    new() { Id = "NT", Name = "Northwest Territories" },
                    new() { Id = "YT", Name = "Yukon" },
                    new() { Id = "NU", Name = "Nunavut" },
                }
            }
        };

    private static IEnumerable<City> Cities()
        => new[]
        {
            new City
            {
                Id = 1,
                CountryId = "US",
                Name = "New York",
                Region = new Region { Id = "NY", Name = "New York" },
                Population = 18972871,
                Density = 10768.2,
                Tags = new[] { "NY", "US" },
                AverageTemperatureMin = 0.6,
                AverageTemperatureMax = 24.9,
                Date = DateTime.Parse("2008-05-03 11:38:04 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 2,
                CountryId = "US",
                Name = "Los Angeles",
                Region = new Region { Id = "CA", Name = "California" },
                Population = 12121244,
                Density =  3267.6,
                Tags = new[] { "CA", "US" },
                AverageTemperatureMin = 14.2,
                AverageTemperatureMax = 23.5,
                Date = DateTime.Parse("1999-09-29 11:52:32 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 3,
                CountryId = "US",
                Name = "Chicago",
                Region = new Region { Id = "IL", Name = "Illinois" },
                Population = 8595181,
                Density = 4576.6,
                Tags = new[] { "IL", "US" },
                AverageTemperatureMin = -4.6,
                AverageTemperatureMax = 23.3,
                Date = DateTime.Parse("2001-11-30 8:58:53 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 4,
                CountryId = "US",
                Name = "Miami",
                Region = new Region { Id = "FL", Name = "Florida" },
                Population = 5711945,
                Density = 4945.7,
                Tags = new[] { "FL", "US" },
                AverageTemperatureMin = 19.9,
                AverageTemperatureMax = 28.8,
                Date = DateTime.Parse("2014-11-23 8:25:23 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 5,
                CountryId = "US",
                Name = "Dallas",
                Region = new Region { Id = "TX", Name = "Texas" },
                Population = 5668165,
                Density = 1522.2,
                Tags = new[] { "TX", "US" },
                AverageTemperatureMin = 7.7,
                AverageTemperatureMax = 29.8,
                Date = DateTime.Parse("2001-10-23 2:16:00 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 6,
                CountryId = "US",
                Name = "Houston",
                Region = new Region { Id = "TX", Name = "Texas" },
                Population = 5650910,
                Density = 1394.6,
                Tags = new[] { "TX", "US" },
                AverageTemperatureMin = 11.5,
                AverageTemperatureMax = 29.0,
                Date = DateTime.Parse("2021-06-08 9:50:12 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 7,
                CountryId = "US",
                Name = "Philadelphia",
                Region = new Region { Id = "PA", Name = "Pennsylvania" },
                Population = 5512873,
                Density = 4544.9,
                Tags = new[] { "PA", "US" },
                AverageTemperatureMin = 0.5,
                AverageTemperatureMax = 25.6,
                Date = DateTime.Parse("1999-11-23 5:22:27 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 8,
                CountryId = "US",
                Name = "Atlanta",
                Region = new Region { Id = "GA", Name = "Georgia" },
                Population = 5046555,
                Density = 1419.9,
                Tags = new[] { "GA", "US" },
                AverageTemperatureMin = 6.4,
                AverageTemperatureMax = 26.9,
                Date = DateTime.Parse("2002-05-31 5:32:05 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 9,
                CountryId = "US",
                Name = "Washington",
                Region = new Region { Id = "DC", Name = "District of Columbia" },
                Population = 4810669,
                Density = 4434.0,
                Tags = new[] { "DC", "US" },
                AverageTemperatureMin = 2.3,
                AverageTemperatureMax = 26.6,
                Date = DateTime.Parse("2004-01-04 10:11:00 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 10,
                CountryId = "US",
                Name = "Boston",
                Region = new Region { Id = "MA", Name = "Massachusetts" },
                Population = 4208580,
                Density = 5505.8,
                Tags = new[] { "MA", "US" },
                AverageTemperatureMin = -1.5,
                AverageTemperatureMax = 23.2,
                Date = DateTime.Parse("2019-06-17 7:09:39 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 11,
                CountryId = "US",
                Name = "Phoenix",
                Region = new Region { Id = "AZ", Name = "Arizona" },
                Population = 4047095,
                Density = 1235.5,
                Tags = new[] { "AZ", "US" },
                AverageTemperatureMin = 13.1,
                AverageTemperatureMax = 34.9,
                Date = DateTime.Parse("1985-11-04 10:05:42 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 12,
                CountryId = "US",
                Name = "Detroit",
                Region = new Region { Id = "MI", Name = "Michigan" },
                Population = 3522856,
                Density = 1871.2,
                Tags = new[] { "MI", "US" },
                AverageTemperatureMin = -3.7,
                AverageTemperatureMax = 23.0,
                Date = DateTime.Parse("1972-05-12 9:16:56 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 13,
                CountryId = "US",
                Name = "Seattle",
                Region = new Region { Id = "WA", Name = "Washington" },
                Population = 3438221,
                Density = 3412.2,
                Tags = new[] { "WA", "US" },
                AverageTemperatureMin = 4.7,
                AverageTemperatureMax = 18.8,
                Date = DateTime.Parse("2008-09-24 6:32:55 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 14,
                CountryId = "US",
                Name = "San Francisco",
                Region = new Region { Id = "CA", Name = "California" },
                Population = 3290197,
                Density = 7199.4,
                Tags = new[] { "CA", "US" },
                AverageTemperatureMin = 11.2,
                AverageTemperatureMax = 17.5,
                Date = DateTime.Parse("1983-09-27 5:12:00 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 15,
                CountryId = "US",
                Name = "San Diego",
                Region = new Region { Id = "CA", Name = "California" },
                Population = 3084174,
                Density = 1675.9,
                Tags = new[] { "CA", "US" },
                AverageTemperatureMin = 13.9,
                AverageTemperatureMax = 22.2,
                Date = DateTime.Parse("1986-10-28 9:24:02 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 16,
                CountryId = "US",
                Name = "Minneapolis",
                Region = new Region { Id = "MN", Name = "Minnesota" },
                Population = 2856952,
                Density = 3035.5,
                Tags = new[] { "MN", "US" },
                AverageTemperatureMin = -9.1,
                AverageTemperatureMax = 23.2,
                Date = DateTime.Parse("2001-06-08 8:11:38 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 17,
                CountryId = "US",
                Name = "Brooklyn",
                Region = new Region { Id = "NY", Name = "New York" },
                Population = 2736074,
                Density = 15200.5,
                Tags = new[] { "NY", "US" },
                AverageTemperatureMin = 0.6,
                AverageTemperatureMax = 24.9,
                Date = DateTime.Parse("1998-07-13 4:40:16 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 18,
                CountryId = "US",
                Name = "Tampa",
                Region = new Region { Id = "FL", Name = "Florida" },
                Population = 2683956,
                Density = 1340.7,
                Tags = new[] { "FL", "US" },
                AverageTemperatureMin = 15.9,
                AverageTemperatureMax = 28.3,
                Date = DateTime.Parse("2019-01-22 6:55:45 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 19,
                CountryId = "US",
                Name = "Denver",
                Region = new Region { Id = "CO", Name = "Colorado" },
                Population = 2650725,
                Density = 1805.7,
                Tags = new[] { "CO", "US" },
                AverageTemperatureMin = -0.9,
                AverageTemperatureMax = 23.4,
                Date = DateTime.Parse("1977-07-16 9:49:32 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 20,
                CountryId = "US",
                Name = "Queens",
                Region = new Region { Id = "NY", Name = "New York" },
                Population = 2405464,
                Density = 8503.7,
                Tags = new[] { "NY", "US" },
                AverageTemperatureMin = 0.6,
                AverageTemperatureMax = 24.9,
                Date = DateTime.Parse("2001-03-28 2:33:00 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 21,
                CountryId = "US",
                Name = "Baltimore",
                Region = new Region { Id = "MD", Name = "Maryland" },
                Population = 2205092,
                Density = 2872.8,
                Tags = new[] { "MD", "US" },
                AverageTemperatureMin = 0.8,
                AverageTemperatureMax = 25.3,
                Date = DateTime.Parse("2011-07-11 4:24:49 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 22,
                CountryId = "US",
                Name = "Las Vegas",
                Region = new Region { Id = "NV", Name = "Nevada" },
                Population = 2150373,
                Density = 1754.7,
                Tags = new[] { "NV", "US" },
                AverageTemperatureMin = 8.2,
                AverageTemperatureMax = 33.2,
                Date = DateTime.Parse("2008-01-08 1:40:37 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 23,
                CountryId = "US",
                Name = "St. Louis",
                Region = new Region { Id = "MO", Name = "Missouri" },
                Population = 2092481,
                Density = 1905.7,
                Tags = new[] { "MO", "US" },
                AverageTemperatureMin = -0.1,
                AverageTemperatureMax = 26.7,
                Date = DateTime.Parse("2011-06-17 6:29:58 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 24,
                CountryId = "US",
                Name = "Portland",
                Region = new Region { Id = "OR", Name = "Oregon" },
                Population = 2036875,
                Density = 1881.6,
                Tags = new[] { "OR", "US" },
                AverageTemperatureMin = 4.7,
                AverageTemperatureMax = 20.9,
                Date = DateTime.Parse("1979-08-30 9:12:22 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 25,
                CountryId = "US",
                Name = "Riverside",
                Region = new Region { Id = "CA", Name = "California" },
                Population = 2022285,
                Density = 1557.0,
                Tags = new[] { "CA", "US" },
                AverageTemperatureMin = 12.7,
                AverageTemperatureMax = 26.6,
                Date = DateTime.Parse("1987-03-25 9:16:56 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 26,
                CountryId = "US",
                Name = "Orlando",
                Region = new Region { Id = "FL", Name = "Florida" },
                Population = 1927699,
                Density = 992.0,
                Tags = new[] { "FL", "US" },
                AverageTemperatureMin = 15.9,
                AverageTemperatureMax = 28.1,
                Date = DateTime.Parse("2017-05-27 8:52:55 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 27,
                CountryId = "US",
                Name = "Sacramento",
                Region = new Region { Id = "CA", Name = "California" },
                Population = 1924167,
                Density = 1971.3,
                Tags = new[] { "CA", "US" },
                AverageTemperatureMin = 8.0,
                AverageTemperatureMax = 24.2,
                Date = DateTime.Parse("2001-11-16 12:30:27 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 28,
                CountryId = "US",
                Name = "San Juan",
                Region = new Region { Id = "PR", Name = "Puerto Rico" },
                Population = 1915105,
                Density = 2982.2,
                Tags = new[] { "PR", "US" },
                AverageTemperatureMin = 24.8,
                AverageTemperatureMax = 28.7,
                Date = DateTime.Parse("1981-04-23 6:20:38 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 29,
                CountryId = "US",
                Name = "San Antonio",
                Region = new Region { Id = "TX", Name = "Texas" },
                Population = 1910785,
                Density = 1184.5,
                Tags = new[] { "TX", "US" },
                AverageTemperatureMin = 11.0,
                AverageTemperatureMax = 29.7,
                Date = DateTime.Parse("2017-07-25 11:38:36 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 30,
                CountryId = "US",
                Name = "San Jose",
                Region = new Region { Id = "CA", Name = "California" },
                Population = 1729879,
                Density = 2229.0,
                Tags = new[] { "CA", "US" },
                AverageTemperatureMin = 10.6,
                AverageTemperatureMax = 21.8,
                Date = DateTime.Parse("2020-04-05 1:51:11 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 31,
                CountryId = "US",
                Name = "Pittsburgh",
                Region = new Region { Id = "PA", Name = "Pennsylvania" },
                Population = 1720279,
                Density = 2100.7,
                Tags = new[] { "PA", "US" },
                AverageTemperatureMin = -1.9,
                AverageTemperatureMax = 22.7,
                Date = DateTime.Parse("2012-05-26 10:32:43 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 32,
                CountryId = "US",
                Name = "Cincinnati",
                Region = new Region { Id = "OH", Name = "Ohio" },
                Population = 1712287,
                Density = 1500.2,
                Tags = new[] { "OH", "US" },
                AverageTemperatureMin = -0.3,
                AverageTemperatureMax = 24.4,
                Date = DateTime.Parse("1977-12-01 2:34:56 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 33,
                CountryId = "US",
                Name = "Manhattan",
                Region = new Region { Id = "NY", Name = "New York" },
                Population = 1694263,
                Density = 28653.9,
                Tags = new[] { "NY", "US" },
                AverageTemperatureMin = 0.6,
                AverageTemperatureMax = 24.9,
                Date = DateTime.Parse("1996-12-20 4:56:47 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 34,
                CountryId = "US",
                Name = "Cleveland",
                Region = new Region { Id = "OH", Name = "Ohio" },
                Population = 1683059,
                Density = 1904.0,
                Tags = new[] { "OH", "US" },
                AverageTemperatureMin = -1.6,
                AverageTemperatureMax = 23.6,
                Date = DateTime.Parse("2007-01-01 4:38:25 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 35,
                CountryId = "US",
                Name = "Indianapolis",
                Region = new Region { Id = "IN", Name = "Indiana" },
                Population = 1659305,
                Density = 928.2,
                Tags = new[] { "IN", "US" },
                AverageTemperatureMin = -2.2,
                AverageTemperatureMax = 24.2,
                Date = DateTime.Parse("1998-04-25 12:14:20 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 36,
                CountryId = "US",
                Name = "Austin",
                Region = new Region { Id = "TX", Name = "Texas" },
                Population = 1659251,
                Density = 1165.6,
                Tags = new[] { "TX", "US" },
                AverageTemperatureMin = 10.8,
                AverageTemperatureMax = 29.9,
                Date = DateTime.Parse("2005-11-02 2:44:55 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 37,
                CountryId = "US",
                Name = "Kansas City",
                Region = new Region { Id = "MO", Name = "Missouri" },
                Population = 1644497,
                Density = 602.6,
                Tags = new[] { "MO", "US" },
                AverageTemperatureMin = -1.8,
                AverageTemperatureMax = 25.7,
                Date = DateTime.Parse("1986-03-11 8:35:55 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 38,
                CountryId = "US",
                Name = "Columbus",
                Region = new Region { Id = "OH", Name = "Ohio" },
                Population = 1556848,
                Density = 1559.5,
                Tags = new[] { "OH", "US" },
                AverageTemperatureMin = -1.6,
                AverageTemperatureMax = 23.8,
                Date = DateTime.Parse("2000-12-09 3:03:10 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 39,
                CountryId = "US",
                Name = "Charlotte",
                Region = new Region { Id = "NC", Name = "North Carolina" },
                Population = 1516107,
                Density = 1091.0,
                Tags = new[] { "NC", "US" },
                AverageTemperatureMin = 5.1,
                AverageTemperatureMax = 26.4,
                Date = DateTime.Parse("2000-10-10 1:01:21 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 40,
                CountryId = "US",
                Name = "Virginia Beach",
                Region = new Region { Id = "VA", Name = "Virginia" },
                Population = 1500764,
                Density = 711.4,
                Tags = new[] { "VA", "US" },
                AverageTemperatureMin = 4.9,
                AverageTemperatureMax = 26.7,
                Date = DateTime.Parse("2005-02-05 10:37:17 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 41,
                CountryId = "US",
                Name = "Bronx",
                Region = new Region { Id = "NY", Name = "New York" },
                Population = 1472654,
                Density = 13356.3,
                Tags = new[] { "NY", "US" },
                AverageTemperatureMin = 0.6,
                AverageTemperatureMax = 24.9,
                Date = DateTime.Parse("1981-08-30 12:12:14 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 42,
                CountryId = "US",
                Name = "Milwaukee",
                Region = new Region { Id = "WI", Name = "Wisconsin" },
                Population = 1340981,
                Density = 2379.0,
                Tags = new[] { "WI", "US" },
                AverageTemperatureMin = -5.2,
                AverageTemperatureMax = 22.3,
                Date = DateTime.Parse("1980-12-27 12:05:04 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 43,
                CountryId = "US",
                Name = "Providence",
                Region = new Region { Id = "RI", Name = "Rhode Island" },
                Population = 1270149,
                Density = 3764.8,
                Tags = new[] { "RI", "US" },
                AverageTemperatureMin = -1.0,
                AverageTemperatureMax = 23.6,
                Date = DateTime.Parse("1980-03-10 4:01:50 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 44,
                CountryId = "US",
                Name = "Jacksonville",
                Region = new Region { Id = "FL", Name = "Florida" },
                Population = 1220191,
                Density = 466.3,
                Tags = new[] { "FL", "US" },
                AverageTemperatureMin = 11.7,
                AverageTemperatureMax = 27.9,
                Date = DateTime.Parse("2019-05-25 10:43:57 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 45,
                CountryId = "US",
                Name = "Salt Lake City",
                Region = new Region { Id = "UT", Name = "Utah" },
                Population = 1135344,
                Density = 700.3,
                Tags = new[] { "UT", "US" },
                AverageTemperatureMin = -1.3,
                AverageTemperatureMax = 26.1,
                Date = DateTime.Parse("2003-03-09 10:08:09 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 46,
                CountryId = "US",
                Name = "Nashville",
                Region = new Region { Id = "TN", Name = "Tennessee" },
                Population = 1098486,
                Density = 541.3,
                Tags = new[] { "TN", "US" },
                AverageTemperatureMin = 3.4,
                AverageTemperatureMax = 26.6,
                Date = DateTime.Parse("2018-02-10 6:15:33 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 47,
                CountryId = "US",
                Name = "Raleigh",
                Region = new Region { Id = "NC", Name = "North Carolina" },
                Population = 1062018,
                Density = 1232.8,
                Tags = new[] { "NC", "US" },
                AverageTemperatureMin = 5.5,
                AverageTemperatureMax = 26.9,
                Date = DateTime.Parse("1973-10-28 7:59:55 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 48,
                CountryId = "US",
                Name = "Memphis",
                Region = new Region { Id = "TN", Name = "Tennessee" },
                Population = 1034498,
                Density = 852.1,
                Tags = new[] { "TN", "US" },
                AverageTemperatureMin = 5.2,
                AverageTemperatureMax = 28.2,
                Date = DateTime.Parse("1977-12-25 6:12:59 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 49,
                CountryId = "US",
                Name = "Louisville",
                Region = new Region { Id = "KY", Name = "Kentucky" },
                Population = 1022630,
                Density = 907.6,
                Tags = new[] { "KY", "US" },
                AverageTemperatureMin = 1.1,
                AverageTemperatureMax = 25.7,
                Date = DateTime.Parse("1983-03-07 7:07:24 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 50,
                CountryId = "US",
                Name = "Richmond",
                Region = new Region { Id = "VA", Name = "Virginia" },
                Population = 1008069,
                Density = 1477.0,
                Tags = new[] { "VA", "US" },
                AverageTemperatureMin = 3.5,
                AverageTemperatureMax = 26.3,
                Date = DateTime.Parse("2009-07-30 4:36:37 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 51,
                CountryId = "US",
                Name = "Buffalo",
                Region = new Region { Id = "NY", Name = "New York" },
                Population = 1004655,
                Density = 2446.1,
                Tags = new[] { "NY", "US" },
                AverageTemperatureMin = -3.6,
                AverageTemperatureMax = 22.1,
                Date = DateTime.Parse("1988-08-04 1:29:23 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 52,
                CountryId = "US",
                Name = "Oklahoma City",
                Region = new Region { Id = "OK", Name = "Oklahoma" },
                Population = 994284,
                Density = 413.7,
                Tags = new[] { "OK", "US" },
                AverageTemperatureMin = 3.6,
                AverageTemperatureMax = 27.8,
                Date = DateTime.Parse("2017-09-09 1:25:48 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 53,
                CountryId = "US",
                Name = "Bridgeport",
                Region = new Region { Id = "CT", Name = "Connecticut" },
                Population = 975078,
                Density = 3485.4,
                Tags = new[] { "CT", "US" },
                AverageTemperatureMin = -0.3,
                AverageTemperatureMax = 24.3,
                Date = DateTime.Parse("1990-08-27 2:47:57 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 54,
                CountryId = "US",
                Name = "New Orleans",
                Region = new Region { Id = "LA", Name = "Louisiana" },
                Population = 925443,
                Density = 891.2,
                Tags = new[] { "LA", "US" },
                AverageTemperatureMin = 11.6,
                AverageTemperatureMax = 28.2,
                Date = DateTime.Parse("1992-09-19 8:50:39 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 55,
                CountryId = "US",
                Name = "Fort Worth",
                Region = new Region { Id = "TX", Name = "Texas" },
                Population = 908469,
                Density = 988.3,
                Tags = new[] { "TX", "US" },
                AverageTemperatureMin = 7.7,
                AverageTemperatureMax = 29.6,
                Date = DateTime.Parse("2001-12-23 2:30:46 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 56,
                CountryId = "US",
                Name = "Hartford",
                Region = new Region { Id = "CT", Name = "Connecticut" },
                Population = 907046,
                Density = 2722.5,
                Tags = new[] { "CT", "US" },
                AverageTemperatureMin = -2.7,
                AverageTemperatureMax = 23.5,
                Date = DateTime.Parse("1971-10-13 5:08:29 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 57,
                CountryId = "US",
                Name = "Tucson",
                Region = new Region { Id = "AZ", Name = "Arizona" },
                Population = 875284,
                Density = 873.6,
                Tags = new[] { "AZ", "US" },
                AverageTemperatureMin = 11.3,
                AverageTemperatureMax = 30.8,
                Date = DateTime.Parse("1989-12-27 12:45:24 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 58,
                CountryId = "US",
                Name = "Honolulu",
                Region = new Region { Id = "HI", Name = "Hawaii" },
                Population = 835291,
                Density = 2214.1,
                Tags = new[] { "HI", "US" },
                AverageTemperatureMin = 22.9,
                AverageTemperatureMax = 27.8,
                Date = DateTime.Parse("1975-01-03 5:14:52 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 59,
                CountryId = "US",
                Name = "McAllen",
                Region = new Region { Id = "TX", Name = "Texas" },
                Population = 809002,
                Density = 883.4,
                Tags = new[] { "TX", "US" },
                AverageTemperatureMin = 17.1,
                AverageTemperatureMax = 31.8,
                Date = DateTime.Parse("1986-10-09 10:56:21 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 60,
                CountryId = "US",
                Name = "Omaha",
                Region = new Region { Id = "NE", Name = "Nebraska" },
                Population = 806485,
                Density = 1299.2,
                Tags = new[] { "NE", "US" },
                AverageTemperatureMin = -4.6,
                AverageTemperatureMax = 24.8,
                Date = DateTime.Parse("2019-08-18 3:51:58 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 61,
                CountryId = "US",
                Name = "El Paso",
                Region = new Region { Id = "TX", Name = "Texas" },
                Population = 794344,
                Density = 1015.8,
                Tags = new[] { "TX", "US" },
                AverageTemperatureMin = 7.2,
                AverageTemperatureMax = 28.2,
                Date = DateTime.Parse("2018-09-23 6:40:18 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 62,
                CountryId = "US",
                Name = "Albuquerque",
                Region = new Region { Id = "NM", Name = "New Mexico" },
                Population = 765693,
                Density = 1155.5,
                Tags = new[] { "NM", "US" },
                AverageTemperatureMin = 2.4,
                AverageTemperatureMax = 25.7,
                Date = DateTime.Parse("1997-01-04 6:57:29 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 63,
                CountryId = "US",
                Name = "Rochester",
                Region = new Region { Id = "NY", Name = "New York" },
                Population = 737309,
                Density = 2227.6,
                Tags = new[] { "NY", "US" },
                AverageTemperatureMin = -3.2,
                AverageTemperatureMax = 22.4,
                Date = DateTime.Parse("2014-12-25 1:45:54 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 64,
                CountryId = "US",
                Name = "Sarasota",
                Region = new Region { Id = "FL", Name = "Florida" },
                Population = 727388,
                Density = 1518.1,
                Tags = new[] { "FL", "US" },
                AverageTemperatureMin = 17.9,
                AverageTemperatureMax = 28.0,
                Date = DateTime.Parse("2014-12-25 1:45:54 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 65,
                CountryId = "US",
                Name = "Fresno",
                Region = new Region { Id = "CA", Name = "California" },
                Population = 719558,
                Density = 1757.3,
                Tags = new[] { "CA", "US" },
                AverageTemperatureMin = 8.1,
                AverageTemperatureMax = 28.3,
                Date = DateTime.Parse("1974-06-29 7:08:18 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 66,
                CountryId = "US",
                Name = "Tulsa",
                Region = new Region { Id = "OK", Name = "Oklahoma" },
                Population = 715983,
                Density = 785.8,
                Tags = new[] { "OK", "US" },
                AverageTemperatureMin = 3.6,
                AverageTemperatureMax = 28.6,
                Date = DateTime.Parse("2015-05-11 2:46:09 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 67,
                CountryId = "US",
                Name = "Allentown",
                Region = new Region { Id = "PA", Name = "Pennsylvania" },
                Population = 714136,
                Density = 2663.3,
                Tags = new[] { "PA", "US" },
                AverageTemperatureMin = -1.1,
                AverageTemperatureMax = 24.2,
                Date = DateTime.Parse("1980-11-17 1:42:10 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 68,
                CountryId = "US",
                Name = "Dayton",
                Region = new Region { Id = "OH", Name = "Ohio" },
                Population = 709300,
                Density = 971.7,
                Tags = new[] { "OH", "US" },
                AverageTemperatureMin = -2.1,
                AverageTemperatureMax = 24.6,
                Date = DateTime.Parse("2004-08-18 5:30:09 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 69,
                CountryId = "US",
                Name = "Birmingham",
                Region = new Region { Id = "AL", Name = "Alabama" },
                Population = 704676,
                Density = 553.9,
                Tags = new[] { "AL", "US" },
                AverageTemperatureMin = 4.3,
                AverageTemperatureMax = 16.8,
                Date = DateTime.Parse("2011-01-28 8:55:12 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 70,
                CountryId = "US",
                Name = "Charleston",
                Region = new Region { Id = "SC", Name = "South Carolina" },
                Population = 685517,
                Density = 460.9,
                Tags = new[] { "SC", "US" },
                AverageTemperatureMin = 10.4,
                AverageTemperatureMax = 27.9,
                Date = DateTime.Parse("1974-12-18 6:27:50 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 71,
                CountryId = "US",
                Name = "Cape Coral",
                Region = new Region { Id = "FL", Name = "Florida" },
                Population = 682773,
                Density = 690.5,
                Tags = new[] { "FL", "US" },
                AverageTemperatureMin = 18.0,
                AverageTemperatureMax = 28.3,
                Date = DateTime.Parse("2003-08-01 12:38:38 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 72,
                CountryId = "US",
                Name = "Columbia",
                Region = new Region { Id = "SC", Name = "South Carolina" },
                Population = 640502,
                Density = 377.3,
                Tags = new[] { "SC", "US" },
                AverageTemperatureMin = 7.6,
                AverageTemperatureMax = 28.0,
                Date = DateTime.Parse("2020-06-22 6:04:13 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 73,
                CountryId = "US",
                Name = "Concord",
                Region = new Region { Id = "CA", Name = "California" },
                Population = 640270,
                Density = 1633.1,
                Tags = new[] { "CA", "US" },
                AverageTemperatureMin = 5.4,
                AverageTemperatureMax = 28.6,
                Date = DateTime.Parse("2021-03-16 10:57:14 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 74,
                CountryId = "US",
                Name = "Colorado Springs",
                Region = new Region { Id = "CO", Name = "Colorado" },
                Population = 628808,
                Density = 930.0,
                Tags = new[] { "CO", "US" },
                AverageTemperatureMin = -0.2,
                AverageTemperatureMax = 22.4,
                Date = DateTime.Parse("2004-02-18 7:56:50 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 75,
                CountryId = "US",
                Name = "Springfield",
                Region = new Region { Id = "MA", Name = "Massachusetts" },
                Population = 620494,
                Density = 1861.8,
                Tags = new[] { "MA", "US" },
                AverageTemperatureMin = -2.7,
                AverageTemperatureMax = 23.5,
                Date = DateTime.Parse("1989-12-24 5:46:10 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 76,
                CountryId = "US",
                Name = "Knoxville",
                Region = new Region { Id = "TN", Name = "Tennessee" },
                Population = 619925,
                Density = 733.3,
                Tags = new[] { "TN", "US" },
                AverageTemperatureMin = 3.9,
                AverageTemperatureMax = 25.8,
                Date = DateTime.Parse("2019-09-23 10:52:06 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 77,
                CountryId = "US",
                Name = "Baton Rouge",
                Region = new Region { Id = "LA", Name = "Louisiana" },
                Population = 610751,
                Density = 993.9,
                Tags = new[] { "LA", "US" },
                AverageTemperatureMin = 11.1,
                AverageTemperatureMax = 28.3,
                Date = DateTime.Parse("1985-09-01 11:41:06 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 78,
                CountryId = "US",
                Name = "Ogden",
                Region = new Region { Id = "UT", Name = "Utah" },
                Population = 608259,
                Density = 1221.9,
                Tags = new[] { "UT", "US" },
                AverageTemperatureMin = -5.9,
                AverageTemperatureMax = 33.0,
                Date = DateTime.Parse("1997-06-06 12:06:39 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 79,
                CountryId = "US",
                Name = "Grand Rapids",
                Region = new Region { Id = "MI", Name = "Michigan" },
                Population = 604311,
                Density = 1719.6,
                Tags = new[] { "MI", "US" },
                AverageTemperatureMin = -4.0,
                AverageTemperatureMax = 22.7,
                Date = DateTime.Parse("1995-10-14 11:33:28 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 80,
                CountryId = "US",
                Name = "Albany",
                Region = new Region { Id = "NY", Name = "New York" },
                Population = 604077,
                Density = 1747.3,
                Tags = new[] { "NY", "US" },
                AverageTemperatureMin = -4.2,
                AverageTemperatureMax = 22.8,
                Date = DateTime.Parse("1986-05-03 8:58:13 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 81,
                CountryId = "US",
                Name = "Bakersfield",
                Region = new Region { Id = "CA", Name = "California" },
                Population = 590845,
                Density = 979.0,
                Tags = new[] { "CA", "US" },
                AverageTemperatureMin = 9.7,
                AverageTemperatureMax = 29.3,
                Date = DateTime.Parse("2007-05-22 9:45:49 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 82,
                CountryId = "US",
                Name = "Mission Viejo",
                Region = new Region { Id = "CA", Name = "California" },
                Population = 588540,
                Density = 2091.0,
                Tags = new[] { "CA", "US" },
                AverageTemperatureMin = 6.1,
                AverageTemperatureMax = 25.3,
                Date = DateTime.Parse("1988-11-25 3:29:42 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 83,
                CountryId = "US",
                Name = "New Haven",
                Region = new Region { Id = "CT", Name = "Connecticut" },
                Population = 587648,
                Density = 2693.4,
                Tags = new[] { "CT", "US" },
                AverageTemperatureMin = -0.8,
                AverageTemperatureMax = 23.3,
                Date = DateTime.Parse("1988-06-24 12:00:09 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 84,
                CountryId = "US",
                Name = "Worcester",
                Region = new Region { Id = "MA", Name = "Massachusetts" },
                Population = 573573,
                Density = 1913.8,
                Tags = new[] { "MA", "US" },
                AverageTemperatureMin = -4.1,
                AverageTemperatureMax = 21.6,
                Date = DateTime.Parse("1993-04-18 2:33:04 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 85,
                CountryId = "US",
                Name = "Provo",
                Region = new Region { Id = "UT", Name = "Utah" },
                Population = 551645,
                Density = 1082.6,
                Tags = new[] { "UT", "US" },
                AverageTemperatureMin = 0.2,
                AverageTemperatureMax = 25.9,
                Date = DateTime.Parse("1977-10-04 10:02:52 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 86,
                CountryId = "US",
                Name = "Akron",
                Region = new Region { Id = "OH", Name = "Ohio" },
                Population = 546549,
                Density = 1230.5,
                Tags = new[] { "OH", "US" },
                AverageTemperatureMin = -2.3,
                AverageTemperatureMax = 23.3,
                Date = DateTime.Parse("2014-08-18 5:43:40 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 87,
                CountryId = "US",
                Name = "Palm Bay",
                Region = new Region { Id = "FL", Name = "Florida" },
                Population = 528322,
                Density = 508.2,
                Tags = new[] { "FL", "US" },
                AverageTemperatureMin = 16.1,
                AverageTemperatureMax = 27.9,
                Date = DateTime.Parse("2020-05-04 9:05:30 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 88,
                CountryId = "US",
                Name = "Des Moines",
                Region = new Region { Id = "IA", Name = "Iowa" },
                Population = 514654,
                Density = 943.2,
                Tags = new[] { "IA", "US" },
                AverageTemperatureMin = -5.4,
                AverageTemperatureMax = 24.4,
                Date = DateTime.Parse("2021-03-11 12:06:23 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 89,
                CountryId = "US",
                Name = "Murrieta",
                Region = new Region { Id = "CA", Name = "California" },
                Population = 509526,
                Density = 1310.3,
                Tags = new[] { "CA", "US" },
                AverageTemperatureMin = 12.1,
                AverageTemperatureMax = 26.7,
                Date = DateTime.Parse("2014-04-24 1:05:55 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 90,
                CountryId = "US",
                Name = "Mesa",
                Region = new Region { Id = "AZ", Name = "Arizona" },
                Population = 497752,
                Density = 1415.7,
                Tags = new[] { "AZ", "US" },
                AverageTemperatureMin = 11.7,
                AverageTemperatureMax = 33.6,
                Date = DateTime.Parse("1971-11-26 5:19:48 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 91,
                CountryId = "US",
                Name = "Staten Island",
                Region = new Region { Id = "NY", Name = "New York" },
                Population = 495747,
                Density = 3286.2,
                Tags = new[] { "NY", "US" },
                AverageTemperatureMin = 0.6,
                AverageTemperatureMax = 24.9,
                Date = DateTime.Parse("2002-08-25 3:38:16 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 92,
                CountryId = "US",
                Name = "Wichita",
                Region = new Region { Id = "KS", Name = "Kansas" },
                Population = 491916,
                Density = 930.9,
                Tags = new[] { "KS", "US" },
                AverageTemperatureMin = 0.7,
                AverageTemperatureMax = 27.5,
                Date = DateTime.Parse("1975-08-22 6:38:39 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 93,
                CountryId = "US",
                Name = "Toledo",
                Region = new Region { Id = "OH", Name = "Ohio" },
                Population = 490832,
                Density = 1319.7,
                Tags = new[] { "OH", "US" },
                AverageTemperatureMin = -2.5,
                AverageTemperatureMax = 24.1,
                Date = DateTime.Parse("2018-01-22 5:41:32 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 94,
                CountryId = "US",
                Name = "Harrisburg",
                Region = new Region { Id = "PA", Name = "Pennsylvania" },
                Population = 472261,
                Density = 2342.0,
                Tags = new[] { "PA", "US" },
                AverageTemperatureMin = 0.3,
                AverageTemperatureMax = 25.3,
                Date = DateTime.Parse("1998-09-13 1:16:53 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 95,
                CountryId = "US",
                Name = "Port St. Lucie",
                Region = new Region { Id = "FL", Name = "Florida" },
                Population = 468979,
                Density = 634.0,
                Tags = new[] { "FL", "US" },
                AverageTemperatureMin = 15.2,
                AverageTemperatureMax = 27.8,
                Date = DateTime.Parse("1994-12-15 10:04:14 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 96,
                CountryId = "US",
                Name = "Long Beach",
                Region = new Region { Id = "CA", Name = "California" },
                Population = 466565,
                Density = 3517.9,
                Tags = new[] { "CA", "US" },
                AverageTemperatureMin = 13.7,
                AverageTemperatureMax = 23.5,
                Date = DateTime.Parse("1972-04-25 3:01:42 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 97,
                CountryId = "US",
                Name = "Reno",
                Region = new Region { Id = "NV", Name = "Nevada" },
                Population = 463328,
                Density = 889.9,
                Tags = new[] { "CA", "US" },
                AverageTemperatureMin = 2.3,
                AverageTemperatureMax = 25.1,
                Date = DateTime.Parse("1996-11-21 8:38:16 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 98,
                CountryId = "US",
                Name = "Madison",
                Region = new Region { Id = "WI", Name = "Wisconsin" },
                Population = 461778,
                Density = 1253.6,
                Tags = new[] { "WI", "US" },
                AverageTemperatureMin = -9.4,
                AverageTemperatureMax = 20.6,
                Date = DateTime.Parse("1981-07-25 1:21:44 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 99,
                CountryId = "US",
                Name = "Little Rock",
                Region = new Region { Id = "AR", Name = "Arkansas" },
                Population = 457379,
                Density = 637.0,
                Tags = new[] { "AR", "US" },
                AverageTemperatureMin = 4.8,
                AverageTemperatureMax = 27.4,
                Date = DateTime.Parse("2014-02-14 10:16:49 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 100,
                CountryId = "US",
                Name = "Greenville",
                Region = new Region { Id = "SC", Name = "South Carolina" },
                Population = 450487,
                Density = 903.6,
                Tags = new[] { "SC", "US" },
                AverageTemperatureMin = 5.8,
                AverageTemperatureMax = 26.5,
                Date = DateTime.Parse("1996-08-14 12:25:13 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1001,
                CountryId = "CA",
                Name = "Toronto",
                Region = new Region { Id = "ON", Name = "Ontario" },
                Population = 5429524,
                Density = 4334.4,
                Tags = new[] { "ON", "CA" },
                AverageTemperatureMin = -3.7,
                AverageTemperatureMax = 22.3,
                Date = DateTime.Parse("1981-08-25 12:45:32 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1002,
                CountryId = "CA",
                Name = "Montreal",
                Region = new Region { Id = "QC", Name = "Quebec" },
                Population = 3519595,
                Density = 3889.0,
                Tags = new[] { "QC", "CA" },
                AverageTemperatureMin = -9.7,
                AverageTemperatureMax = 21.2,
                Date = DateTime.Parse("1984-03-11 8:40:04 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1003,
                CountryId = "CA",
                Name = "Vancouver",
                Region = new Region { Id = "BC", Name = "British Columbia" },
                Population = 2264823,
                Density = 5492.6,
                Tags = new[] { "BC", "CA" },
                AverageTemperatureMin = 3.6,
                AverageTemperatureMax = 18.0,
                Date = DateTime.Parse("1970-03-30 6:10:36 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1004,
                CountryId = "CA",
                Name = "Calgary",
                Region = new Region { Id = "AB", Name = "Alberta" },
                Population = 1239220,
                Density = 1501.1,
                Tags = new[] { "AB", "CA" },
                AverageTemperatureMin = -7.1,
                AverageTemperatureMax = 16.5,
                Date = DateTime.Parse("2021-09-01 3:44:19 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1005,
                CountryId = "CA",
                Name = "Edmonton",
                Region = new Region { Id = "AB", Name = "Alberta" },
                Population = 1062643,
                Density = 1360.9,
                Tags = new[] { "AB", "CA" },
                AverageTemperatureMin = -10.4,
                AverageTemperatureMax = 17.7,
                Date = DateTime.Parse("1975-09-06 1:40:26 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1006,
                CountryId = "CA",
                Name = "Ottawa",
                Region = new Region { Id = "ON", Name = "Ontario" },
                Population = 989567,
                Density = 334.0,
                Tags = new[] { "ON", "CA" },
                AverageTemperatureMin = -10.2,
                AverageTemperatureMax = 21.2,
                Date = DateTime.Parse("1989-09-18 3:55:12 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1007,
                CountryId = "CA",
                Name = "Mississauga",
                Region = new Region { Id = "ON", Name = "Ontario" },
                Population = 721599,
                Density = 2467.6,
                Tags = new[] { "ON", "CA" },
                AverageTemperatureMin = -5.5,
                AverageTemperatureMax = 21.5,
                Date = DateTime.Parse("1979-04-13 11:11:30 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1008,
                CountryId = "CA",
                Name = "Winnipeg",
                Region = new Region { Id = "MB", Name = "Manitoba" },
                Population = 705244,
                Density = 1430.0,
                Tags = new[] { "MB", "CA" },
                AverageTemperatureMin = -16.4,
                AverageTemperatureMax = 19.7,
                Date = DateTime.Parse("2017-04-15 6:24:08 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1009,
                CountryId = "CA",
                Name = "Quebec City",
                Region = new Region { Id = "QC", Name = "Quebec" },
                Population = 705103,
                Density = 1173.2,
                Tags = new[] { "QC", "CA" },
                AverageTemperatureMin = -12.8,
                AverageTemperatureMax = 19.8,
                Date = DateTime.Parse("2007-02-28 4:55:35 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1010,
                CountryId = "CA",
                Name = "Hamilton",
                Region = new Region { Id = "ON", Name = "Ontario" },
                Population = 693645,
                Density = 480.6,
                Tags = new[] { "ON", "CA" },
                AverageTemperatureMin = 18.0,
                AverageTemperatureMax = 27.6,
                Date = DateTime.Parse("2010-07-25 7:16:44 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1011,
                CountryId = "CA",
                Name = "Brampton",
                Region = new Region { Id = "ON", Name = "Ontario" },
                Population = 593638,
                Density = 2228.7,
                Tags = new[] { "ON", "CA" },
                AverageTemperatureMin = -5.5,
                AverageTemperatureMax = 21.5,
                Date = DateTime.Parse("1970-10-23 4:26:50 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1012,
                CountryId = "CA",
                Name = "Surrey",
                Region = new Region { Id = "BC", Name = "British Columbia" },
                Population = 517887,
                Density = 1636.8,
                Tags = new[] { "BC", "CA" },
                AverageTemperatureMin = 3.6,
                AverageTemperatureMax = 18.0,
                Date = DateTime.Parse("1971-06-26 10:47:34 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1013,
                CountryId = "CA",
                Name = "Kitchener",
                Region = new Region { Id = "ON", Name = "Ontario" },
                Population = 470015,
                Density = 3433.5,
                Tags = new[] { "ON", "CA" },
                AverageTemperatureMin = -6.5,
                AverageTemperatureMax = 20.0,
                Date = DateTime.Parse("2003-03-11 9:43:19 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1014,
                CountryId = "CA",
                Name = "Laval",
                Region = new Region { Id = "QC", Name = "Quebec" },
                Population = 422993,
                Density = 1710.9,
                Tags = new[] { "QC", "CA" },
                AverageTemperatureMin = -10.3,
                AverageTemperatureMax = 21.4,
                Date = DateTime.Parse("1988-05-23 1:02:49 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1015,
                CountryId = "CA",
                Name = "Halifax",
                Region = new Region { Id = "NS", Name = "Nova Scotia" },
                Population = 403131,
                Density = 73.4,
                Tags = new[] { "NS", "CA" },
                AverageTemperatureMin = -4.1,
                AverageTemperatureMax = 19.1,
                Date = DateTime.Parse("2013-10-24 2:49:29 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1016,
                CountryId = "CA",
                Name = "London",
                Region = new Region { Id = "ON", Name = "Ontario" },
                Population = 383822,
                Density = 913.1,
                Tags = new[] { "ON", "CA" },
                AverageTemperatureMin = -5.6,
                AverageTemperatureMax = 20.8,
                Date = DateTime.Parse("2003-03-13 7:24:23 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1017,
                CountryId = "CA",
                Name = "Victoria",
                Region = new Region { Id = "BC", Name = "British Columbia" },
                Population = 335696,
                Density = 4406.3,
                Tags = new[] { "BC", "CA" },
                AverageTemperatureMin = 5.0,
                AverageTemperatureMax = 15.9,
                Date = DateTime.Parse("1989-06-06 7:48:02 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1018,
                CountryId = "CA",
                Name = "Markham",
                Region = new Region { Id = "ON", Name = "Ontario" },
                Population = 328966,
                Density = 1549.2,
                Tags = new[] { "ON", "CA" },
                AverageTemperatureMin = -5.8,
                AverageTemperatureMax = 21.2,
                Date = DateTime.Parse("1971-01-19 8:01:33 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1019,
                CountryId = "CA",
                Name = "St. Catharines",
                Region = new Region { Id = "ON", Name = "Ontario" },
                Population = 309319,
                Density = 1384.8,
                Tags = new[] { "ON", "CA" },
                AverageTemperatureMin = -3.8,
                AverageTemperatureMax = 21.9,
                Date = DateTime.Parse("1991-06-12 10:26:49 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1020,
                CountryId = "CA",
                Name = "Niagara Falls",
                Region = new Region { Id = "ON", Name = "Ontario" },
                Population = 308596,
                Density = 419.9,
                Tags = new[] { "ON", "CA" },
                AverageTemperatureMin = -4.1,
                AverageTemperatureMax = 22.2,
                Date = DateTime.Parse("1976-12-30 9:57:54 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1021,
                CountryId = "CA",
                Name = "Vaughan",
                Region = new Region { Id = "ON", Name = "Ontario" },
                Population = 306233,
                Density = 1119.4,
                Tags = new[] { "ON", "CA" },
                AverageTemperatureMin = -6.6,
                AverageTemperatureMax = 20.8,
                Date = DateTime.Parse("1999-05-28 11:45:20 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1022,
                CountryId = "CA",
                Name = "Gatineau",
                Region = new Region { Id = "QC", Name = "Quebec" },
                Population = 276245,
                Density = 773.7,
                Tags = new[] { "QC", "CA" },
                AverageTemperatureMin = -10.2,
                AverageTemperatureMax = 21.2,
                Date = DateTime.Parse("2006-12-13 1:22:57 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1023,
                CountryId = "CA",
                Name = "Windsor",
                Region = new Region {  Id = "ON", Name = "Ontario" },
                Population = 276165,
                Density = 1484.3,
                Tags = new[] { "ON", "CA" },
                AverageTemperatureMin = -3.8,
                AverageTemperatureMax = 23.0,
                Date = DateTime.Parse("1988-04-13 5:04:35 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1024,
                CountryId = "CA",
                Name = "Saskatoon",
                Region = new Region { Id = "SK", Name = "Saskatchewan" },
                Population = 246376,
                Density = 1080.0,
                Tags = new[] { "SK", "CA" },
                AverageTemperatureMin = -13.9,
                AverageTemperatureMax = 19.0,
                Date = DateTime.Parse("1995-08-29 4:57:23 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1025,
                CountryId = "CA",
                Name = "Longueuil",
                Region = new Region { Id = "QC", Name = "Quebec" },
                Population = 239700,
                Density = 2002.0,
                Tags = new[] { "QC", "CA" },
                AverageTemperatureMin = -10.4,
                AverageTemperatureMax = 20.6,
                Date = DateTime.Parse("2016-03-31 2:40:27 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1026,
                CountryId = "CA",
                Name = "Burnaby",
                Region = new Region { Id = "BC", Name = "British Columbia" },
                Population = 232755,
                Density = 2568.7,
                Tags = new[] { "BC", "CA" },
                AverageTemperatureMin = 3.6,
                AverageTemperatureMax = 18.0,
                Date = DateTime.Parse("1994-11-07 12:13:48 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1027,
                CountryId = "CA",
                Name = "Regina",
                Region = new Region { Id = "SK", Name = "Saskatchewan" },
                Population = 215106,
                Density = 1195.2,
                Tags = new[] { "SK", "CA" },
                AverageTemperatureMin = -14.7,
                AverageTemperatureMax = 18.9,
                Date = DateTime.Parse("2015-01-15 6:37:04 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1028,
                CountryId = "CA",
                Name = "Richmond",
                Region = new Region { Id = "BC", Name = "British Columbia" },
                Population = 198309,
                Density = 1534.1,
                Tags = new[] { "BC", "CA" },
                AverageTemperatureMin = 3.6,
                AverageTemperatureMax = 18.0,
                Date = DateTime.Parse("1978-01-09 1:33:04 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1029,
                CountryId = "CA",
                Name = "Richmond Hill",
                Region = new Region { Id = "ON", Name = "Ontario" },
                Population = 195022,
                Density = 1928.8,
                Tags = new[] { "ON", "CA" },
                AverageTemperatureMin = -6.2,
                AverageTemperatureMax = 21.4,
                Date = DateTime.Parse("2021-07-18 2:48:21 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1030,
                CountryId = "CA",
                Name = "Oakville",
                Region = new Region { Id = "ON", Name = "Ontario" },
                Population = 193832,
                Density = 1314.2,
                Tags = new[] { "ON", "CA" },
                AverageTemperatureMin = -4.7,
                AverageTemperatureMax = 20.9,
                Date = DateTime.Parse("1978-12-25 6:09:08 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1031,
                CountryId = "CA",
                Name = "Burlington",
                Region = new Region { Id = "ON", Name = "Ontario" },
                Population = 183314,
                Density = 946.8,
                Tags = new[] { "ON", "CA" },
                AverageTemperatureMin = -4.4,
                AverageTemperatureMax = 22.5,
                Date = DateTime.Parse("2013-05-09 2:46:01 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1032,
                CountryId = "CA",
                Name = "Barrie",
                Region = new Region { Id = "ON", Name = "Ontario" },
                Population = 172657,
                Density = 1428.0,
                Tags = new[] { "ON", "CA" },
                AverageTemperatureMin = -8.1,
                AverageTemperatureMax = 19.6,
                Date = DateTime.Parse("1975-08-09 8:51:16 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1033,
                CountryId = "CA",
                Name = "Oshawa",
                Region = new Region { Id = "ON", Name = "Ontario" },
                Population = 166000,
                Density = 1027.0,
                Tags = new[] { "ON", "CA" },
                AverageTemperatureMin = -4.8,
                AverageTemperatureMax = 20.6,
                Date = DateTime.Parse("2004-11-29 10:10:16 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1034,
                CountryId = "CA",
                Name = "Sherbrooke",
                Region = new Region { Id = "QC", Name = "Quebec" },
                Population = 161323,
                Density = 456.0,
                Tags = new[] { "QC", "CA" },
                AverageTemperatureMin = -5.8,
                AverageTemperatureMax = 24.6,
                Date = DateTime.Parse("2009-04-03 3:35:28 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1035,
                CountryId = "CA",
                Name = "Saguenay",
                Region = new Region { Id = "QC", Name = "Quebec" },
                Population = 144746,
                Density = 128.5,
                Tags = new[] { "QC", "CA" },
                AverageTemperatureMin = -15.7,
                AverageTemperatureMax = 18.4,
                Date = DateTime.Parse("1984-02-23 10:24:32 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1036,
                CountryId = "CA",
                Name = "Levis",
                Region = new Region { Id = "QC", Name = "Quebec" },
                Population = 143414,
                Density = 319.4,
                Tags = new[] { "QC", "CA" },
                AverageTemperatureMin = -15.7,
                AverageTemperatureMax = 18.4,
                Date = DateTime.Parse("2004-03-24 10:45:28 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1037,
                CountryId = "CA",
                Name = "Kelowna",
                Region = new Region { Id = "BC", Name = "British Columbia" },
                Population = 142146,
                Density = 601.3,
                Tags = new[] { "BC", "CA" },
                AverageTemperatureMin = -2.5,
                AverageTemperatureMax = 19.5,
                Date = DateTime.Parse("1983-01-06 8:48:29 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1038,
                CountryId = "CA",
                Name = "Abbotsford",
                Region = new Region { Id = "BC", Name = "British Columbia" },
                Population = 141397,
                Density = 376.5,
                Tags = new[] { "BC", "CA" },
                AverageTemperatureMin = 3.6,
                AverageTemperatureMax = 18.0,
                Date = DateTime.Parse("2018-07-24 3:24:07 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1039,
                CountryId = "CA",
                Name = "Coquitlam",
                Region = new Region { Id = "BC", Name = "British Columbia" },
                Population = 139284,
                Density = 1138.9,
                Tags = new[] { "BC", "CA" },
                AverageTemperatureMin = 3.6,
                AverageTemperatureMax = 18.0,
                Date = DateTime.Parse("1998-11-12 5:46:00 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1040,
                CountryId = "CA",
                Name = "Trois-Rivieres",
                Region = new Region { Id = "QC", Name = "Quebec" },
                Population = 134413,
                Density = 1581.2,
                Tags = new[] { "QC", "CA" },
                AverageTemperatureMin = -12.1,
                AverageTemperatureMax = 20.0,
                Date = DateTime.Parse("1999-03-27 11:03:42 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1041,
                CountryId = "CA",
                Name = "Guelph",
                Region = new Region { Id = "ON", Name = "Ontario" },
                Population = 131794,
                Density = 1511.1,
                Tags = new[] { "ON", "CA" },
                AverageTemperatureMin = -6.9,
                AverageTemperatureMax = 19.7,
                Date = DateTime.Parse("1987-11-17 3:38:36 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1042,
                CountryId = "CA",
                Name = "Cambridge",
                Region = new Region { Id = "ON", Name = "Ontario" },
                Population = 129920,
                Density = 1149.6,
                Tags = new[] { "ON", "CA" },
                AverageTemperatureMin = -6.0,
                AverageTemperatureMax = 20.6,
                Date = DateTime.Parse("1985-02-10 10:37:14 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1043,
                CountryId = "CA",
                Name = "Whitby",
                Region = new Region { Id = "ON", Name = "Ontario" },
                Population = 128377,
                Density = 876.1,
                Tags = new[] { "ON", "CA" },
                AverageTemperatureMin = -4.8,
                AverageTemperatureMax = 20.6,
                Date = DateTime.Parse("2015-02-12 5:04:02 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1044,
                CountryId = "CA",
                Name = "Ajax",
                Region = new Region { Id = "ON", Name = "Ontario" },
                Population = 119677,
                Density = 1634.2,
                Tags = new[] { "ON", "CA" },
                AverageTemperatureMin = -4.8,
                AverageTemperatureMax = 20.6,
                Date = DateTime.Parse("1992-02-15 12:47:21 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1045,
                CountryId = "CA",
                Name = "Langley",
                Region = new Region { Id = "BC", Name = "British Columbia" },
                Population = 117285,
                Density = 380.8,
                Tags = new[] { "BC", "CA" },
                AverageTemperatureMin = 3.6,
                AverageTemperatureMax = 18.0,
                Date = DateTime.Parse("2005-05-07 12:10:12 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1046,
                CountryId = "CA",
                Name = "Saanich",
                Region = new Region { Id = "BC", Name = "British Columbia" },
                Population = 114148,
                Density = 1099.9,
                Tags = new[] { "BC", "CA" },
                AverageTemperatureMin = 5.0,
                AverageTemperatureMax = 15.9,
                Date = DateTime.Parse("1985-12-20 5:12:55 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1047,
                CountryId = "CA",
                Name = "Terrebonne",
                Region = new Region { Id = "QC", Name = "Quebec" },
                Population = 111575,
                Density = 687.1,
                Tags = new[] { "QC", "CA" },
                AverageTemperatureMin = -9.7,
                AverageTemperatureMax = 21.7,
                Date = DateTime.Parse("1998-11-07 7:56:48 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1048,
                CountryId = "CA",
                Name = "Milton",
                Region = new Region { Id = "ON", Name = "Ontario" },
                Population = 110128,
                Density = 230.11,
                Tags = new[] { "ON", "CA" },
                AverageTemperatureMin = -6.8,
                AverageTemperatureMax = 20.6,
                Date = DateTime.Parse("1995-05-14 11:21:02 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1049,
                CountryId = "CA",
                Name = "St. John's",
                Region = new Region { Id = "NL", Name = "Newfoundland and Labrador" },
                Population = 108860,
                Density = 244.10,
                Tags = new[] { "NL", "CA" },
                AverageTemperatureMin = -4.5,
                AverageTemperatureMax = 16.1,
                Date = DateTime.Parse("1989-12-12 2:30:03 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1050,
                CountryId = "CA",
                Name = "Moncton",
                Region = new Region { Id = "NB", Name = "New Brunswick" },
                Population = 108620,
                Density = 506.0,
                Tags = new[] { "NB", "CA" },
                AverageTemperatureMin = -8.2,
                AverageTemperatureMax = 19.5,
                Date = DateTime.Parse("2014-08-11 1:42:18 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1051,
                CountryId = "CA",
                Name = "Thunder Bay",
                Region = new Region { Id = "ON", Name = "Ontario" },
                Population = 107909,
                Density = 330.1,
                Tags = new[] { "ON", "CA" },
                AverageTemperatureMin = -14.3,
                AverageTemperatureMax = 17.7,
                Date = DateTime.Parse("1987-10-17 1:52:18 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1052,
                CountryId = "CA",
                Name = "Dieppe",
                Region = new Region { Id = "NB", Name = "New Brunswick" },
                Population = 107068,
                Density = 469.6,
                Tags = new[] { "NB", "CA" },
                AverageTemperatureMin = 5.2,
                AverageTemperatureMax = 17.4,
                Date = DateTime.Parse("2002-09-02 3:41:45 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1053,
                CountryId = "CA",
                Name = "Waterloo",
                Region = new Region { Id = "ON", Name = "Ontario" },
                Population = 104986,
                Density = 1520.7,
                Tags = new[] { "ON", "CA" },
                AverageTemperatureMin = -6.5,
                AverageTemperatureMax = 20.0,
                Date = DateTime.Parse("1984-05-08 8:23:47 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1054,
                CountryId = "CA",
                Name = "Delta",
                Region = new Region { Id = "BC", Name = "British Columbia" },
                Population = 102238,
                Density = 567.4,
                Tags = new[] { "BC", "CA" },
                AverageTemperatureMin = 3.6,
                AverageTemperatureMax = 18.0,
                Date = DateTime.Parse("2002-07-14 3:31:26 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1055,
                CountryId = "CA",
                Name = "Chatham",
                Region = new Region { Id = "ON", Name = "Ontario" },
                Population = 101647,
                Density = 41.40,
                Tags = new[] { "ON", "CA" },
                AverageTemperatureMin = -3.6,
                AverageTemperatureMax = 22.6,
                Date = DateTime.Parse("2010-04-03 5:45:04 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1056,
                CountryId = "CA",
                Name = "Red Deer",
                Region = new Region { Id = "AB", Name = "Alberta" },
                Population = 100418,
                Density = 958.8,
                Tags = new[] { "AB", "CA" },
                AverageTemperatureMin = -10.2,
                AverageTemperatureMax = 16.8,
                Date = DateTime.Parse("2013-11-03 7:44:01 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1057,
                CountryId = "CA",
                Name = "Kamloops",
                Region = new Region { Id = "BC", Name = "British Columbia" },
                Population = 100046,
                Density = 301.7,
                Tags = new[] { "BC", "CA" },
                AverageTemperatureMin = -2.8,
                AverageTemperatureMax = 21.5,
                Date = DateTime.Parse("1988-02-29 6:34:21 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1058,
                CountryId = "CA",
                Name = "Brantford",
                Region = new Region { Id = "ON", Name = "Ontario" },
                Population = 97496,
                Density = 1345.9,
                Tags = new[] { "ON", "CA" },
                AverageTemperatureMin = -6.0,
                AverageTemperatureMax = 21.3,
                Date = DateTime.Parse("1988-11-15 1:31:10 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1059,
                CountryId = "CA",
                Name = "Cape Breton",
                Region = new Region { Id = "NS", Name = "Nova Scotia" },
                Population = 94285,
                Density = 38.8,
                Tags = new[] { "NS", "CA" },
                AverageTemperatureMin = -5.4,
                AverageTemperatureMax = 18.0,
                Date = DateTime.Parse("2004-02-20 9:36:01 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1060,
                CountryId = "CA",
                Name = "Lethbridge",
                Region = new Region { Id = "AB", Name = "Alberta" },
                Population = 92729,
                Density = 759.5,
                Tags = new[] { "AB", "CA" },
                AverageTemperatureMin = -6.0,
                AverageTemperatureMax = 18.2,
                Date = DateTime.Parse("2018-08-05 1:33:05 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1061,
                CountryId = "CA",
                Name = "Saint-Jean-sur-Richelieu",
                Region = new Region { Id = "QC", Name = "Quebec" },
                Population = 92394,
                Density = 419.7,
                Tags = new[] { "QC", "CA" },
                AverageTemperatureMin = -12.8,
                AverageTemperatureMax = 19.8,
                Date = DateTime.Parse("1982-04-22 7:28:57 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1062,
                CountryId = "CA",
                Name = "Clarington",
                Region = new Region { Id = "ON", Name = "Ontario" },
                Population = 92013,
                Density = 138.3,
                Tags = new[] { "ON", "CA" },
                AverageTemperatureMin = -5.6,
                AverageTemperatureMax = 20.0,
                Date = DateTime.Parse("1994-11-10 10:10:41 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1063,
                CountryId = "CA",
                Name = "Pickering",
                Region = new Region { Id = "ON", Name = "Ontario" },
                Population = 91771,
                Density = 383.1,
                Tags = new[] { "ON", "CA" },
                AverageTemperatureMin = -4.8,
                AverageTemperatureMax = 20.6,
                Date = DateTime.Parse("1988-12-06 7:08:34 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1064,
                CountryId = "CA",
                Name = "Nanaimo",
                Region = new Region { Id = "BC", Name = "British Columbia" },
                Population = 90504,
                Density = 918.0,
                Tags = new[] { "BC", "CA" },
                AverageTemperatureMin = 3.5,
                AverageTemperatureMax = 18.2,
                Date = DateTime.Parse("2010-01-08 10:30:24 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1065,
                CountryId = "CA",
                Name = "Sudbury",
                Region = new Region { Id = "ON", Name = "Ontario" },
                Population = 88054,
                Density = 49.7,
                Tags = new[] { "ON", "CA" },
                AverageTemperatureMin = -13.0,
                AverageTemperatureMax = 19.1,
                Date = DateTime.Parse("2011-04-26 3:33:15 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1066,
                CountryId = "CA",
                Name = "North Vancouver",
                Region = new Region { Id = "BC", Name = "British Columbia" },
                Population = 85935,
                Density = 534.60,
                Tags = new[] { "BC", "CA" },
                AverageTemperatureMin = 3.6,
                AverageTemperatureMax = 18.0,
                Date = DateTime.Parse("2007-09-22 12:44:31 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1067,
                CountryId = "CA",
                Name = "Brossard",
                Region = new Region { Id = "QC", Name = "Quebec" },
                Population = 85721,
                Density = 1896.0,
                Tags = new[] { "QC", "CA" },
                AverageTemperatureMin = -13.2,
                AverageTemperatureMax = 21.4,
                Date = DateTime.Parse("2017-09-26 1:19:22 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1068,
                CountryId = "CA",
                Name = "Repentigny",
                Region = new Region { Id = "QC", Name = "Quebec" },
                Population = 84965,
                Density = 1395.4,
                Tags = new[] { "QC", "CA" },
                AverageTemperatureMin = -13.2,
                AverageTemperatureMax = 21.4,
                Date = DateTime.Parse("1988-03-05 6:46:48 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1069,
                CountryId = "CA",
                Name = "Newmarket",
                Region = new Region { Id = "ON", Name = "Ontario" },
                Population = 84224,
                Density = 2190.5,
                Tags = new[] { "ON", "CA" },
                AverageTemperatureMin = -7.4,
                AverageTemperatureMax = 20.5,
                Date = DateTime.Parse("2006-03-13 9:55:18 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1070,
                CountryId = "CA",
                Name = "Chilliwack",
                Region = new Region { Id = "BC", Name = "British Columbia" },
                Population = 83788,
                Density = 320.2,
                Tags = new[] { "BC", "CA" },
                AverageTemperatureMin = 3.6,
                AverageTemperatureMax = 18.0,
                Date = DateTime.Parse("1996-11-25 12:43:37 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1071,
                CountryId = "CA",
                Name = "White Rock",
                Region = new Region { Id = "BC", Name = "British Columbia" },
                Population = 82368,
                Density = 3893.1,
                Tags = new[] { "BC", "CA" },
                AverageTemperatureMin = 3.6,
                AverageTemperatureMax = 18.0,
                Date = DateTime.Parse("2020-09-22 6:06:13 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1072,
                CountryId = "CA",
                Name = "Maple Ridge",
                Region = new Region { Id = "BC", Name = "British Columbia" },
                Population = 82256,
                Density = 308.3,
                Tags = new[] { "BC", "CA" },
                AverageTemperatureMin = 3.6,
                AverageTemperatureMax = 18.0,
                Date = DateTime.Parse("2020-07-24 6:40:59 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1073,
                CountryId = "CA",
                Name = "Peterborough",
                Region = new Region { Id = "ON", Name = "Ontario" },
                Population = 82094,
                Density = 1261.2,
                Tags = new[] { "ON", "CA" },
                AverageTemperatureMin = -8.4,
                AverageTemperatureMax = 20.7,
                Date = DateTime.Parse("2019-12-19 5:20:58 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1074,
                CountryId = "CA",
                Name = "Kawartha Lakes",
                Region = new Region { Id = "ON", Name = "Ontario" },
                Population = 75423,
                Density = 24.5,
                Tags = new[] { "ON", "CA" },
                AverageTemperatureMin = -8.4,
                AverageTemperatureMax = 20.3,
                Date = DateTime.Parse("2006-09-11 3:24:58 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1075,
                CountryId = "CA",
                Name = "Prince George",
                Region = new Region { Id = "BC", Name = "British Columbia" },
                Population = 74003,
                Density = 232.5,
                Tags = new[] { "BC", "CA" },
                AverageTemperatureMin = -6.7,
                AverageTemperatureMax = 16.6,
                Date = DateTime.Parse("2015-07-01 8:21:17 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1076,
                CountryId = "CA",
                Name = "Sault Ste. Marie",
                Region = new Region { Id = "ON", Name = "Ontario" },
                Population = 73368,
                Density = 328.6,
                Tags = new[] { "ON", "CA" },
                AverageTemperatureMin = -11.2,
                AverageTemperatureMax = 19.3,
                Date = DateTime.Parse("2005-10-22 2:36:28 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1077,
                CountryId = "CA",
                Name = "Sarnia",
                Region = new Region { Id = "ON", Name = "Ontario" },
                Population = 71594,
                Density = 434.3,
                Tags = new[] { "ON", "CA" },
                AverageTemperatureMin = -4.8,
                AverageTemperatureMax = 21.1,
                Date = DateTime.Parse("2015-09-14 7:33:36 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1078,
                CountryId = "CA",
                Name = "Wood Buffalo",
                Region = new Region { Id = "AB", Name = "Alberta" },
                Population = 71589,
                Density = 1.20,
                Tags = new[] { "AB", "CA" },
                AverageTemperatureMin = -17.4,
                AverageTemperatureMax = 17.1,
                Date = DateTime.Parse("1987-07-27 1:18:29 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1079,
                CountryId = "CA",
                Name = "New Westminster",
                Region = new Region { Id = "BC", Name = "British Columbia" },
                Population = 70996,
                Density = 4543.4,
                Tags = new[] { "BC", "CA" },
                AverageTemperatureMin = 3.6,
                AverageTemperatureMax = 18.0,
                Date = DateTime.Parse("1971-06-13 2:32:50 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1080,
                CountryId = "CA",
                Name = "Chateauguay",
                Region = new Region { Id = "QC", Name = "Quebec" },
                Population = 70812,
                Density = 1278.9,
                Tags = new[] { "QC", "CA" },
                AverageTemperatureMin = -9.7,
                AverageTemperatureMax = 21.2,
                Date = DateTime.Parse("1988-05-24 11:33:48 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1081,
                CountryId = "CA",
                Name = "Saint-Jerome",
                Region = new Region { Id = "QC", Name = "Quebec" },
                Population = 69598,
                Density = 756.3,
                Tags = new[] { "QC", "CA" },
                AverageTemperatureMin = -9.7,
                AverageTemperatureMax = 21.2,
                Date = DateTime.Parse("1987-01-21 4:18:41 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1082,
                CountryId = "CA",
                Name = "Drummondville",
                Region = new Region { Id = "QC", Name = "Quebec" },
                Population = 68601,
                Density = 1315.4,
                Tags = new[] { "QC", "CA" },
                AverageTemperatureMin = -10.2,
                AverageTemperatureMax = 20.9,
                Date = DateTime.Parse("2021-11-06 10:06:59 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1083,
                CountryId = "CA",
                Name = "Saint John",
                Region = new Region { Id = "NB", Name = "New Brunswick" },
                Population = 67575,
                Density = 213.8,
                Tags = new[] { "NB", "CA" },
                AverageTemperatureMin = -7.9,
                AverageTemperatureMax = 17.1,
                Date = DateTime.Parse("1979-06-27 6:57:30 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1084,
                CountryId = "CA",
                Name = "Caledon",
                Region = new Region { Id = "ON", Name = "Ontario" },
                Population = 66502,
                Density = 96.6,
                Tags = new[] { "ON", "CA" },
                AverageTemperatureMin = -7.0,
                AverageTemperatureMax = 19.9,
                Date = DateTime.Parse("2011-04-15 4:02:32 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1085,
                CountryId = "CA",
                Name = "St. Albert",
                Region = new Region { Id = "AB", Name = "Alberta" },
                Population = 65589,
                Density = 1353.9,
                Tags = new[] { "AB", "CA" },
                AverageTemperatureMin = -10.4,
                AverageTemperatureMax = 17.7,
                Date = DateTime.Parse("1986-04-13 5:37:40 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1086,
                CountryId = "CA",
                Name = "Granby",
                Region = new Region { Id = "QC", Name = "Quebec" },
                Population = 63433,
                Density = 415.3,
                Tags = new[] { "QC", "CA" },
                AverageTemperatureMin = -10.0,
                AverageTemperatureMax = 20.1,
                Date = DateTime.Parse("1981-11-13 5:02:48 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1087,
                CountryId = "CA",
                Name = "Medicine Hat",
                Region = new Region { Id = "AB", Name = "Alberta" },
                Population = 63260,
                Density = 564.6,
                Tags = new[] { "AB", "CA" },
                AverageTemperatureMin = -8.4,
                AverageTemperatureMax = 20.0,
                Date = DateTime.Parse("1980-01-04 6:45:30 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1088,
                CountryId = "CA",
                Name = "Grande Prairie",
                Region = new Region { Id = "AB", Name = "Alberta" },
                Population = 63166,
                Density = 475.9,
                Tags = new[] { "AB", "CA" },
                AverageTemperatureMin = -13.6,
                AverageTemperatureMax = 16.2,
                Date = DateTime.Parse("2022-01-11 3:09:57 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1089,
                CountryId = "CA",
                Name = "St. Thomas",
                Region = new Region { Id = "ON", Name = "Ontario" },
                Population = 61707,
                Density = 1067.3,
                Tags = new[] { "ON", "CA" },
                AverageTemperatureMin = -4.7,
                AverageTemperatureMax = 21.2,
                Date = DateTime.Parse("2008-11-05 11:43:26 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1090,
                CountryId = "CA",
                Name = "Airdrie",
                Region = new Region { Id = "AB", Name = "Alberta" },
                Population = 61581,
                Density = 728.2,
                Tags = new[] { "AB", "CA" },
                AverageTemperatureMin = -7.1,
                AverageTemperatureMax = 16.5,
                Date = DateTime.Parse("2005-09-20 7:56:25 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1091,
                CountryId = "CA",
                Name = "Halton Hills",
                Region = new Region { Id = "ON", Name = "Ontario" },
                Population = 61161,
                Density = 221.4,
                Tags = new[] { "ON", "CA" },
                AverageTemperatureMin = -6.3,
                AverageTemperatureMax = 20.0,
                Date = DateTime.Parse("1974-11-01 3:48:56 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1092,
                CountryId = "CA",
                Name = "Saint-Hyacinthe",
                Region = new Region { Id = "QC", Name = "Quebec" },
                Population = 59614,
                Density = 294.5,
                Tags = new[] { "QC", "CA" },
                AverageTemperatureMin = -9.7,
                AverageTemperatureMax = 21.2,
                Date = DateTime.Parse("1974-05-06 8:50:33 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1093,
                CountryId = "CA",
                Name = "Lac-Brome",
                Region = new Region { Id = "QC", Name = "Quebec" },
                Population = 58889,
                Density = 27.3,
                Tags = new[] { "QC", "CA" },
                AverageTemperatureMin = -9.7,
                AverageTemperatureMax = 21.2,
                Date = DateTime.Parse("2002-04-20 12:04:58 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1094,
                CountryId = "CA",
                Name = "Port Coquitlam",
                Region = new Region { Id = "BC", Name = "British Columbia" },
                Population = 58612,
                Density = 2009.4,
                Tags = new[] { "BC", "CA" },
                AverageTemperatureMin = 3.6,
                AverageTemperatureMax = 18.0,
                Date = DateTime.Parse("1975-03-24 5:49:19 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1095,
                CountryId = "CA",
                Name = "Fredericton",
                Region = new Region { Id = "NB", Name = "New Brunswick" },
                Population = 58220,
                Density = 439.2,
                Tags = new[] { "NB", "CA" },
                AverageTemperatureMin = -9.4,
                AverageTemperatureMax = 19.4,
                Date = DateTime.Parse("2008-11-20 3:36:15 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1096,
                CountryId = "CA",
                Name = "Blainville",
                Region = new Region { Id = "QC", Name = "Quebec" },
                Population = 56363,
                Density = 1030.9,
                Tags = new[] { "QC", "CA" },
                AverageTemperatureMin = -9.7,
                AverageTemperatureMax = 21.2,
                Date = DateTime.Parse("1995-12-21 3:03:19 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1097,
                CountryId = "CA",
                Name = "Aurora",
                Region = new Region { Id = "ON", Name = "Ontario" },
                Population = 55445,
                Density = 1112.3,
                Tags = new[] { "ON", "CA" },
                AverageTemperatureMin = -11.5,
                AverageTemperatureMax = 19.8,
                Date = DateTime.Parse("2019-12-09 4:54:09 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1098,
                CountryId = "CA",
                Name = "Welland",
                Region = new Region { Id = "ON", Name = "Ontario" },
                Population = 52293,
                Density = 645.3,
                Tags = new[] { "ON", "CA" },
                AverageTemperatureMin = -4.3,
                AverageTemperatureMax = 21.6,
                Date = DateTime.Parse("1990-04-17 11:41:32 AM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1099,
                CountryId = "CA",
                Name = "North Bay",
                Region = new Region { Id = "ON", Name = "Ontario" },
                Population = 51553,
                Density = 161.6,
                Tags = new[] { "ON", "CA" },
                AverageTemperatureMin = -12.5,
                AverageTemperatureMax = 18.9,
                Date = DateTime.Parse("1985-01-28 4:25:49 PM", styles: DateTimeStyles.AssumeUniversal)
            },
            new City
            {
                Id = 1100,
                CountryId = "CA",
                Name = "Beloeil",
                Region = new Region { Id = "QC", Name = "Quebec" },
                Population = 50796,
                Density = 862.8,
                Tags = new[] { "QC", "CA" },
                AverageTemperatureMin = -9.7,
                AverageTemperatureMax = 21.2,
                Date = DateTime.Parse("2015-11-24 5:16:05 AM", styles: DateTimeStyles.AssumeUniversal)
            }
        };
}
