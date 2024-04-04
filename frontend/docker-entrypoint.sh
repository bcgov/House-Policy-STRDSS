#!/bin/sh

# this script is for populating nonce with the environment variable when starting the container

set -e

echo "hit entrypoint..."

cat >/nginx/nginx.conf <<EOL

location /__networkchecker {
    return 200 '{ "status": true }';
    add_header Content-Type application/json;
}
EOL

/docker-entrypoint.sh nginx -g 'daemon off;'
