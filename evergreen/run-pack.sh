#!/usr/bin/env bash
set -o errexit  # Exit the script with error if any of the commands fail

if [ -z "$PACKAGE_VERSION" ]
then
  PACKAGE_VERSION=$(git describe --tags)
  echo Calculated PACKAGE_VERSION value: "$PACKAGE_VERSION"
fi

echo Creating nuget package...

dotnet clean ./MongoDB.AspNetCore.OData.sln
dotnet pack ./MongoDB.AspNetCore.OData.sln -o ./artifacts/nuget -c Release -p:Version="$PACKAGE_VERSION" --include-symbols -p:SymbolPackageFormat=snupkg -p:ContinuousIntegrationBuild=true
