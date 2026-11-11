# Cancellation and Transaction Recovery

> **Status:** Working reference

## 6. Cancellation does not guarantee rollback

We discussed a database query that receives a cancellation request.

An initial assumption was that the database management system would automatically perform an appropriate rollback.

That is not guaranteed.

Cancellation concerns execution control.

Rollback concerns transactional consistency.

These are separate responsibilities.

For example:

```
await db.ExecuteAsync(
    "UPDATE Accounts SET Balance = Balance - 100 WHERE Id = 1",
    token);
```

If cancellation occurs, the application cannot automatically conclude that the update was rolled back.

The database statement might already have completed, or cancellation might have interrupted it.

The outcome depends on the database provider, transaction boundaries, and timing.

### Explicit transaction example

```
await using var transaction =
    await db.BeginTransactionAsync(token);

try
{
    await UpdateAccountAsync(transaction, token);
    await InsertAuditRecordAsync(transaction, token);

    await transaction.CommitAsync(token);
}
catch
{
    await transaction.RollbackAsync(
        CancellationToken.None);

    throw;
}
```

Here, the application explicitly establishes a transaction boundary.

If an operation fails before commit, it attempts to roll back the transaction.

Rollback can itself fail, and interruption during commit can leave the outcome uncertain. Therefore, transaction recovery may require additional safeguards.

**Architectural distinction:**

- Cancellation requests that execution stop.
    
- Transactions define atomic commit and rollback boundaries.
    
- Compensation addresses previously committed side effects that cannot be rolled back together.

## 8. Why recovery operations need a separate cancellation scope

Suppose an application updates an account balance inside a transaction.

Before committing, the caller requests cancellation.

The application catches the cancellation exception and needs to roll back.

Should rollback use the already-canceled token?

Our conclusion was **no**.

The original token represents the caller's intent to stop the primary operation.

Rollback is recovery work initiated because the original operation was interrupted.

The recovery work does not necessarily belong to the same cancellation lifecycle.

```
try
{
    await UpdateBalanceAsync(requestToken);

    await CommitAsync(requestToken);
}
catch
{
    await RollbackAsync(CancellationToken.None);
    throw;
}
```

Passing an already-canceled token could cause rollback to be canceled immediately.

Using `CancellationToken.None` avoids that specific problem.

In production, a separate cancellation token source with its own timeout may be preferable so that recovery does not run indefinitely.

### Architectural reasoning

The distinction is between two responsibilities:

**Primary work:** Perform the requested operation while honoring appropriate caller cancellation.

**Recovery work:** Restore or preserve consistency when the primary operation cannot complete normally.

These responsibilities may require different cancellation policies.

The general lesson is that cancellation scope follows the **ownership and lifecycle of work**, not merely the method call chain.

## Related concepts

- [[cancellation-boundaries-and-business-consistency]]
- [[compensation-and-sagas]]
