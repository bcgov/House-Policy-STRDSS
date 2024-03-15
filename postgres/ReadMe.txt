oc login
oc project f4a30d-dev / test

helm install strdss-db-dev bitnami/postgresql -f values-dev.yaml 

helm install strdss-db-test bitnami/postgresql -f values-test.yaml

Enable PostGIS extension
https://github.com/bitnami/charts/issues/2830

Login
  psql -U postgres

Create Extension
  CREATE EXTENSION postgis;

Create Database
CREATE DATABASE "strdss-dev";

CREATE ROLE "strdss-dev" WITH LOGIN PASSWORD '';
GRANT ALL PRIVILEGES ON DATABASE "strdss-dev" TO "strdss-dev";


Create Secret

oc port-forward svc/strdss-dev 5434:5432