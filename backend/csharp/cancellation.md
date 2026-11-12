# Cancellation

Cancellation in .NET is a **cooperative request to stop work**, not forced thread termination, transactional rollback, or a guarantee of a canceled outcome. The caller controls the request; participating operations determine where they can safely honor it.

## Cancellation tokens and propagation

### Why cancellation exists

An operation may continue executing even when its caller no longer needs the result.

For example, a user requests a report, but closes the browser while the server is still generating it.

Without cancellation, the server may continue consuming resources unnecessarily.

Cancellation provides a way to communicate that an operation is no longer needed.

**Core principle:** Cancellation communicates an intent to stop, not a guarantee that execution will terminate.

In .NET, cancellation is cooperative. The executing operation must observe the cancellation request and decide how to respond.

### CancellationTokenSource vs CancellationToken

These abstractions separate the authority to request cancellation from the ability to observe it.

- `CancellationTokenSource` owns the cancellation signal and exposes `Cancel()`.
    
- `CancellationToken` allows operations to observe whether cancellation has been requested.
    

```
using var cts = new CancellationTokenSource();

Task<Report> reportTask = GenerateReportAsync(cts.Token);

// The caller no longer needs the report.
cts.Cancel();

try
{
    Report report = await reportTask;
}
catch (OperationCanceledException)
{
    Console.WriteLine("Report generation canceled");
}
```

Calling `Cancel()` does not forcibly terminate the task or its executing thread.

Instead, the token becomes cancellation-requested, allowing participating operations to react.

This separation allows callers to control cancellation without exposing that authority to every dependency.

### Why CPU-bound operations must explicitly support cancellation

Consider:

```
public async Task ProcessAsync(CancellationToken token)
{
    await FetchDataAsync(token);

    CalculateStatistics(); // Takes 5 seconds

    await SaveResultAsync(token);
}
```

Suppose `FetchDataAsync()` finishes and `CalculateStatistics()` begins.

One second later, the caller requests cancellation.

Will the calculation automatically stop?

**No.**

`CalculateStatistics()` has no cancellation awareness. It will continue unless it has some independent mechanism for stopping.

To support cooperative cancellation, it must accept and observe the token.

```
public Report CalculateStatistics(
    IEnumerable<Transaction> transactions,
    CancellationToken token)
{
    var report = new Report();

    foreach (var transaction in transactions)
    {
        token.ThrowIfCancellationRequested();

        ProcessTransaction(report, transaction);
    }

    return report;
}
```

`ThrowIfCancellationRequested()` checks the token and throws `OperationCanceledException` when cancellation has been requested.

This gives the calculation explicit cancellation checkpoints.

#### Why not stop immediately at an arbitrary instruction?

Forcibly interrupting execution could leave partially updated state or resources in an unsafe condition.

Cooperative cancellation allows an operation to stop at appropriate boundaries.

For long-running calculations, cancellation checks may occur periodically rather than after every instruction.

The frequency depends on the desired responsiveness and the cost of checking.

#### Does cancellation always require an exception?

Not necessarily.

An operation might return a partial result if its contract supports partial completion.

However, throwing `OperationCanceledException` with the canceled token is the conventional way to communicate cancellation through .NET's task-based asynchronous model.

### Cancellation propagation through a call chain

Consider:

```
public async Task<Report> GenerateReportAsync(
    CancellationToken token)
{
    var data = await FetchDataAsync(token);

    var processed = await ProcessDataAsync(data, token);

    return await SaveReportAsync(processed, token);
}
```

These methods participate in the same logical report-generation operation.

Passing the same token allows each component to observe the cancellation intent of the caller.

Our reasoning was:

> These subtasks belong to the same business operation. Propagating the same cancellation token ensures that the caller's cancellation request reaches the appropriate components.

This establishes a shared cancellation signal without requiring each method to create and manage its own cancellation source.

#### What happens if nested methods create independent sources?

```
public async Task<Report> GenerateReportAsync(
    CancellationToken token)
{
    using var cts = new CancellationTokenSource();

    var data = await FetchDataAsync(cts.Token);

    return ProcessData(data);
}
```

The new source is independent of the caller's token.

Canceling the original token does not cancel `cts.Token`.

Consequently, cancellation propagation has been broken.

**General principle:** Related subtasks should share cancellation intent, but each component remains responsible for deciding whether cancellation can be honored safely.

## Linked cancellation and timeouts

### Combining caller cancellation with a timeout

Suppose report generation can be canceled by the caller, but an individual database query must also have a maximum duration of three seconds.

These are two independent cancellation conditions:

1. The caller cancels the overall report operation.
    
2. The database query exceeds its permitted duration.
    

Our initial idea was to start a timer, periodically check for cancellation, and throw an exception if the query had not completed after three seconds.

