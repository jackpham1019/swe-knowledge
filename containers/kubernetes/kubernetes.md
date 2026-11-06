# Kubernetes

Kubernetes keeps the observed cluster state aligned with the declared desired state through controllers and reconciliation.

## Conceptual areas
- [Architecture](architecture.md)
- [Kubernetes Services](kubernetes%20service.md)
- Workload lifecycle (planned)
- Networking (planned)

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

## Next notes to develop

- Workload lifecycle (Deployment, ReplicaSet, Pod, scheduler, kubelet)
- Networking (Service discovery, ingress, and endpoint routing)
- Storage and persistence

## Navigation

- [Containers index](containers.md)