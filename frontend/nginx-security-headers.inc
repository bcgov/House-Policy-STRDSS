# Included from location blocks that set Cache-Control (nginx clears parent add_header when child uses add_header).
# See https://nginx.org/en/docs/http/ngx_http_headers_module.html#add_header

add_header strict-transport-security 'max-age=15768000; includeSubDomains; preload' always;
add_header x-content-type-options "nosniff" always;
add_header x-frame-options "SAMEORIGIN" always;
add_header x-xss-protection "0" always;
add_header permissions-policy "geolocation=(self),midi=(),sync-xhr=(),microphone=(),camera=(),magnetometer=(),gyroscope=(),fullscreen=(self),payment=()" always;
add_header referrer-policy "strict-origin" always;
add_header x-dns-prefetch-control "off" always;
add_header content-security-policy "default-src 'self'; style-src 'self' 'nonce-$cspNonce' 'sha256-47DEQpj8HBSa+/TImW+5JCeuQeRkm5NMpJWZG3hSuFU=' 'sha256-biLFinpqYMtWHmXfkA1BPeCY0/fNt46SAZ+BBk5YUog=' 'sha256-YKfQryHKJ5Zcem6B61tAPvvVYJ8V1slH2rf9Wx9C/0Q=' 'sha256-+17AcPK/e5AtiK52Z2vnx3uG3BMzyzRr4Qv5UQsEbDU=' 'unsafe-hashes'; script-src 'self' 'nonce-$cspNonce' 'unsafe-eval'; font-src 'self'; frame-src 'self' https://loginproxy.gov.bc.ca https://dev.loginproxy.gov.bc.ca https://test.loginproxy.gov.bc.ca; connect-src 'self' ws: wss: https://loginproxy.gov.bc.ca https://dev.loginproxy.gov.bc.ca https://test.loginproxy.gov.bc.ca server.arcgisonline.com; img-src 'self' https://tile.openstreetmap.org data: server.arcgisonline.com www.w3.org; frame-ancestors https://loginproxy.gov.bc.ca https://dev.loginproxy.gov.bc.ca https://test.loginproxy.gov.bc.ca; object-src 'none'; base-uri 'self'; form-action 'self'; report-uri /csp-violation-report;" always;
