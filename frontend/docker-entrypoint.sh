#!/bin/sh

set -e

echo "hit entrypoint..."

mkdir -p /nginx/html
cp -R /usr/share/nginx/html/* /nginx/html

target=/nginx/html/main.js
sed -i "s~__SSO_HOST__~$SSO_HOST~g" "$target"
sed -i "s~__ENV_NAME__~$ENV_NAME~g" "$target"
sed -i "s~__SM_LOGOFF_URL__~$SM_LOGOFF_URL~g" "$target"

cat >/nginx/nginx.conf <<EOL

location /__networkchecker {
    return 200 '{ "status": true }';
    add_header Content-Type application/json;
}
EOL

/docker-entrypoint.sh nginx -g 'daemon off;'
