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
1. In the namespace, authorization profile has been created as follows:
    * Flow: Client Credential Flow, using Client ID and Secret
    * Mode: Automatic
    * Client Mappers (Audience): gateway-awp


### Publication


1. Log into https://api.gov.bc.ca/
1. Select the strdata namespace
1. Create a service account with `GatewayConfig.Publish` scope and note down the client id and client secret
1. Download the GWA CLI from https://github.com/bcgov/gwa-cli/releases
1. In command prompt run the following commands (the first command create a .env file locally, which will need to be deleted if you need to create one for the other environment):

   ```sh
    gwa config set host api.gov.bc.ca
    gwa config set --namespace strdata

    export SCID="<<client id>>"
    export SCSC="<<client secret>>"
    export SURL="https://authz.apps.gov.bc.ca/auth/realms/aps/protocol/openid-connect/token"

    gwa login --client-id $SCID --client-secret $SCSC
    gwa pg strdata-{env}.yaml
   ```
1. (optional for Windows GWA) In command prompt of Windows run the following commands (the first command create a .env file locally, which will need to be deleted if you need to create one for the other environment):

   ```sh
    gwa config set host api.gov.bc.ca
    gwa config set --namespace strdata
    gwa login --client-id "<<client id>>" --client-secret "<<client secret>>"
    gwa pg strdata-{env}.yaml
   ```
1. Check the Gateway in the API Service Portal to make sure that the routes have been published
1. Create a dataset if it doesn't exist.

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

1. Create a product if it doesn't exist.

## Consumption

### Consumer Request & Approval

Refer to [Public Short-Term Rental Data API.pdf](./Public%20Short-Term%20Rental%20Data%20API.pdf):

### Calling API

1. Get a token with client ID and secret.

   ```sh
   export CLIENT_ID="your_client_id"
   export CLIENT_SECRET="your_secret"

   curl -X POST https://dev.loginproxy.gov.bc.ca/auth/realms/apigw/protocol/openid-connect/token \
   -H "Content-Type: application/x-www-form-urlencoded" \
   -H "Authorization: Basic $(echo -n $CLIENT_ID:$CLIENT_SECRET | base64)" \
   -H "Accept: application/json" \
   -H "Cache-Control: no-cache" \
   -H "Connection: keep-alive" \
   --data-urlencode "grant_type=client_credentials" \
   --compressed
   ```

1. Call API with the token.

   ```sh
   curl -X GET "https://strdata.dev.api.gov.bc.ca/api/organizations/strrequirements?longitude=-123.3709161&latitude=48.4177006" \
   -H "Authorization: Bearer your_token" \
   -H "Accept: application/json" \
   -H "Accept-Encoding: gzip, deflate, br" \
   -H "Connection: keep-alive" \
   --compressed
   ```