That approach captures the concept of a deadline, but manually throwing after a timeout does not necessarily stop the underlying query.

.NET supports linked cancellation tokens.

```
public async Task<Data> FetchReportDataAsync(
    CancellationToken callerToken)
{
    using var timeoutCts =
        new CancellationTokenSource(
            TimeSpan.FromSeconds(3));

    using var linkedCts =
        CancellationTokenSource.CreateLinkedTokenSource(
            callerToken,
            timeoutCts.Token);

    return await database.FetchDataAsync(
        linkedCts.Token);
}
```

The linked token becomes cancellation-requested when either source requests cancellation.

This is an **OR relationship**.

#### Example timeline

- At 0 seconds, the database query begins.
    
- At 1 second, the caller requests cancellation.
    
- The linked token immediately becomes cancellation-requested.
    
- The database driver observes the request and attempts to interrupt the query.
    

The three-second timeout does not need to expire.

However, the database driver must support and honor cancellation. A token alone cannot guarantee immediate termination.

#### Cancellation versus timeout

Caller cancellation means the caller no longer wants the operation to continue.

A timeout means the operation exceeded an allowed duration.

Both can use cancellation tokens, but they represent different reasons for stopping.

A linked token does not automatically communicate which source caused cancellation. The application may need to inspect the original sources to distinguish the outcomes.

## Cancellation boundaries and business consistency

### Cancellation is subject to business and consistency requirements

This became one of the most important conclusions of our discussion.

Consider:

```
public async Task CheckoutAsync(
    CancellationToken token)
{
    await ReserveInventoryAsync(token);

    await ChargePaymentAsync(token);

    await ConfirmOrderAsync(token);
}
```

Suppose inventory reservation succeeds and the payment is charged successfully.

Immediately afterward, the caller requests cancellation.

Should the system abandon order confirmation?

Our reasoning was that this depends on the business requirements and the consistency obligations of the workflow.

A cancellation request does not necessarily mean the customer wants to cancel the purchase.

For example, closing a browser or disconnecting from the server should not automatically invalidate a successfully charged payment.

Once important side effects have committed, the system may need to complete or recover the workflow regardless of caller cancellation.

#### Cancellation boundaries

Before important side effects occur, honoring caller cancellation may be safe.

After irreversible or externally committed actions, stopping immediately may leave the system inconsistent.

This introduces the concept of a **point of no cancellation**: a boundary after which the caller's cancellation request should no longer interrupt required consistency work.

```
public async Task CheckoutAsync(
    CancellationToken requestToken)
{
    await ReserveInventoryAsync(requestToken);

    await ChargePaymentAsync(requestToken);

    // Required consistency work should not be
    // abandoned because the caller disconnected.
    await ConfirmOrderAsync(CancellationToken.None);
}
```

This is only a simplified illustration. A production checkout must account for uncertain payment outcomes, idempotency, durable workflow state, and recovery.

**Core principle:** Cancellation is subject to the operation's business and consistency requirements.

The caller expresses a desire to stop, but the system retains responsibility for preserving valid business state.

## Cancellation versus transaction rollback

### Cancellation does not guarantee rollback

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

#### Explicit transaction example

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

## Recovery and independent cancellation scopes

### Why recovery operations need a separate cancellation scope

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

#### Architectural reasoning

The distinction is between two responsibilities:

**Primary work:** Perform the requested operation while honoring appropriate caller cancellation.

**Recovery work:** Restore or preserve consistency when the primary operation cannot complete normally.

These responsibilities may require different cancellation policies.

The general lesson is that cancellation scope follows the **ownership and lifecycle of work**, not merely the method call chain.

#

## Mental model

### Consolidated mental models

#### Cancellation is cooperative

A token communicates cancellation intent. Operations must observe it and stop at safe checkpoints.

#### Cancellation propagation preserves shared intent

Related subtasks generally receive the same token so that they can respond to the caller's cancellation request.

#### Cancellation ownership is separate from execution

The caller may own the cancellation decision without controlling how each operation responds.

#### Linked cancellation combines independent conditions

A linked token allows caller cancellation, timeouts, or other cancellation sources to govern the same operation.

#### Cancellation is not transactional rollback

Stopping execution does not establish whether committed state has been reversed.

#### Business consistency takes precedence over unconditional cancellation

Once important side effects have occurred, the workflow may need to complete or recover independently of the caller.

#### Recovery has its own lifecycle

Cleanup, rollback, and compensation should not automatically inherit the cancellation policy of the failed primary operation.

---

## Related topics

- [Concurrency](concurrency.md) — execution and task coordination.
- [Exceptions](exceptions.md) — exception propagation and handling.
- [Compensation and sagas](compensation-and-sagas.md) — recovery across committed side effects.
