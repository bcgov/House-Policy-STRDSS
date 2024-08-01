# Postgres

Short Term Rental Data Sharing System (STRDSS) is using bitnami/postgresql with chart version 15.5.20

## Helm deploy

```sh
helm upgrade strdssdev bitnami/postgresql -f values-dev.yaml --version 15.5.20.
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
ALTER ROLE strdsstest with password '[pwd]';
```
