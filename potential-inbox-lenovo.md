# Potential Inbox — Lenovo

This is a **device-specific capture file**, kept separate from `potential-inbox.md` to avoid merge conflicts before synchronization. Completed explanations have been moved to permanent topical notes; the subject remains open for gradual study.

## Open questions — cancellation and recovery

## 10. Questions to revisit during refinement

- How should applications distinguish a timeout from explicit caller cancellation?
    
- What happens when cancellation races with successful task completion?
    
- How should cancellation be handled when a database commit has an uncertain outcome?
    
- When should cleanup use `CancellationToken.None` versus an independent timeout?
    
- How do cancellation boundaries relate to durable workflows, transactions, and sagas?
    
- How should ASP.NET Core propagate `HttpContext.RequestAborted` through application layers?
    

These questions are candidates for later exploration, not necessarily separate Anki cards.

## Additional notes to capture

