#!/usr/bin/env bash
set -o errexit  # Exit the script with error if any of the commands fail

MONGODB_URI=${MONGODB_URI:=mongodb://localhost:27017/}

ODATA_PROJECT_PATH="./src/MongoDB.AspNetCore.OData/MongoDB.AspNetCore.OData.csproj"
if [ -n "$DRIVER_VERSION" ]
then
  ## Update Driver's package reference if specified
  if [ "$DRIVER_VERSION" = "latest" ]
  then
    echo "Installing the latest version of MongoDB.Driver..."
    dotnet remove "$ODATA_PROJECT_PATH" package MongoDB.Driver
    dotnet add "$ODATA_PROJECT_PATH" package MongoDB.Driver
  elif [ -n "$DRIVER_VERSION" ]
  then
    echo "Installing the $DRIVER_VERSION version of MongoDB.Driver..."
    dotnet remove "$ODATA_PROJECT_PATH" package MongoDB.Driver
    dotnet add "$ODATA_PROJECT_PATH" package MongoDB.Driver -v "$DRIVER_VERSION"
  fi
fi

dotnet clean "./MongoDB.AspNetCore.OData.sln"
dotnet test "./MongoDB.AspNetCore.OData.sln" -e MONGODB__URI="${MONGODB_URI}" --results-directory ./build/test-results --logger "junit;verbosity=detailed;LogFileName=TEST_{assembly}.xml;FailureBodyFormat=Verbose" --logger "console;verbosity=detailed"
