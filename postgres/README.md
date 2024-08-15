# Postgres

Short Term Rental Data Sharing System (STRDSS) is using bitnami/postgresql with chart version 12.12.10

## List Helm Chart Versions

```sh
helm search repo bitnami/postgresql --versions
helm show values bitnami/postgresql --version 12.12.10
```

## Get Image Version

```sh
helm show values bitnami/postgresql --version 12.12.10  | yq eval '.image' -
```

## Helm deploy

```sh
helm upgrade --install strdssdev bitnami/postgresql -f values-dev.yaml --version 12.12.10
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
