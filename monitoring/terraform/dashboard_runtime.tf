resource "sysdig_monitor_dashboard" "runtime" {
  name        = "STRDSS runtime"
  description = ".NET GC, exceptions, and thread-pool metrics for the selected namespace and container. Exception rate includes exceptions the application catches."

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
    content                = "Select a namespace, then a container in that namespace. Dev and test containers are both in b0471a-dev. UAT containers are in b0471a-test, and prod containers are in b0471a-prod. GC heap and pause charts describe the .NET 8 runtime instrumentation, not process working set. The exception rate counts every exception thrown, including handled ones, from the moment instrumentation started. Thread-pool queue length is the .NET pool, not the Hangfire worker count of 5. A queue that grows while completed work stays flat means the process is saturated."
  }

  panel {
    pos_x       = 0
    pos_y       = 4
    width       = 6
    height      = 4
    type        = "number"
    name        = "Exceptions per second"
    description = "First-chance exceptions per second, including handled exceptions."

    query {
      promql = "sum(rate(process_runtime_dotnet_exceptions_count_total{${local.scope}}[$__interval]))"
      unit   = "number rate"

      display_info {
        display_name                      = "exceptions"
        time_series_display_name_template = "exceptions/s"
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
    name        = "GC pause percent"
    description = "Average share of time pods spent paused for GC."

    query {
      promql = "100 * avg(rate(process_runtime_dotnet_gc_duration_nanoseconds_total{${local.scope}}[$__interval])) / 1000000000"
      unit   = "percent"

      display_info {
        display_name                      = "gc pause"
        time_series_display_name_template = "GC pause %"
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
    name        = "Longest thread-pool queue"
    description = "Largest .NET thread-pool queue among pods in the selected workload."

    query {
      promql = "max(process_runtime_dotnet_thread_pool_queue_length{${local.scope}})"
      unit   = "number"

      display_info {
        display_name                      = "queue"
        time_series_display_name_template = "queue"
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
    pos_x       = 18
    pos_y       = 4
    width       = 6
    height      = 4
    type        = "number"
    name        = "Busiest thread pool"
    description = "Largest .NET thread-pool thread count among pods in the selected workload."

    query {
      promql = "max(process_runtime_dotnet_thread_pool_threads_count{${local.scope}})"
      unit   = "number"

      display_info {
        display_name                      = "threads"
        time_series_display_name_template = "threads"
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
    name        = "GC heap by generation"
    description = "Managed heap size after the latest collection, summed across pods. loh is the large object heap and poh is the pinned object heap."

    legend {
      enabled      = true
      layout       = "table"
      position     = "bottom"
      show_current = true
    }

    query {
      promql = "sum by (generation) (process_runtime_dotnet_gc_heap_size_bytes{${local.scope}})"
      unit   = "data"

      display_info {
        display_name                      = "heap"
        time_series_display_name_template = "{{generation}}"
        type                              = "stackedArea"
      }

      format {
        decimals                = 0
        display_format          = "auto"
        input_format            = "B"
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
    name        = "Managed memory by pod"
    description = "Live object bytes and committed GC heap. Committed memory stays above live objects because it includes space reserved for new allocations."

    legend {
      enabled      = true
      layout       = "table"
      position     = "bottom"
      show_current = true
    }

    query {
      promql = "sum by (kube_pod_name) (process_runtime_dotnet_gc_objects_size_bytes{${local.scope}})"
      unit   = "data"

      display_info {
        display_name                      = "live objects"
        time_series_display_name_template = "{{kube_pod_name}} live objects"
        type                              = "lines"
      }

      format {
        decimals                = 0
        display_format          = "auto"
        input_format            = "B"
        null_value_display_mode = "nullGap"
        y_axis                  = "auto"
      }
    }

    query {
      promql = "sum by (kube_pod_name) (process_runtime_dotnet_gc_committed_memory_size_bytes{${local.scope}})"
      unit   = "data"

      display_info {
        display_name                      = "committed"
        time_series_display_name_template = "{{kube_pod_name}} committed"
        type                              = "lines"
      }

      format {
        decimals                = 0
        display_format          = "auto"
        input_format            = "B"
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
    name        = "Heap fragmentation"
    description = "Fragmented bytes after the latest collection. Growth in loh fragmentation usually means large objects are being allocated and freed repeatedly."

    legend {
      enabled      = true
      layout       = "table"
      position     = "bottom"
      show_current = true
    }

    query {
      promql = "sum by (generation) (process_runtime_dotnet_gc_heap_fragmentation_size_bytes{${local.scope}})"
      unit   = "data"

      display_info {
        display_name                      = "fragmentation"
        time_series_display_name_template = "{{generation}}"
        type                              = "stackedArea"
      }

      format {
        decimals                = 0
        display_format          = "auto"
        input_format            = "B"
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
    name        = "Allocation rate"
    description = "Bytes allocated on the managed heap per second. A pod that allocates much faster than the others is creating GC pressure."

    legend {
      enabled      = true
      layout       = "table"
      position     = "bottom"
      show_current = true
    }

    query {
      promql = "sum by (kube_pod_name) (rate(process_runtime_dotnet_gc_allocations_size_bytes_total{${local.scope}}[$__interval]))"
      unit   = "data rate"

      display_info {
        display_name                      = "allocated"
        time_series_display_name_template = "{{kube_pod_name}}"
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
    pos_y       = 24
    width       = 12
    height      = 8
    type        = "timechart"
    name        = "Garbage collections"
    description = "Collections per second by generation. gen2 collections are the expensive ones."

    legend {
      enabled      = true
      layout       = "table"
      position     = "bottom"
      show_current = true
    }

    query {
      promql = "sum by (generation) (rate(process_runtime_dotnet_gc_collections_count_total{${local.scope}}[$__interval]))"
      unit   = "number rate"

      display_info {
        display_name                      = "collections"
        time_series_display_name_template = "{{generation}}"
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
    pos_x       = 12
    pos_y       = 24
    width       = 12
    height      = 8
    type        = "timechart"
    name        = "GC pause by pod"
    description = "Share of time each pod spent paused for garbage collection."

    legend {
      enabled      = true
      layout       = "table"
      position     = "bottom"
      show_current = true
    }

    query {
      promql = "100 * sum by (kube_pod_name) (rate(process_runtime_dotnet_gc_duration_nanoseconds_total{${local.scope}}[$__interval])) / 1000000000"
      unit   = "percent"

      display_info {
        display_name                      = "pause"
        time_series_display_name_template = "{{kube_pod_name}}"
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
    pos_x       = 0
    pos_y       = 32
    width       = 12
    height      = 8
    type        = "timechart"
    name        = "Exceptions by pod"
    description = "Exceptions thrown per second, including handled exceptions. Compare with the HTTP 5xx chart before treating a spike as user-facing errors."

    legend {
      enabled      = true
      layout       = "table"
      position     = "bottom"
      show_current = true
    }

    query {
      promql = "sum by (kube_pod_name) (rate(process_runtime_dotnet_exceptions_count_total{${local.scope}}[$__interval]))"
      unit   = "number rate"

      display_info {
        display_name                      = "exceptions"
        time_series_display_name_template = "{{kube_pod_name}}"
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
    pos_y       = 32
    width       = 12
    height      = 8
    type        = "timechart"
    name        = "Lock contention"
    description = "Times per second a thread blocked on a monitor lock (lock / Monitor.Enter)."

    legend {
      enabled      = true
      layout       = "table"
      position     = "bottom"
      show_current = true
    }

    query {
      promql = "sum by (kube_pod_name) (rate(process_runtime_dotnet_monitor_lock_contention_count_total{${local.scope}}[$__interval]))"
      unit   = "number rate"

      display_info {
        display_name                      = "contention"
        time_series_display_name_template = "{{kube_pod_name}}"
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
    pos_y       = 40
    width       = 12
    height      = 8
    type        = "timechart"
    name        = "Thread-pool threads"
    description = "Threads currently in the .NET thread pool."

    legend {
      enabled      = true
      layout       = "table"
      position     = "bottom"
      show_current = true
    }

    query {
      promql = "sum by (kube_pod_name) (process_runtime_dotnet_thread_pool_threads_count{${local.scope}})"
      unit   = "number"

      display_info {
        display_name                      = "threads"
        time_series_display_name_template = "{{kube_pod_name}}"
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
    pos_y       = 40
    width       = 12
    height      = 8
    type        = "timechart"
    name        = "Thread-pool queue"
    description = "Work items waiting for a thread-pool thread. Sustained growth means the process cannot keep up."

    legend {
      enabled      = true
      layout       = "table"
      position     = "bottom"
      show_current = true
    }

    query {
      promql = "sum by (kube_pod_name) (process_runtime_dotnet_thread_pool_queue_length{${local.scope}})"
      unit   = "number"

      display_info {
        display_name                      = "queue"
        time_series_display_name_template = "{{kube_pod_name}}"
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
    pos_y       = 48
    width       = 12
    height      = 8
    type        = "timechart"
    name        = "Thread-pool throughput"
    description = "Work items the thread pool completed per second."

    legend {
      enabled      = true
      layout       = "table"
      position     = "bottom"
      show_current = true
    }

    query {
      promql = "sum by (kube_pod_name) (rate(process_runtime_dotnet_thread_pool_completed_items_count_total{${local.scope}}[$__interval]))"
      unit   = "number rate"

      display_info {
        display_name                      = "completed"
        time_series_display_name_template = "{{kube_pod_name}}"
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
    pos_y       = 48
    width       = 12
    height      = 8
    type        = "timechart"
    name        = "Active timers"
    description = "Timers registered to fire in the future, including Task.Delay and cancellation timeouts. A steady climb suggests timers are being left running."

    legend {
      enabled      = true
      layout       = "table"
      position     = "bottom"
      show_current = true
    }

    query {
      promql = "sum by (kube_pod_name) (process_runtime_dotnet_timer_count{${local.scope}})"
      unit   = "number"

      display_info {
        display_name                      = "timers"
        time_series_display_name_template = "{{kube_pod_name}}"
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
}
