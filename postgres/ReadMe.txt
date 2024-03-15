oc login
oc project f4a30d-dev / test

helm install strdssdev bitnami/postgresql -f values-dev.yaml 

helm install strdsstest bitnami/postgresql -f values-test.yaml

Enable PostGIS extension
https://github.com/bitnami/charts/issues/2830

Login
  psql -U postgres

Create Extension
  CREATE EXTENSION postgis;

Create Database
CREATE DATABASE strdssdev;

CREATE ROLE strdssdev WITH LOGIN PASSWORD '';


GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO strdssdev;

ALTER DEFAULT PRIVILEGES FOR ROLE postgres GRANT ALL ON TABLES TO strdssdev ;
ALTER DEFAULT PRIVILEGES FOR ROLE postgres GRANT ALL  ON SEQUENCES TO strdssdev ;
ALTER DEFAULT PRIVILEGES FOR ROLE postgres GRANT ALL  ON FUNCTIONS TO strdssdev ;
ALTER DEFAULT PRIVILEGES FOR ROLE postgres GRANT ALL  ON TYPES TO strdssdev ;
ALTER DEFAULT PRIVILEGES FOR ROLE postgres GRANT ALL  ON SCHEMAS TO strdssdev ;

Create Secret

oc port-forward svc/strdssdev 5433:5432