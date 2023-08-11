#!/usr/bin/env bash
set -o errexit  # Exit the script with error if any of the commands fail

MONGODB_URI=${MONGODB_URI:=mongodb://localhost:27017/}
if [ -z "$PACKAGE_VERSION" ]
then
  PACKAGE_VERSION=$(git describe --tags)
  echo Calculated PACKAGE_VERSION value: "$PACKAGE_VERSION"
fi

ODATA_PROJECT="./src/MongoDB.AspNetCore.OData/MongoDB.AspNetCore.OData.csproj"
ODATA_SAMPLE_PROJECT="./tests/MongoDB.AspNetCore.OData.Sample.WebApi/MongoDB.AspNetCore.OData.Sample.WebApi.csproj"
ODATA_TESTS_PROJECT="./tests/MongoDB.AspNetCore.OData.Tests/MongoDB.AspNetCore.OData.Tests.csproj"

echo Retargeting API tests to use generated package instead of project dependency...
dotnet clean "./MongoDB.AspNetCore.OData.sln"

dotnet nuget add source "./build/nuget" -n local --configfile "./nuget.config"
dotnet nuget locals temp -c
dotnet remove "$ODATA_SAMPLE_PROJECT" reference "$ODATA_PROJECT"
dotnet add "$ODATA_SAMPLE_PROJECT" package "MongoDB.AspNetCore.OData" -v "$PACKAGE_VERSION"

dotnet test "$ODATA_TESTS_PROJECT" -e MONGODB__URI="${MONGODB_URI}" --results-directory "./build/test-results" --logger "junit;verbosity=detailed;LogFileName=TEST_{assembly}.xml;FailureBodyFormat=Verbose"

echo Revert the changes.
dotnet nuget remove source local --configfile "./nuget.config"
dotnet remove "$ODATA_SAMPLE_PROJECT" package "MongoDB.AspNetCore.OData"
dotnet add "$ODATA_SAMPLE_PROJECT" reference "$ODATA_PROJECT"

