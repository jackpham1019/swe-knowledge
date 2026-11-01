## Purpose
Orchestrate containerized applications across
a cluster of machines.

## Core mental model
Kubernetes continuously reconciles actual
cluster state toward declared desired state.

## Conceptual areas
- [Architecture](architecture.md)
- [Workload lifecycle](workload-lifecycle.md)
- [Networking](networking.md)

## Relationships
- Deployments manage ReplicaSets.
- ReplicaSets maintain desired Pod counts.
- Services provide stable access to Pods.
- Ingress routes external HTTP(S) traffic.

## Scenarios to understand
- A Pod crashes.
- A node becomes unavailable.
- A Service has no ready endpoints.
- A rolling deployment fails.

## Related topics
- Container networking
- Distributed systems
- Infrastructure as Code
