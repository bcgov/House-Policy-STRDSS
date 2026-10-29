resource "sysdig_monitor_dashboard" "data_access" {
  name        = "STRDSS data access"
  description = "Repository, Entity Framework Core, and Npgsql metrics for the selected namespace and container. Hangfire jobs show up here as repository calls."

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
    content                = "Select a namespace, then a container in that namespace. Dev and test containers are both in b0471a-dev. UAT containers are in b0471a-test, and prod containers are in b0471a-prod. Repository labels are the interface name and the method. outcome=error is a call that threw. Hangfire runs upload processing every 5 minutes and two nightly jobs, so regular spikes on a hangfire container are expected. Npgsql charts sum away pool_name because the default pool name is the connection string. A pending-request or timeout rate above zero means the connection pool is exhausted."
  }

  panel {
    pos_x       = 0
    pos_y       = 4
    width       = 6
    height      = 4
    type        = "number"
    name        = "Repository calls"
    description = "Repository method calls per second."

    query {
      promql = "sum(rate(strdss_repository_operation_duration_seconds_count{${local.scope}}[$__interval]))"
      unit   = "number rate"

      display_info {
        display_name                      = "calls"
        time_series_display_name_template = "calls/s"
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
    name        = "Repository error percent"
    description = "Share of repository calls that threw."

    query {
      promql = "100 * ((sum(rate(strdss_repository_operation_duration_seconds_count{${local.scope},outcome=\"error\"}[$__interval])) or vector(0)) / sum(rate(strdss_repository_operation_duration_seconds_count{${local.scope}}[$__interval])))"
      unit   = "percent"

      display_info {
        display_name                      = "errors"
        time_series_display_name_template = "error %"
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
    name        = "Repository p95"
    description = "95th percentile repository call duration, including the database round trip."

    query {
      promql = "histogram_quantile(0.95, sum by (le) (rate(strdss_repository_operation_duration_seconds_bucket{${local.scope}}[$__interval])))"
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
    name        = "Repository calls in flight"
    description = "Repository methods currently executing. A value that stays above zero means calls are not completing."

    query {
      promql = "sum(strdss_repository_operation_active{${local.scope}})"
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
    name        = "Call rate by repository"
    description = "Repository calls per second, stacked by interface."

    legend {
      enabled      = true
      layout       = "table"
      position     = "bottom"
      show_current = true
    }

    query {
      promql = "sum by (repository) (rate(strdss_repository_operation_duration_seconds_count{${local.scope}}[$__interval]))"
      unit   = "number rate"

      display_info {
        display_name                      = "calls"
        time_series_display_name_template = "{{repository}}"
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
    name        = "p95 by repository"
    description = "95th percentile duration of each repository interface."

    legend {
      enabled      = true
      layout       = "table"
      position     = "bottom"
      show_current = true
    }

    query {
      promql = "histogram_quantile(0.95, sum by (le, repository) (rate(strdss_repository_operation_duration_seconds_bucket{${local.scope}}[$__interval])))"
      unit   = "time"

      display_info {
        display_name                      = "p95"
        time_series_display_name_template = "{{repository}}"
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
    pos_y       = 16
    width       = 12
    height      = 8
    type        = "timechart"
    name        = "Slowest repository methods"
    description = "Ten slowest repository methods at p95."

    legend {
      enabled      = true
      layout       = "table"
      position     = "bottom"
      show_current = true
    }

    query {
      promql = "topk(10, histogram_quantile(0.95, sum by (le, repository, operation) (rate(strdss_repository_operation_duration_seconds_bucket{${local.scope}}[$__interval]))))"
      unit   = "time"

      display_info {
        display_name                      = "p95"
        time_series_display_name_template = "{{repository}}.{{operation}}"
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
    name        = "Repository errors"
    description = "Thrown repository calls per second, by method and exception type."

    legend {
      enabled      = true
      layout       = "table"
      position     = "bottom"
      show_current = true
    }

    query {
      promql = "sum by (repository, operation, error_type) (rate(strdss_repository_operation_errors_total{${local.scope}}[$__interval]))"
      unit   = "number rate"

      display_info {
        display_name                      = "errors"
        time_series_display_name_template = "{{repository}}.{{operation}} {{error_type}}"
        type                              = "stackedArea"
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
    pos_x       = 0
    pos_y       = 24
    width       = 12
    height      = 8
    type        = "timechart"
    name        = "Calls in flight by repository"
    description = "Repository methods executing right now. A series that does not return to zero is stuck."

    legend {
      enabled      = true
      layout       = "table"
      position     = "bottom"
      show_current = true
    }

    query {
      promql = "sum by (repository) (strdss_repository_operation_active{${local.scope}})"
      unit   = "number"

      display_info {
        display_name                      = "active"
        time_series_display_name_template = "{{repository}}"
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
    pos_y       = 24
    width       = 12
    height      = 8
    type        = "timechart"
    name        = "Success and error rate"
    description = "Repository calls per second split by outcome."

    legend {
      enabled      = true
      layout       = "table"
      position     = "bottom"
      show_current = true
    }

    query {
      promql = "sum by (outcome) (rate(strdss_repository_operation_duration_seconds_count{${local.scope}}[$__interval]))"
      unit   = "number rate"

      display_info {
        display_name                      = "outcome"
        time_series_display_name_template = "{{outcome}}"
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
    pos_x                  = 0
    pos_y                  = 32
    width                  = 24
    height                 = 3
    type                   = "text"
    name                   = "Entity Framework Core"
    visible_title          = true
    autosize_text          = false
    transparent_background = false
    content                = "EF Core 9 metrics. Active DbContexts climbing without a matching drop means contexts are not being disposed. The compiled-query cache should sit near 100% after warmup. This application uses dynamic LINQ, so a cache hit rate that stays below that means new query shapes are being compiled on each call."
  }

  panel {
    pos_x       = 0
    pos_y       = 35
    width       = 6
    height      = 4
    type        = "number"
    name        = "Active DbContexts"
    description = "DbContext instances currently alive, including pooled contexts that are not in use."

    query {
      promql = "sum(microsoft_entityframeworkcore_active_dbcontexts{${local.scope}})"
      unit   = "number"

      display_info {
        display_name                      = "dbcontexts"
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
    pos_x       = 6
    pos_y       = 35
    width       = 6
    height      = 4
    type        = "number"
    name        = "Queries per second"
    description = "LINQ queries EF Core has executed."

    query {
      promql = "sum(rate(microsoft_entityframeworkcore_queries_total{${local.scope}}[$__interval]))"
      unit   = "number rate"

      display_info {
        display_name                      = "queries"
        time_series_display_name_template = "queries/s"
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
    pos_x       = 12
    pos_y       = 35
    width       = 6
    height      = 4
    type        = "number"
    name        = "SaveChanges per second"
    description = "SaveChanges calls per second. Each call can write many entities."

    query {
      promql = "sum(rate(microsoft_entityframeworkcore_savechanges_total{${local.scope}}[$__interval]))"
      unit   = "number rate"

      display_info {
        display_name                      = "savechanges"
        time_series_display_name_template = "saves/s"
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
    pos_x       = 18
    pos_y       = 35
    width       = 6
    height      = 4
    type        = "number"
    name        = "Query cache hit percent"
    description = "Compiled query cache hits divided by hits plus misses."

    query {
      promql = "100 * (sum(rate(microsoft_entityframeworkcore_compiled_query_cache_hits_total{${local.scope}}[$__interval])) or vector(0)) / ((sum(rate(microsoft_entityframeworkcore_compiled_query_cache_hits_total{${local.scope}}[$__interval])) or vector(0)) + (sum(rate(microsoft_entityframeworkcore_compiled_query_cache_misses_total{${local.scope}}[$__interval])) or vector(0)))"
      unit   = "percent"

      display_info {
        display_name                      = "hit rate"
        time_series_display_name_template = "hit %"
        type                              = "lines"
      }

      format {
        decimals                = 1
        display_format          = "auto"
        input_format            = "0-100"
        null_value_display_mode = "nullGap"
        y_axis                  = "auto"
      }
    }
  }

  panel {
    pos_x       = 0
    pos_y       = 39
    width       = 12
    height      = 8
    type        = "timechart"
    name        = "Queries and SaveChanges"
    description = "Database work EF Core is doing."

    legend {
      enabled      = true
      layout       = "table"
      position     = "bottom"
      show_current = true
    }

    query {
      promql = "sum(rate(microsoft_entityframeworkcore_queries_total{${local.scope}}[$__interval]))"
      unit   = "number rate"

      display_info {
        display_name                      = "queries"
        time_series_display_name_template = "queries/s"
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

    query {
      promql = "sum(rate(microsoft_entityframeworkcore_savechanges_total{${local.scope}}[$__interval]))"
      unit   = "number rate"

      display_info {
        display_name                      = "savechanges"
        time_series_display_name_template = "SaveChanges/s"
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
    pos_x       = 12
    pos_y       = 39
    width       = 12
    height      = 8
    type        = "timechart"
    name        = "EF Core failures"
    description = "Execution-strategy failures include each retry. Optimistic concurrency failures are DbUpdateConcurrencyException."

    legend {
      enabled      = true
      layout       = "table"
      position     = "bottom"
      show_current = true
    }

    query {
      promql = "sum(rate(microsoft_entityframeworkcore_execution_strategy_operation_failures_total{${local.scope}}[$__interval]))"
      unit   = "number rate"

      display_info {
        display_name                      = "execution strategy"
        time_series_display_name_template = "execution strategy failures/s"
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

    query {
      promql = "sum(rate(microsoft_entityframeworkcore_optimistic_concurrency_failures_total{${local.scope}}[$__interval]))"
      unit   = "number rate"

      display_info {
        display_name                      = "concurrency"
        time_series_display_name_template = "optimistic concurrency failures/s"
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
    pos_x                  = 0
    pos_y                  = 47
    width                  = 24
    height                 = 3
    type                   = "text"
    name                   = "Npgsql connection pool"
    visible_title          = true
    autosize_text          = false
    transparent_background = false
    content                = "Pool saturation is the failure mode to watch. Used connections approaching max, pending requests above zero, or any timeout rate means callers are waiting or failing to get a connection. Charts do not group by pool name."
  }

  panel {
    pos_x       = 0
    pos_y       = 50
    width       = 6
    height      = 4
    type        = "number"
    name        = "Pool used percent"
    description = "Busy connections divided by the pool maximum."

    query {
      promql = "100 * sum(db_client_connections_usage{${local.scope},state=\"used\"}) / sum(db_client_connections_max{${local.scope}})"
      unit   = "percent"

      display_info {
        display_name                      = "used"
        time_series_display_name_template = "used %"
        type                              = "lines"
      }

      format {
        decimals                = 1
        display_format          = "auto"
        input_format            = "0-100"
        null_value_display_mode = "nullGap"
        y_axis                  = "auto"
      }
    }
  }

  panel {
    pos_x       = 6
    pos_y       = 50
    width       = 6
    height      = 4
    type        = "number"
    name        = "Pending connection requests"
    description = "Callers waiting for a pooled connection. This should stay at zero."

    query {
      promql = "sum(db_client_connections_pending_requests{${local.scope}})"
      unit   = "number"

      display_info {
        display_name                      = "pending"
        time_series_display_name_template = "pending"
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
    pos_x       = 12
    pos_y       = 50
    width       = 6
    height      = 4
    type        = "number"
    name        = "Connection timeouts"
    description = "Failures to obtain a connection from the pool, per second."

    query {
      promql = "sum(rate(db_client_connections_timeouts_total{${local.scope}}[$__interval]))"
      unit   = "number rate"

      display_info {
        display_name                      = "timeouts"
        time_series_display_name_template = "timeouts/s"
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
    pos_x       = 18
    pos_y       = 50
    width       = 6
    height      = 4
    type        = "number"
    name        = "Command p95"
    description = "95th percentile Npgsql command duration."

    query {
      promql = "histogram_quantile(0.95, sum by (le) (rate(db_client_commands_duration_seconds_bucket{${local.scope}}[$__interval])))"
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
    pos_x       = 0
    pos_y       = 54
    width       = 12
    height      = 8
    type        = "timechart"
    name        = "Pool connections"
    description = "Idle and busy connections against the pool maximum. Pending requests are callers waiting for a connection and should stay at zero."

    legend {
      enabled      = true
      layout       = "table"
      position     = "bottom"
      show_current = true
    }

    query {
      promql = "sum(db_client_connections_usage{${local.scope},state=\"used\"})"
      unit   = "number"

      display_info {
        display_name                      = "used"
        time_series_display_name_template = "used"
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
      promql = "sum(db_client_connections_usage{${local.scope},state=\"idle\"})"
      unit   = "number"

      display_info {
        display_name                      = "idle"
        time_series_display_name_template = "idle"
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
      promql = "sum(db_client_connections_max{${local.scope}})"
      unit   = "number"

      display_info {
        display_name                      = "max"
        time_series_display_name_template = "max"
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
      promql = "sum(db_client_connections_pending_requests{${local.scope}})"
      unit   = "number"

      display_info {
        display_name                      = "pending"
        time_series_display_name_template = "pending"
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
    pos_x       = 12
    pos_y       = 54
    width       = 12
    height      = 8
    type        = "timechart"
    name        = "Command latency"
    description = "Npgsql command duration. This is the time the driver spent running the command."

    legend {
      enabled      = true
      layout       = "table"
      position     = "bottom"
      show_current = true
    }

    query {
      promql = "histogram_quantile(0.50, sum by (le) (rate(db_client_commands_duration_seconds_bucket{${local.scope}}[$__interval])))"
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
      promql = "histogram_quantile(0.95, sum by (le) (rate(db_client_commands_duration_seconds_bucket{${local.scope}}[$__interval])))"
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
      promql = "histogram_quantile(0.99, sum by (le) (rate(db_client_commands_duration_seconds_bucket{${local.scope}}[$__interval])))"
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
    pos_x       = 0
    pos_y       = 62
    width       = 12
    height      = 8
    type        = "timechart"
    name        = "Pool timeouts and failed commands"
    description = "Pool timeouts and commands Npgsql reported as failed, per second. The pending-request number above is the current waiter count."

    legend {
      enabled      = true
      layout       = "table"
      position     = "bottom"
      show_current = true
    }

    query {
      promql = "sum(rate(db_client_connections_timeouts_total{${local.scope}}[$__interval]))"
      unit   = "number rate"

      display_info {
        display_name                      = "timeouts"
        time_series_display_name_template = "timeouts/s"
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

    query {
      promql = "sum(rate(db_client_commands_failed_total{${local.scope}}[$__interval]))"
      unit   = "number rate"

      display_info {
        display_name                      = "failed commands"
        time_series_display_name_template = "failed commands/s"
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
    pos_y       = 62
    width       = 12
    height      = 8
    type        = "timechart"
    name        = "Command bytes"
    description = "Bytes Npgsql read from and wrote to PostgreSQL. A spike alongside slow commands points at a large result or a large write."

    legend {
      enabled      = true
      layout       = "table"
      position     = "bottom"
      show_current = true
    }

    query {
      promql = "sum(rate(db_client_commands_bytes_read_bytes_total{${local.scope}}[$__interval]))"
      unit   = "data rate"

      display_info {
        display_name                      = "read"
        time_series_display_name_template = "read"
        type                              = "lines"
      }

      format {
        decimals                = 0
        display_format          = "auto"
        input_format            = "B/s"
        null_value_display_mode = "nullGap"
        y_axis                  = "auto"
      }
    }

    query {
      promql = "sum(rate(db_client_commands_bytes_written_bytes_total{${local.scope}}[$__interval]))"
      unit   = "data rate"

      display_info {
        display_name                      = "written"
        time_series_display_name_template = "written"
        type                              = "lines"
      }

      format {
        decimals                = 0
        display_format          = "auto"
        input_format            = "B/s"
        null_value_display_mode = "nullGap"
        y_axis                  = "auto"
      }
    }
  }

  panel {
    pos_x       = 0
    pos_y       = 70
    width       = 12
    height      = 8
    type        = "timechart"
    name        = "New connection time p95"
    description = "Time to open a physical connection. Samples exist only when the pool creates a connection, so this chart is sparse while the pool is warm."

    legend {
      enabled      = true
      layout       = "table"
      position     = "bottom"
      show_current = true
    }

    query {
      promql = "histogram_quantile(0.95, sum by (le) (rate(db_client_connections_create_time_seconds_bucket{${local.scope}}[$__interval])))"
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
    pos_x       = 12
    pos_y       = 70
    width       = 12
    height      = 8
    type        = "timechart"
    name        = "Prepared command percent"
    description = "Share of commands that were prepared since the previous scrape. Npgsql resets this ratio on each observation."

    legend {
      enabled      = true
      layout       = "table"
      position     = "bottom"
      show_current = true
    }

    query {
      promql = "avg(db_client_commands_prepared_ratio{${local.scope}})"
      unit   = "percent"

      display_info {
        display_name                      = "prepared"
        time_series_display_name_template = "prepared %"
        type                              = "lines"
      }

      format {
        decimals                = 1
        display_format          = "auto"
        input_format            = "0-100"
        null_value_display_mode = "nullGap"
        y_axis                  = "auto"
      }
    }
  }
}
