# Backup Container

Short Term Rental Data Sharing System (STRDSS) is using an updated version of the official [BCDevOps backup-container](https://github.com/BCDevOps/backup-container) for backing up DB.

## Modifications from the official backup-container

### 1. [Dockerfile](./src/docker/Dockerfile)

**Original:**

```Dockerfile
FROM --platform=linux/amd64 quay.io/fedora/postgresql-15:15
```

**Updated:**

```Dockerfile
FROM --platform=linux/amd64 quay.io/fedora/postgresql-16:16
```

### 2. [backup.postgres.plugin](./src/docker/backup.postgres.plugin)

**Original:**

```sh
pg_dump -Fp -h "${_hostname}" ${_portArg} -U "${_username}" "${_database}" > "${BACKUP_DIR}backup.sql"
pg_dumpall -h "${_hostname}" ${_portArg} -U "${_username}" --roles-only --no-role-passwords > "${BACKUP_DIR}roles.sql"
```

**Updated:**

```sh
pg_dump -Fp -h "${_hostname}" ${_portArg} -U "${_username}" "${_database}" --clean > "${BACKUP_DIR}backup.sql"
pg_dumpall -h "${_hostname}" ${_portArg} -U "${_username}" --clean --roles-only --no-role-passwords > "${BACKUP_DIR}roles.sql"
```

## Docker build and push to the Artifactory

cd into [docker](./src/docker/)

```sh
docker build -t backup-container .
docker tag backup-container artifacts.developer.gov.bc.ca/sf4a-strdss/backup-container:16
docker login artifacts.developer.gov.bc.ca
docker push artifacts.developer.gov.bc.ca/sf4a-strdss/backup-container:16
```

## Helm deploy

cd into [docker](./helm/backup-storage/)

```sh
helm upgrade --install strdssdev-backup --values ../dev-values.yaml .
```

## One-time backup

```sh
./backup.sh -1
```

## Restore

### 1. Specific backup

```sh
./backup.sh -r strdssdev/strdssdev -f /backups/daily/2024-08-01/strdssdev-strdssdev_2024-08-01_09-13-51.sql.gz 
```

### 2. Latest backup

```sh
./backup.sh -r strdssdev/strdssdev 
```
