#!/bin/bash

# Get CPU and memory usage for each pod
echo "CPU and Memory Usage for Pods:"
oc adm top pods | awk '{print $1,$2,$3,$4,$5,$6}' | column -t

# Get resource requests and limits for each pod
echo -e "\nPod Resource Requests and Limits:"
oc get pods -o jsonpath='{range .items[*]}{"Pod: "}{.metadata.name}{"\nCPU Requests: "}{.spec.containers[*].resources.requests.cpu}{"\nCPU Limits: "}{.spec.containers[*].resources.limits.cpu}{"\nMemory Requests: "}{.spec.containers[*].resources.requests.memory}{"\nMemory Limits: "}{.spec.containers[*].resources.limits.memory}{"\n\n"}{end}' 

# Get resource quota for the project
echo -e "\nResource Quotas for the Project:"
oc get quota

