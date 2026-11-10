# Validation Boundaries

Validation is distributed across layers because each layer protects a different contract.

| Layer | Responsibility | Goal |
| --- | --- | --- |
| Frontend | Required fields, formats, immediate feedback | Help construct a valid request |
| Controller / API boundary | Request shape, types, external constraints | Reject malformed or untrusted input |
| Application service | Workflow and operation preconditions | Permit only valid operations |
| Domain object / service | Business invariants and transitions | Prevent invalid business state |

Frontend validation improves usability but cannot replace server-side enforcement. Domain invariants should hold regardless of entry point (HTTP API, background job, test, or message consumer). Avoid duplicating every rule in every layer; enforce each invariant at the authoritative boundary, optionally mirroring rules for user feedback.
