# Backup Container

Short Term Rental Data Sharing System (STRDSS) is using an customized version of the official [BCDevOps backup-container](https://github.com/BCDevOps/backup-container) for backing up DB.

## The main changes from the official backup-container

### 1. [Dockerfile](./src/docker/Dockerfile)

Installed postgis extension

```docker
RUN dnf install -y curl util-linux postgis && \
    curl https://dl.min.io/client/mc/release/linux-amd64/mc -o /usr/bin/mc && \
    chmod +x /usr/bin/mc /usr/bin/go-crond && \
    dnf clean all && \
    rm -rf /var/cache/dnf && \
    echo $TZ > /etc/timezone
```

### 2. [backup.postgres.plugin](./src/docker/backup.postgres.plugin)

Excluded roles backup

```sh
cat "${BACKUP_DIR}backup.sql" | gzip > ${_backupFile}
rm "${BACKUP_DIR}backup.sql"
```

Set owner

```sh
  # SET OWNER
  if (( ${_rtnCd} == 0 )); then
    psql -h "${_hostname}" ${_portArg} -ac "ALTER DATABASE \"${_database}\" OWNER TO \"${_database}\";"
    _rtnCd=${?}
  fi
```

Added POSTGRESQL_EXTENSIONS for verification

```sh
# Start a local PostgreSql instance
POSTGRESQL_DATABASE=$(getDatabaseName "${_databaseSpec}") \
POSTGRESQL_USER=$(getDatabaseName "${_databaseSpec}") \
POSTGRESQL_PASSWORD=$(getPassword "${_databaseSpec}") \
POSTGRESQL_EXTENSIONS=postgis \
run-postgresql >/dev/null 2>&1 &
```

## Docker build and push to the Artifactory

cd into [docker](./src/docker/)

```sh
docker build -t backup-container .
docker tag backup-container artifacts.developer.gov.bc.ca/sf4a-strdss/backup-container:latest
docker login artifacts.developer.gov.bc.ca
docker push artifacts.developer.gov.bc.ca/sf4a-strdss/backup-container:latest
```

## Helm deploy

cd into [docker](./helm/backup-storage/)

```sh
helm upgrade --install strdssdev-backup --values ../dev-values.yaml .
```

## RocketChat Integration

A dedicated channel named [strdss-db-backup](https://chat.developer.gov.bc.ca/group/strdss-db-backup) has been established in RocketChat for backup notifications. The [rocketchat-script](./rocketchat-script.js) has been added to an incoming webhook integration for this channel.

The webhook URL is securely stored in the `strdssprod-backup-secrets` secret within the production namespace.

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
