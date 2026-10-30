terraform {
  required_version = ">= 1.5.0"

  required_providers {
    sysdig = {
      source  = "sysdiglabs/sysdig"
      version = "~> 3.11"
    }
  }
}

# The API token still comes from SYSDIG_MONITOR_API_TOKEN.
# The URL is set here, without a trailing slash. The provider appends
# /api/v3/dashboards, and a trailing slash makes that //api/v3/dashboards.
# Sysdig redirects that to an HTML page, which Terraform cannot parse.
provider "sysdig" {
  sysdig_monitor_url = "https://app.sysdigcloud.com"
}
