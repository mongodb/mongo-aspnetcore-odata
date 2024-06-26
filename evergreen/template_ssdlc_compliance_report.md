# ${PRODUCT_NAME} SSDLC compliance report

This report is available
<a href="https://us-west-2.console.aws.amazon.com/s3/object/csharp-driver-release-assets?region=us-west-2&bucketType=general&prefix=${PRODUCT_NAME}/${PACKAGE_VERSION}/ssdlc_compliance_report.md">here</a>.

<table>
  <tr>
    <th>Product name</th>
    <td><a href="https://github.com/mongodb/mongo-aspnetcore-odata">${PRODUCT_NAME}</a></td>
  </tr>
  <tr>
    <th>Product version</th>
    <td>${PACKAGE_VERSION}</td>
  </tr>
  <tr>
    <th>Report date, UTC</th>
    <td>${REPORT_DATE_UTC}</td>
  </tr>
</table>

## Release creator

This information is available in multiple ways:

<table>
  <tr>
    <th>Evergreen</th>
    <td>
        See the "Submitted by" field in <a href="https://spruce.mongodb.com/task/${task_id}">Evergreen release task</a>.
    </td>
  </tr>
   <tr>
    <th>Papertrail</th>
    <td>
        Refer to data in Papertrail. There is currently no official way to serve that data.
    </td>
  </tr>
</table>

## Process document

Blocked on <https://jira.mongodb.org/browse/DRIVERS-2892>.

The MongoDB SSDLC policy is available <a href="https://docs.google.com/document/d/1u0m4Kj2Ny30zU74KoEFCN4L6D_FbEYCaJ3CQdCYXTMc">here</a>.

## Third-darty dependency information

Our third party report is available <a href="https://us-west-2.console.aws.amazon.com/s3/object/csharp-driver-release-assets?region=us-west-2&bucketType=general&prefix=${PRODUCT_NAME}/${PACKAGE_VERSION}/augmented-sbom.json">here</a>.

## Static analysis findings

Coverity static analysis report is available <a href="https://us-west-2.console.aws.amazon.com/s3/object/csharp-driver-release-assets?region=us-west-2&bucketType=general&prefix=${PRODUCT_NAME}/${PACKAGE_VERSION}/static_code_analysis.csv">here</a>.

## Signature information

Packages are signed with certificate with fingerprint: ${NUGET_SIGN_CERTIFICATE_FINGERPRINT}.
Signature can be validated by running ```dotnet nuget verify``` command.

For example signature of ```MongoDB.AspNetCore.OData.${PACKAGE_VERSION}.nupkg``` package can be verified by running:
```
dotnet nuget verify MongoDB.AspNetCore.OData.${PACKAGE_VERSION}.nupkg --certificate-fingerprint ${NUGET_SIGN_CERTIFICATE_FINGERPRINT}
```
