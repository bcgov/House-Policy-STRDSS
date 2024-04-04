#!/bin/sh

# this script is for populating nonce with the environment variable when starting the container

set -e

echo "hit entrypoint..."

cat >/nginx/nginx.conf <<EOL
# See https://developer.mozilla.org/en-US/docs/Web/HTTP/CSP
# See https://content-security-policy.com/nonce/

add_header content-security-policy "default-src 'self'; style-src 'self' 'unsafe-inline'; script-src 'self' ; font-src 'self'; frame-src loginproxy.gov.bc.ca dev.loginproxy.gov.bc.ca test.loginproxy.gov.bc.ca; connect-src 'self' loginproxy.gov.bc.ca dev.loginproxy.gov.bc.ca test.loginproxy.gov.bc.ca server.arcgisonline.com; img-src 'self' https://tile.openstreetmap.org data: server.arcgisonline.com www.w3.org; frame-ancestors https://loginproxy.gov.bc.ca https://dev.loginproxy.gov.bc.ca https://test.loginproxy.gov.bc.ca; object-src 'none'; base-uri 'self'; form-action 'self';";

location /__networkchecker {
    return 200 '{ "status": true }';
    add_header Content-Type application/json;
}
EOL

/docker-entrypoint.sh nginx -g 'daemon off;'
