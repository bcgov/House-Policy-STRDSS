#!/bin/sh
# Do not use set -e: sed -i fails on read-only FS, and verification must not crash the pod.

echo "hit entrypoint..."

replace_placeholders_in_dir() {
  html_root=$1
  [ -d "$html_root" ] || return 0

  for target in "$html_root"/*.js; do
    [ -f "$target" ] || continue
    if ! sed -i \
      -e "s~__SSO_HOST__~$SSO_HOST~g" \
      -e "s~__ENV_NAME__~$ENV_NAME~g" \
      -e "s~__SM_LOGOFF_URL__~$SM_LOGOFF_URL~g" \
      -e "s~__RENTAL_LISTING_REPORT_MAX_SIZE__~$RENTAL_LISTING_REPORT_MAX_SIZE~g" \
      -e "s~__BUISINESS_LICENCE_MAX_SIZE__~$BUISINESS_LICENCE_MAX_SIZE~g" \
      -e "s~__ADDRESS_SCORE__~$ADDRESS_SCORE~g" \
      -e "s~__VALIDATE_REGISTRATION_MAX_SIZE__~$VALIDATE_REGISTRATION_MAX_SIZE~g" \
      "$target" 2>/dev/null
    then
      echo "WARN: could not patch $target (read-only filesystem?). Keycloak/env may be broken until /nginx/html is writable." >&2
    fi
  done
}

echo "Substituting env placeholders in *.js under /nginx/html and /usr/share/nginx/html ..."
replace_placeholders_in_dir /nginx/html
replace_placeholders_in_dir /usr/share/nginx/html

js_count=0
placeholder_left=0
for target in /nginx/html/*.js; do
  [ -f "$target" ] || continue
  js_count=$((js_count + 1))
  if grep -q '__SSO_HOST__' "$target" 2>/dev/null; then
    placeholder_left=1
    break
  fi
done

if [ "$js_count" = 0 ]; then
  echo "ERROR: no *.js bundles under /nginx/html (wrong emptyDir/volume mount?). Nginx will start but the app will not load." >&2
elif [ "$placeholder_left" = 1 ]; then
  echo "ERROR: __SSO_HOST__ still present in JS after sed. Keycloak will not work. Fix: make /nginx/html writable at runtime (e.g. emptyDir volume) or substitute at build time." >&2
else
  echo "Env substitution OK: checked $js_count JS bundle(s), __SSO_HOST__ not present."
fi

cat >/nginx/nginx.conf <<EOL

location /__networkchecker {
    return 200 '{ "status": true }';
    add_header Content-Type application/json;
}
EOL

exec /docker-entrypoint.sh nginx -g 'daemon off;'
