## Problem
Pods are ephemeral. Their IP addresses can
change when Pods are replaced.

## Mechanism
A Service provides a stable logical endpoint
for accessing a changing group of Pods.

A selector commonly determines which Pods
belong to the Service.

## Relationships
- Pod: Runs the application container.
- EndpointSlice: Represents backend endpoints.
- kube-proxy or another data plane: Can
  implement Service traffic forwarding.
- DNS: Resolves Service names.

## What happens if...?
What happens if no Pods match the selector?

The Service may still exist, but there are
no matching backend Pods to receive traffic.

## Practical example
[Networking lab](../../projects/microservices-lab/)

## Recall
Anki tag: k8s::networking
