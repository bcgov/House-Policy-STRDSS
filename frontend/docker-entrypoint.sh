#!/bin/sh
# Do not use set -e: sed -i fails on read-only FS, and verification must not crash the pod.

echo "hit entrypoint..."

api_url=${API_URL:-http://server:8080}
if ! sed -i "s~__API_URL__~$api_url~g" /etc/nginx/nginx.conf 2>/dev/null; then
  echo "WARN: could not configure the API proxy URL in /etc/nginx/nginx.conf." >&2
fi

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
      echo "WARN: could not patch $target (read-only?). Keycloak/env will be wrong until /nginx/html is writable." >&2
    fi
  done
}

# Nginx serves from /usr/share/nginx/html only in stock images; this app uses root /nginx/html (see nginx.conf).
# Do not patch /usr/share/nginx/html — it is often read-only on OpenShift and is not what clients get anyway.
echo "Substituting env placeholders in *.js under /nginx/html ..."
replace_placeholders_in_dir /nginx/html

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

# gzip_static serves these .gz files. Compress only after substitution so the
# precompressed bytes contain the real Keycloak/env values, not __SSO_HOST__.
# Files under this size shrink by about one packet, which does not change load time.
GZIP_MIN_BYTES=${GZIP_MIN_BYTES:-8192}

compress_static_assets() {
  html_root=$1
  compressed=0
  skipped=0
  unchanged=0
  list=/tmp/gzip-candidates

  # JS and CSS are at the web root. CSS-referenced fonts and PrimeIcons land in
  # media/; copied images land in assets/. A .gz that does not shrink is removed
  # so woff2 that is already compressed is not sent larger than the original.
  find "$html_root" -type f \( \
      -name '*.js' -o -name '*.css' -o -name '*.svg' -o -name '*.woff2' \
    \) > "$list" || {
    echo "WARN: could not list static files under $html_root." >&2
    return 0
  }

  while IFS= read -r target; do
    [ -n "$target" ] || continue
    [ -f "$target" ] || continue
    size=$(( $(wc -c < "$target") ))
    if [ "$size" -lt "$GZIP_MIN_BYTES" ]; then
      skipped=$((skipped + 1))
      continue
    fi
    if ! gzip -9 -k -f "$target"; then
      echo "WARN: gzip failed for $target; it will be served uncompressed." >&2
      continue
    fi
    gz_size=$(( $(wc -c < "$target.gz") ))
    if [ "$gz_size" -ge "$size" ]; then
      rm -f "$target.gz"
      unchanged=$((unchanged + 1))
      echo "gzip: $(basename "$target") did not shrink ($size bytes); serving uncompressed."
      continue
    fi
    # nginx recommends the .gz and the original share a modification time.
    touch -r "$target" "$target.gz"
    compressed=$((compressed + 1))
    echo "gzip: $(basename "$target") $size -> $gz_size bytes"
  done < "$list"

  rm -f "$list"
  echo "gzip: compressed $compressed file(s) at or above ${GZIP_MIN_BYTES} bytes, skipped $skipped smaller file(s), left $unchanged file(s) uncompressed because gzip did not shrink them."
}

if [ "$js_count" = 0 ]; then
  echo "ERROR: no *.js bundles under /nginx/html (wrong emptyDir/volume mount?). Nginx will start but the app will not load." >&2
elif [ "$placeholder_left" = 1 ]; then
  echo "ERROR: __SSO_HOST__ still present in JS after sed. Keycloak will not work. Fix: make /nginx/html writable at runtime (e.g. emptyDir volume) or substitute at build time." >&2
else
  echo "Env substitution OK: checked $js_count JS bundle(s), __SSO_HOST__ not present."
  echo "Precompressing JS, CSS, SVG, and WOFF2 at or above ${GZIP_MIN_BYTES} bytes ..."
  compress_static_assets /nginx/html
fi

cat >/nginx/nginx.conf <<EOL

location /__networkchecker {
    return 200 '{ "status": true }';
    add_header Content-Type application/json;
}
EOL

exec /docker-entrypoint.sh nginx -g 'daemon off;'
