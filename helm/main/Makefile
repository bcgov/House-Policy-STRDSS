SHELL := /usr/bin/env bash
RANDOM_STRING := $(shell LC_ALL=C tr -dc 'A-Za-z0-9' </dev/urandom | head -c 48)
NAME := strdss

NAMESPACE=
ENV_NAME=
IMAGE_TAG_FRONTEND=
IMAGE_TAG_BACKEND=

ifndef NAMESPACE
$(error NAMESPACE is not set)
endif

ifndef ENV_NAME
$(error ENV_NAME is not set)
endif

ifndef IMAGE_TAG_FRONTEND
$(error IMAGE_TAG_FRONTEND is not set)
endif

ifndef IMAGE_TAG_BACKEND
$(error IMAGE_TAG_BACKEND is not set)
endif

define arguments
	"${NAME}" . -n "${NAMESPACE}" -f values.yaml -f "values-${ENV_NAME}.yaml" \
	--set frontend.image.tag="${IMAGE_TAG_FRONTEND}" \
	--set backend.image.tag="${IMAGE_TAG_BACKEND}" \
	--set backend.env.InternalTokenSecret="${RANDOM_STRING}"
endef

.PHONY: helm-dep
helm-dep:
	helm dependency update

.PHONY: install
install: helm-dep
install:
	@helm install $(call arguments)

.PHONY: upgrade
upgrade: helm-dep
upgrade:
	@helm upgrade --install $(call arguments)

.PHONY: lint
lint: helm-dep
lint:
	@helm upgrade --dry-run --install $(call arguments)

.PHONY: uninstall
uninstall: helm-dep
uninstall:
	@helm uninstall ${NAME} -n ${NAMESPACE}

.PHONY: template
template: helm-dep
template:
	@helm template $(call arguments) > template.yaml

.PHONY: force-install
force-install: uninstall
force-install: install
