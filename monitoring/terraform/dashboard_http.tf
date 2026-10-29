resource "sysdig_monitor_dashboard" "http" {
  name        = "STRDSS HTTP"
  description = "Request rate, errors, and latency for the STRDSS API and Hangfire. Choose a namespace, then a container. Request charts exclude /healthz and /metrics."

  scope {
    metric     = "kubernetes.namespace.name"
    comparator = "equals"
    value      = ["b0471a-dev"]
    variable   = "namespace"
  }

  scope {
    metric     = "container.name"
    comparator = "equals"
    value      = ["strdss-dev-backend"]
    variable   = "container"
  }

  panel {
    pos_x                  = 0
    pos_y                  = 0
    width                  = 24
    height                 = 4
    type                   = "text"
    name                   = "How to read this dashboard"
    visible_title          = true
    autosize_text          = false
    transparent_background = false
    content                = "Select a namespace, then a container in that namespace. Dev and test containers are both in b0471a-dev. UAT containers are in b0471a-test, and prod containers are in b0471a-prod. These charts are emitted by the backend and hangfire containers. Inbound charts omit /healthz and /metrics. Outbound charts are HttpClient calls (CHES, geocoder, registration API). The BCEID SOAP client does not appear there. Hangfire's useful signal is on the STRDSS data access dashboard."
  }

  panel {
    pos_x       = 0
    pos_y       = 4
    width       = 6
    height      = 4
    type        = "number"
    name        = "Request rate"
    description = "Application requests per second, excluding health checks and the metrics scrape."

    query {
      promql = "sum(rate(http_server_request_duration_seconds_count{${local.http_server}}[$__interval]))"
      unit   = "number rate"

      display_info {
        display_name                      = "requests"
        time_series_display_name_template = "requests/s"
        type                              = "lines"
      }

      format {
        decimals                = 2
        display_format          = "auto"
        input_format            = "/s"
        null_value_display_mode = "nullGap"
        y_axis                  = "auto"
      }
    }
  }

  panel {
    pos_x       = 6
    pos_y       = 4
    width       = 6
    height      = 4
    type        = "number"
    name        = "5xx percent"
    description = "Share of application requests with an HTTP 5xx status."

    query {
      promql = "100 * ((sum(rate(http_server_request_duration_seconds_count{${local.http_server},http_response_status_code=~\"5..\"}[$__interval])) or vector(0)) / sum(rate(http_server_request_duration_seconds_count{${local.http_server}}[$__interval])))"
      unit   = "percent"

      display_info {
        display_name                      = "5xx"
        time_series_display_name_template = "5xx %"
        type                              = "lines"
      }

      format {
        decimals                = 2
        display_format          = "auto"
        input_format            = "0-100"
        null_value_display_mode = "nullGap"
        y_axis                  = "auto"
      }
    }
  }

  panel {
    pos_x       = 12
    pos_y       = 4
    width       = 6
    height      = 4
    type        = "number"
    name        = "p95 latency"
    description = "95th percentile application request duration. Measured at the ASP.NET Core host, including the time to send the response."

    query {
      promql = "histogram_quantile(0.95, sum by (le) (rate(http_server_request_duration_seconds_bucket{${local.http_server}}[$__interval])))"
      unit   = "time"

      display_info {
        display_name                      = "p95"
        time_series_display_name_template = "p95"
        type                              = "lines"
      }

      format {
        decimals                = 2
        display_format          = "auto"
        input_format            = "s"
        null_value_display_mode = "nullGap"
        y_axis                  = "auto"
      }
    }
  }

  panel {
    pos_x       = 18
    pos_y       = 4
    width       = 6
    height      = 4
    type        = "number"
    name        = "Active requests"
    description = "Requests currently being processed, including health checks that are in flight."

    query {
      promql = "sum(http_server_active_requests{${local.scope}})"
      unit   = "number"

      display_info {
        display_name                      = "active"
        time_series_display_name_template = "active"
        type                              = "lines"
      }

      format {
        decimals                = 0
        display_format          = "auto"
        input_format            = "1"
        null_value_display_mode = "nullGap"
        y_axis                  = "auto"
      }
    }
  }

  panel {
    pos_x       = 0
    pos_y       = 8
    width       = 12
    height      = 8
    type        = "timechart"
    name        = "Request rate by status"
    description = "Application requests per second, stacked by HTTP status code."

    legend {
      enabled      = true
      layout       = "table"
      position     = "bottom"
      show_current = true
    }

    query {
      promql = "sum by (http_response_status_code) (rate(http_server_request_duration_seconds_count{${local.http_server}}[$__interval]))"
      unit   = "number rate"

      display_info {
        display_name                      = "status"
        time_series_display_name_template = "{{http_response_status_code}}"
        type                              = "stackedArea"
      }

      format {
        decimals                = 2
        display_format          = "auto"
        input_format            = "/s"
        null_value_display_mode = "nullGap"
        y_axis                  = "auto"
      }
    }
  }

  panel {
    pos_x       = 12
    pos_y       = 8
    width       = 12
    height      = 8
    type        = "timechart"
    name        = "Busiest routes"
    description = "Ten routes with the highest request rate."

    legend {
      enabled      = true
      layout       = "table"
      position     = "bottom"
      show_current = true
    }

    query {
      promql = "topk(10, sum by (http_request_method, http_route) (rate(http_server_request_duration_seconds_count{${local.http_server}}[$__interval])))"
      unit   = "number rate"

      display_info {
        display_name                      = "route"
        time_series_display_name_template = "{{http_request_method}} {{http_route}}"
        type                              = "lines"
      }

      format {
        decimals                = 2
        display_format          = "auto"
        input_format            = "/s"
        null_value_display_mode = "nullGap"
        y_axis                  = "auto"
      }
    }
  }

  panel {
    pos_x       = 0
    pos_y       = 16
    width       = 12
    height      = 8
    type        = "timechart"
    name        = "Request latency"
    description = "Overall application request latency. A gap between p50 and p99 means a minority of routes are slow."

    legend {
      enabled      = true
      layout       = "table"
      position     = "bottom"
      show_current = true
    }

    query {
      promql = "histogram_quantile(0.50, sum by (le) (rate(http_server_request_duration_seconds_bucket{${local.http_server}}[$__interval])))"
      unit   = "time"

      display_info {
        display_name                      = "p50"
        time_series_display_name_template = "p50"
        type                              = "lines"
      }

      format {
        decimals                = 2
        display_format          = "auto"
        input_format            = "s"
        null_value_display_mode = "nullGap"
        y_axis                  = "auto"
      }
    }

    query {
      promql = "histogram_quantile(0.95, sum by (le) (rate(http_server_request_duration_seconds_bucket{${local.http_server}}[$__interval])))"
      unit   = "time"

      display_info {
        display_name                      = "p95"
        time_series_display_name_template = "p95"
        type                              = "lines"
      }

      format {
        decimals                = 2
        display_format          = "auto"
        input_format            = "s"
        null_value_display_mode = "nullGap"
        y_axis                  = "auto"
      }
    }

    query {
      promql = "histogram_quantile(0.99, sum by (le) (rate(http_server_request_duration_seconds_bucket{${local.http_server}}[$__interval])))"
      unit   = "time"

      display_info {
        display_name                      = "p99"
        time_series_display_name_template = "p99"
        type                              = "lines"
      }

      format {
        decimals                = 2
        display_format          = "auto"
        input_format            = "s"
        null_value_display_mode = "nullGap"
        y_axis                  = "auto"
      }
    }
  }

  panel {
    pos_x       = 12
    pos_y       = 16
    width       = 12
    height      = 8
    type        = "timechart"
    name        = "5xx by route"
    description = "Server error rate by method and route. ExceptionMiddleware turns unhandled exceptions into these responses."

    legend {
      enabled      = true
      layout       = "table"
      position     = "bottom"
      show_current = true
    }

    query {
      promql = "sum by (http_request_method, http_route) (rate(http_server_request_duration_seconds_count{${local.http_server},http_response_status_code=~\"5..\"}[$__interval]))"
      unit   = "number rate"

      display_info {
        display_name                      = "5xx"
        time_series_display_name_template = "{{http_request_method}} {{http_route}}"
        type                              = "stackedArea"
      }

      format {
        decimals                = 2
        display_format          = "auto"
        input_format            = "/s"
        null_value_display_mode = "nullGap"
        y_axis                  = "auto"
      }
    }
  }

  panel {
    pos_x       = 0
    pos_y       = 24
    width       = 12
    height      = 8
    type        = "timechart"
    name        = "4xx by route"
    description = "Client error rate by method and route."

    legend {
      enabled      = true
      layout       = "table"
      position     = "bottom"
      show_current = true
    }

    query {
      promql = "sum by (http_request_method, http_route) (rate(http_server_request_duration_seconds_count{${local.http_server},http_response_status_code=~\"4..\"}[$__interval]))"
      unit   = "number rate"

      display_info {
        display_name                      = "4xx"
        time_series_display_name_template = "{{http_request_method}} {{http_route}}"
        type                              = "stackedArea"
      }

      format {
        decimals                = 2
        display_format          = "auto"
        input_format            = "/s"
        null_value_display_mode = "nullGap"
        y_axis                  = "auto"
      }
    }
  }

  panel {
    pos_x       = 12
    pos_y       = 24
    width       = 12
    height      = 8
    type        = "timechart"
    name        = "Slowest routes p95"
    description = "Ten slowest method and route combinations at p95."

    legend {
      enabled      = true
      layout       = "table"
      position     = "bottom"
      show_current = true
    }

    query {
      promql = "topk(10, histogram_quantile(0.95, sum by (le, http_request_method, http_route) (rate(http_server_request_duration_seconds_bucket{${local.http_server}}[$__interval]))))"
      unit   = "time"

      display_info {
        display_name                      = "p95"
        time_series_display_name_template = "{{http_request_method}} {{http_route}}"
        type                              = "lines"
      }

      format {
        decimals                = 2
        display_format          = "auto"
        input_format            = "s"
        null_value_display_mode = "nullGap"
        y_axis                  = "auto"
      }
    }
  }

  panel {
    pos_x       = 0
    pos_y       = 32
    width       = 12
    height      = 8
    type        = "timechart"
    name        = "Active requests by method"
    description = "In-flight HTTP requests. Includes probes."

    legend {
      enabled      = true
      layout       = "table"
      position     = "bottom"
      show_current = true
    }

    query {
      promql = "sum by (http_request_method) (http_server_active_requests{${local.scope}})"
      unit   = "number"

      display_info {
        display_name                      = "active"
        time_series_display_name_template = "{{http_request_method}}"
        type                              = "stackedArea"
      }

      format {
        decimals                = 0
        display_format          = "auto"
        input_format            = "1"
        null_value_display_mode = "nullGap"
        y_axis                  = "auto"
      }
    }
  }

  panel {
    pos_x       = 12
    pos_y       = 32
    width       = 12
    height      = 8
    type        = "timechart"
    name        = "Kestrel connections"
    description = "Accepted connections and connections waiting to be accepted. A rising queue means the server is not accepting work as fast as it arrives."

    legend {
      enabled      = true
      layout       = "table"
      position     = "bottom"
      show_current = true
    }

    query {
      promql = "sum(kestrel_active_connections{${local.scope}})"
      unit   = "number"

      display_info {
        display_name                      = "active"
        time_series_display_name_template = "active"
        type                              = "lines"
      }

      format {
        decimals                = 0
        display_format          = "auto"
        input_format            = "1"
        null_value_display_mode = "nullGap"
        y_axis                  = "auto"
      }
    }

    query {
      promql = "sum(kestrel_queued_connections{${local.scope}})"
      unit   = "number"

      display_info {
        display_name                      = "queued"
        time_series_display_name_template = "queued"
        type                              = "lines"
      }

      format {
        decimals                = 0
        display_format          = "auto"
        input_format            = "1"
        null_value_display_mode = "nullGap"
        y_axis                  = "auto"
      }
    }
  }

  panel {
    pos_x       = 0
    pos_y       = 40
    width       = 12
    height      = 8
    type        = "timechart"
    name        = "Rejected connections"
    description = "Connections Kestrel refused. Any sustained rate means the connection limit is being hit."

    legend {
      enabled      = true
      layout       = "table"
      position     = "bottom"
      show_current = true
    }

    query {
      promql = "sum(rate(kestrel_rejected_connections_total{${local.scope}}[$__interval]))"
      unit   = "number rate"

      display_info {
        display_name                      = "rejected"
        time_series_display_name_template = "rejected/s"
        type                              = "lines"
      }

      format {
        decimals                = 3
        display_format          = "auto"
        input_format            = "/s"
        null_value_display_mode = "nullGap"
        y_axis                  = "auto"
      }
    }
  }

  panel {
    pos_x       = 12
    pos_y       = 40
    width       = 12
    height      = 8
    type        = "timechart"
    name        = "Connection duration p95"
    description = "How long accepted connections stay open. Keep-alive makes this much longer than a single request."

    legend {
      enabled      = true
      layout       = "table"
      position     = "bottom"
      show_current = true
    }

    query {
      promql = "histogram_quantile(0.95, sum by (le) (rate(kestrel_connection_duration_seconds_bucket{${local.scope}}[$__interval])))"
      unit   = "time"

      display_info {
        display_name                      = "p95"
        time_series_display_name_template = "p95"
        type                              = "lines"
      }

      format {
        decimals                = 2
        display_format          = "auto"
        input_format            = "s"
        null_value_display_mode = "nullGap"
        y_axis                  = "auto"
      }
    }
  }

  panel {
    pos_x                  = 0
    pos_y                  = 48
    width                  = 24
    height                 = 3
    type                   = "text"
    name                   = "Outbound HTTP"
    visible_title          = true
    autosize_text          = false
    transparent_background = false
    content                = "HttpClient calls grouped by server.address. Duration ends when response headers are received and does not include reading the body. CHES, the geocoder, and the registration API are the expected destinations."
  }

  panel {
    pos_x       = 0
    pos_y       = 51
    width       = 6
    height      = 4
    type        = "number"
    name        = "Outbound request rate"
    description = "HttpClient requests per second."

    query {
      promql = "sum(rate(http_client_request_duration_seconds_count{${local.scope}}[$__interval]))"
      unit   = "number rate"

      display_info {
        display_name                      = "requests"
        time_series_display_name_template = "requests/s"
        type                              = "lines"
      }

      format {
        decimals                = 2
        display_format          = "auto"
        input_format            = "/s"
        null_value_display_mode = "nullGap"
        y_axis                  = "auto"
      }
    }
  }

  panel {
    pos_x       = 6
    pos_y       = 51
    width       = 6
    height      = 4
    type        = "number"
    name        = "Outbound 5xx percent"
    description = "Share of HttpClient calls that received an HTTP 5xx response."

    query {
      promql = "100 * ((sum(rate(http_client_request_duration_seconds_count{${local.scope},http_response_status_code=~\"5..\"}[$__interval])) or vector(0)) / sum(rate(http_client_request_duration_seconds_count{${local.scope}}[$__interval])))"
      unit   = "percent"

      display_info {
        display_name                      = "5xx"
        time_series_display_name_template = "5xx %"
        type                              = "lines"
      }

      format {
        decimals                = 2
        display_format          = "auto"
        input_format            = "0-100"
        null_value_display_mode = "nullGap"
        y_axis                  = "auto"
      }
    }
  }

  panel {
    pos_x       = 12
    pos_y       = 51
    width       = 6
    height      = 4
    type        = "number"
    name        = "Outbound p95"
    description = "95th percentile HttpClient duration, through response headers."

    query {
      promql = "histogram_quantile(0.95, sum by (le) (rate(http_client_request_duration_seconds_bucket{${local.scope}}[$__interval])))"
      unit   = "time"

      display_info {
        display_name                      = "p95"
        time_series_display_name_template = "p95"
        type                              = "lines"
      }

      format {
        decimals                = 2
        display_format          = "auto"
        input_format            = "s"
        null_value_display_mode = "nullGap"
        y_axis                  = "auto"
      }
    }
  }

  panel {
    pos_x       = 18
    pos_y       = 51
    width       = 6
    height      = 4
    type        = "number"
    name        = "Open outbound connections"
    description = "HTTP connections currently open to remote servers."

    query {
      promql = "sum(http_client_open_connections{${local.scope}})"
      unit   = "number"

      display_info {
        display_name                      = "open"
        time_series_display_name_template = "open"
        type                              = "lines"
      }

      format {
        decimals                = 0
        display_format          = "auto"
        input_format            = "1"
        null_value_display_mode = "nullGap"
        y_axis                  = "auto"
      }
    }
  }

  panel {
    pos_x       = 0
    pos_y       = 55
    width       = 12
    height      = 8
    type        = "timechart"
    name        = "Outbound p95 by destination"
    description = "95th percentile HttpClient duration for each server.address."

    legend {
      enabled      = true
      layout       = "table"
      position     = "bottom"
      show_current = true
    }

    query {
      promql = "histogram_quantile(0.95, sum by (le, server_address) (rate(http_client_request_duration_seconds_bucket{${local.scope}}[$__interval])))"
      unit   = "time"

      display_info {
        display_name                      = "p95"
        time_series_display_name_template = "{{server_address}}"
        type                              = "lines"
      }

      format {
        decimals                = 2
        display_format          = "auto"
        input_format            = "s"
        null_value_display_mode = "nullGap"
        y_axis                  = "auto"
      }
    }
  }

  panel {
    pos_x       = 12
    pos_y       = 55
    width       = 12
    height      = 8
    type        = "timechart"
    name        = "Outbound errors by destination"
    description = "HttpClient calls that returned HTTP 4xx or 5xx. Connection failures that never receive a status still show up in the latency chart."

    legend {
      enabled      = true
      layout       = "table"
      position     = "bottom"
      show_current = true
    }

    query {
      promql = "sum by (server_address, http_response_status_code) (rate(http_client_request_duration_seconds_count{${local.scope},http_response_status_code=~\"[45]..\"}[$__interval]))"
      unit   = "number rate"

      display_info {
        display_name                      = "errors"
        time_series_display_name_template = "{{server_address}} {{http_response_status_code}}"
        type                              = "stackedArea"
      }

      format {
        decimals                = 2
        display_format          = "auto"
        input_format            = "/s"
        null_value_display_mode = "nullGap"
        y_axis                  = "auto"
      }
    }
  }

  panel {
    pos_x       = 0
    pos_y       = 63
    width       = 12
    height      = 8
    type        = "timechart"
    name        = "Open connections by destination"
    description = "Open HttpClient connections by server and connection state (idle or active)."

    legend {
      enabled      = true
      layout       = "table"
      position     = "bottom"
      show_current = true
    }

    query {
      promql = "sum by (server_address, http_connection_state) (http_client_open_connections{${local.scope}})"
      unit   = "number"

      display_info {
        display_name                      = "connections"
        time_series_display_name_template = "{{server_address}} {{http_connection_state}}"
        type                              = "stackedArea"
      }

      format {
        decimals                = 0
        display_format          = "auto"
        input_format            = "1"
        null_value_display_mode = "nullGap"
        y_axis                  = "auto"
      }
    }
  }

  panel {
    pos_x       = 12
    pos_y       = 63
    width       = 12
    height      = 8
    type        = "timechart"
    name        = "Outbound rate by destination"
    description = "HttpClient requests per second for each server.address."

    legend {
      enabled      = true
      layout       = "table"
      position     = "bottom"
      show_current = true
    }

    query {
      promql = "sum by (server_address) (rate(http_client_request_duration_seconds_count{${local.scope}}[$__interval]))"
      unit   = "number rate"

      display_info {
        display_name                      = "requests"
        time_series_display_name_template = "{{server_address}}"
        type                              = "stackedArea"
      }

      format {
        decimals                = 2
        display_format          = "auto"
        input_format            = "/s"
        null_value_display_mode = "nullGap"
        y_axis                  = "auto"
      }
    }
  }
}
