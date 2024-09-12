# KONG API Service Portal Setup

The public API is accessible at

* DEV: https://dev.strdata.api.gov.bc.ca
* UAT: https://test.strdata.api.gov.bc.ca
* PROD: https://strdata.api.gov.bc.ca

API access is controlled via Kong, administered via the BC Gov API Programme Services API Gateway.
**Kong configuration is not updated via Github Actions, and must be updated manually when there are changes.**

For an overview of the API Gateway update process, see:
https://bcgov.github.io/aps-infra-platform/guides/owner-journey-v1/


## Publication

### Prerequisites
1. In the API Services Portal (https://api.gov.bc.ca/), the namespace strdata has already been created.
2. In the namespace, authorization profile has been created as follows:
    * Flow: Client Credential Flow, using Client ID and Secret
    * Mode: Automatic
    * Client Mappers (Audience): gateway-awp


### Publication


1. Log into https://api.gov.bc.ca/
2. Select the strdata namespace
3. Create a service account with `GatewayConfig.Publish` scope and note down the client id and client secret
4. Download the GWA CLI from https://github.com/bcgov/gwa-cli/releases
5. In command prompt run the following commands (the first command create a .env file locally, which will need to be deleted if you need to create one for the other environment):

   ```sh
    gwa config set host api.gov.bc.ca
    gwa config set --namespace strdata

    export SCID="<<client id>>"
    export SCSC="<<client secret>>"
    export SURL="https://authz.apps.gov.bc.ca/auth/realms/aps/protocol/openid-connect/token"

    gwa login --client-id $SCID --client-secret $SCSC
    gwa pg strdata-{env}.yaml
   ```
5. (optional for Windows GWA) In command prompt of Windows run the following commands (the first command create a .env file locally, which will need to be deleted if you need to create one for the other environment):

   ```sh
    gwa config set host api.gov.bc.ca
    gwa config set --namespace strdata
    gwa login --client-id "<<client id>>" --client-secret "<<client secret>>"
    gwa pg strdata-{env}.yaml
   ```
6. Check the Gateway in the API Service Portal to make sure that the routes have been published
7. Create a dataset if it doesn't exist.

   https://bcgov.github.io/aps-infra-platform/guides/owner-journey-v1/#91-setup-your-draft-dataset

   ```
   {
      "name": "strdata-dataset",
      "license_title": "Open Government Licence - British Columbia",
      "security_class": "PUBLIC",
      "view_audience": "Public",
      "download_audience": "Public",
      "record_publish_date": "2024-09-11",
      "notes": "Short-Term Rental Data API Services",
      "title": "Short-Term Rental Data API Services",
      "tags": [
         "openapi",
         "standards"
      ],
      "organization": "ministry-of-housing",
      "organizationUnit": "planning-and-land-use-management"
   }
   ```

8. Create a product if it doesn't exist.

### Consumer Request & Approval

