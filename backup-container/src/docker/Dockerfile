# This image provides a postgres installation from which to run backups
FROM --platform=linux/amd64 quay.io/fedora/postgresql-15:15

# Change timezone to PST for convenience
ENV TZ=America/Los_Angeles

# Set the workdir to be root
WORKDIR /

# Load the backup scripts into the container (must be executable).
COPY backup.* /

# ========================================================================================================
# Install go-crond (from https://github.com/webdevops/go-crond)
#
# CRON Jobs in OpenShift:
#  - https://blog.danman.eu/cron-jobs-in-openshift/
# --------------------------------------------------------------------------------------------------------
ARG SOURCE_REPO=webdevops
ARG GOCROND_VERSION=23.2.0
ADD https://github.com/$SOURCE_REPO/go-crond/releases/download/$GOCROND_VERSION/go-crond.linux.amd64 /usr/bin/go-crond

USER root

RUN dnf install -y curl util-linux postgis && \
    curl https://dl.min.io/client/mc/release/linux-amd64/mc -o /usr/bin/mc && \
    chmod +x /usr/bin/mc /usr/bin/go-crond && \
    dnf clean all && \
    rm -rf /var/cache/dnf && \
    echo $TZ > /etc/timezone

# Important - Reset to the base image's user account.
USER 26

# Set the default CMD.
CMD ["bash", "/backup.sh"]
