locals {
  # Sysdig does not apply dashboard scope to PromQL unless the query references
  # the scope variable. The variable must be unquoted, and the operator must
  # match the scope comparator: equals uses =, in uses =~. Quoting the variable
  # leaves $namespace as a literal and the panel returns no data.
  # Namespace is chosen first; the container list is then limited to that
  # namespace. Dev and test containers both run in b0471a-dev. UAT containers
  # run in b0471a-test, and prod containers run in b0471a-prod.
  scope = "kube_namespace_name=$namespace,container_name=$container"

  # /healthz and /metrics are scraped continuously and would dominate rate and latency.
  http_server = "${local.scope},http_route!~\"/healthz|/metrics\""
}

# Metric names are the Prometheus names produced by OpenTelemetry.Exporter.Prometheus.AspNetCore
# 1.20 from the meters enabled in StrDss.Api and StrDss.Hangfire. Both processes expose the
# same meters; the namespace and container scope selects one process.
#
# HTTP server (AddAspNetCoreInstrumentation, .NET 8 built-in meters):
#   http_server_request_duration_seconds_{bucket,sum,count}
#     http_request_method, http_response_status_code, http_route, error_type
#   http_server_active_requests
#   kestrel_active_connections, kestrel_queued_connections
#   kestrel_rejected_connections_total
#   kestrel_connection_duration_seconds_{bucket,sum,count}
#
# HTTP client (AddHttpClientInstrumentation). Covers CHES, the geocoder, and the
# registration API. The BCEID SOAP client uses WCF and is not in these series.
#   http_client_request_duration_seconds_{bucket,sum,count}
#     server_address, http_request_method, http_response_status_code, error_type
#   http_client_open_connections  (http_connection_state)
#
# Repositories (meter StrDss.Repositories):
#   strdss_repository_operation_duration_seconds_{bucket,sum,count}
#     repository = interface name, operation = method, outcome = success|error
#   strdss_repository_operation_errors_total
#     repository, operation, error_type
#   strdss_repository_operation_active
#     repository, operation
#
# EF Core 9 (meter Microsoft.EntityFrameworkCore):
#   microsoft_entityframeworkcore_active_dbcontexts
#   microsoft_entityframeworkcore_queries_total
#   microsoft_entityframeworkcore_savechanges_total
#   microsoft_entityframeworkcore_compiled_query_cache_{hits,misses}_total
#   microsoft_entityframeworkcore_execution_strategy_operation_failures_total
#   microsoft_entityframeworkcore_optimistic_concurrency_failures_total
#
# Npgsql 9 (meter Npgsql). Queries sum away pool_name: Npgsql's default pool name
# is the connection string.
#   db_client_connections_usage{state="idle"|"used"}
#   db_client_connections_max
#   db_client_connections_pending_requests
#   db_client_connections_timeouts_total
#   db_client_connections_create_time_seconds_{bucket,sum,count}
#   db_client_commands_duration_seconds_{bucket,sum,count}
#   db_client_commands_failed_total
#   db_client_commands_bytes_{read,written}_bytes_total
#   db_client_commands_prepared_ratio   (0-100, since the previous scrape)
#
# Runtime (OpenTelemetry.Instrumentation.Runtime 1.12 on .NET 8):
#   process_runtime_dotnet_gc_collections_count_total{generation}
#   process_runtime_dotnet_gc_objects_size_bytes
#   process_runtime_dotnet_gc_allocations_size_bytes_total
#   process_runtime_dotnet_gc_committed_memory_size_bytes
#   process_runtime_dotnet_gc_heap_size_bytes{generation}
#   process_runtime_dotnet_gc_heap_fragmentation_size_bytes{generation}
#   process_runtime_dotnet_gc_duration_nanoseconds_total
#   process_runtime_dotnet_exceptions_count_total
#     First-chance exceptions, including ones the application catches.
#   process_runtime_dotnet_monitor_lock_contention_count_total
#   process_runtime_dotnet_thread_pool_threads_count
#   process_runtime_dotnet_thread_pool_queue_length
#   process_runtime_dotnet_thread_pool_completed_items_count_total
#   process_runtime_dotnet_timer_count
