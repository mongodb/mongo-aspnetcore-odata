#!/usr/bin/env bash
set -o errexit  # Exit the script with error if any of the commands fail

if [ -z "$PACKAGES_SOURCE" ]; then
  echo "PACKAGES_SOURCE variable should be set"
  exit 1
fi

if [ -z "$PACKAGES_SOURCE_KEY" ]; then
  echo "PACKAGES_SOURCE_KEY variable should be set"
  exit 1
fi

if [ -z "$PACKAGE_VERSION" ]; then
  echo "PACKAGE_VERSION variable should be set"
  exit 1
fi

clear_version_rx='^[0-9]+\.[0-9]+\.[0-9]+(-[a-zA-Z0-9]+)?$'
if [ "$PACKAGES_SOURCE" = "https://api.nuget.org/v3/index.json" ] && [[ ! "$PACKAGE_VERSION" =~ $clear_version_rx ]]; then
  echo "Cannot push dev version to nuget.org: '$PACKAGE_VERSION'"
  exit 1
fi

# querying nuget source to find search base url
packages_search_url=$(curl -X GET -s "${PACKAGES_SOURCE}" | jq -r 'first(.resources[] | select(."@type"=="SearchQueryService") | ."@id")')

wait_until_package_is_available ()
{
  package=$1
  version=$2
  resp=""
  count=0
  echo "Checking package availability: ${package}:${version} at ${packages_search_url}"
  while [ -z "$resp" ] && [ $count -le 40 ]; do
    resp=$(curl -X GET -s "$packages_search_url?prerelease=true&take=1&q=PackageId:$package" | jq --arg jq_version "$version" '.data[0].versions[] | select(.version==$jq_version) | .version')
    if [ -z "$resp" ]; then
      echo "sleeping for 15 seconds..."
      sleep 15
    fi
  done

  if [ -z "$resp" ]; then
    echo "Timeout while waiting for package availability: ${package}"
    exit 1
  else
    echo "Package ${package} is available, version: ${resp}"
  fi
}

dotnet nuget verify ./artifacts/nuget/MongoDB.AspNetCore.OData."$PACKAGE_VERSION".nupkg --certificate-fingerprint "$NUGET_SIGN_CERTIFICATE_FINGERPRINT"
echo Pushing nuget package...
dotnet nuget push ./artifacts/nuget/MongoDB.AspNetCore.OData."$PACKAGE_VERSION".nupkg -s "${PACKAGES_SOURCE}" -k "$PACKAGES_SOURCE_KEY"

wait_until_package_is_available "MongoDB.AspNetCore.OData" "$PACKAGE_VERSION"
