FROM node:18.16.0-slim as builder-app

ARG generate_sourcemap=false
WORKDIR /app

COPY package.json package-lock.json ./
RUN npm install

COPY . .

RUN npm run build --prod

# Inject style nonce
# ARG style_nonce
# RUN sed -i -e "s|<script|<script nonce=\"${style_nonce}\"|g" -e "s|<link|<link nonce=\"${style_nonce}\"|g" -e "s|<style|<style nonce=\"${style_nonce}\"|g" /app/dist/frontend/browser/index.html

FROM nginx:stable-alpine as builder-modules

# Ensure that the version of nginx of the builder image is compatible with the nginx version of the main image.
ENV NGINX_VERSION 1.24.0

ENV MORE_HEADERS_VERSION=0.34
ENV MORE_HEADERS_GITREPO=openresty/headers-more-nginx-module

RUN wget "http://nginx.org/download/nginx-${NGINX_VERSION}.tar.gz" -O nginx.tar.gz && \
  wget "https://github.com/${MORE_HEADERS_GITREPO}/archive/v${MORE_HEADERS_VERSION}.tar.gz" -O more_headers.tar.gz

RUN apk add --no-cache --virtual .build-deps \
  gcc \
  gd-dev \
  geoip-dev \
  libc-dev \
  libxslt-dev \
  linux-headers \
  make \
  openssl-dev \
  pcre-dev \
  zlib-dev

ENV DOWNLOADS /tmp/downloads
ENV HEADERS_MORE_MODULE "${DOWNLOADS}/headers-more-nginx-module-${MORE_HEADERS_VERSION}"

RUN mkdir -p ${DOWNLOADS} && \
  CONFARGS=$(nginx -V 2>&1 | sed -n -e 's/^.*arguments: //p') \
  tar -zxC ${DOWNLOADS} -f nginx.tar.gz && \
  tar -zxC ${DOWNLOADS} -f more_headers.tar.gz && \
  cd ${DOWNLOADS}/nginx-$NGINX_VERSION && \
  ./configure --with-compat $CONFARGS --add-dynamic-module=${HEADERS_MORE_MODULE} && \
  make && make install

FROM nginx:stable-alpine3.17-slim

ENV NGINX_DIR /usr/share/nginx

COPY --from=builder-modules /usr/local/nginx/modules/ngx_http_headers_more_filter_module.so ${NGINX_DIR}/modules/ngx_http_headers_more_filter_module.so
COPY --from=builder-app /app/dist/frontend/browser ${NGINX_DIR}/html
COPY nginx.conf /etc/nginx/nginx.conf

COPY ./docker-entrypoint.sh /_docker-entrypoint.sh
RUN chmod +x /_docker-entrypoint.sh

RUN mkdir -p /nginx/html && chmod -R a+rw /nginx
RUN cp -R /usr/share/nginx/html/* /nginx/html

EXPOSE 3001
ENV TZ America/Vancouver

ENTRYPOINT ["/_docker-entrypoint.sh"]
