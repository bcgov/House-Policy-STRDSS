# Postgres

The Short Term Rental Data Sharing System (STRDSS) utilizes the bitnami/postgresql Helm chart, version 15.5.38.

However, the network policy for the database is not managed by the Helm chart and must be created manually. This is due to the following configuration:

```yaml
  networkPolicy:
    enabled: false
```

Instead, the network policies for the database are created through the application Helm chart maintained in the application's GitOps repository. These policies include:

* [Allow ingress from backend](https://github.com/bcgov-c/tenant-gitops-b0471a/blob/07f42166586b9a9d986045a027bb0f0fa0b4981f/charts/backend/templates/backend-networkpolicy.yaml#L126).
* [Allow ingress from hangfire](https://github.com/bcgov-c/tenant-gitops-b0471a/blob/07f42166586b9a9d986045a027bb0f0fa0b4981f/charts/hangfire/templates/hangfire-networkpolicy.yaml#L74).

## List Helm Chart Versions

```sh
helm search repo bitnami/postgresql --versions
helm show values bitnami/postgresql --version 15.5.38
```

## Get Image Version

```sh
helm show values bitnami/postgresql --version 15.5.38  | yq eval '.image' -
```

## Helm deploy

```sh
helm upgrade --install strdssdev bitnami/postgresql -f values-dev.yaml --version 15.5.38
```

## Database Setup

```sql
psql -U postgres
CREATE EXTENSION postgis;
CREATE ROLE strdssdev WITH LOGIN PASSWORD '[pwd]';
CREATE DATABASE strdssdev;
ALTER DATABASE strdssdev OWNER TO strdssdev;
\c strdssdev
CREATE EXTENSION postgis;
```
