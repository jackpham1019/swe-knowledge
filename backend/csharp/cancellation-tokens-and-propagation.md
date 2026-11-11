# Cancellation Tokens and Propagation

> **Status:** Working reference — revisit edge cases as learning progresses

## 1. Why cancellation exists

An operation may continue executing even when its caller no longer needs the result.

For example, a user requests a report, but closes the browser while the server is still generating it.

Without cancellation, the server may continue consuming resources unnecessarily.

Cancellation provides a way to communicate that an operation is no longer needed.

**Core principle:** Cancellation communicates an intent to stop, not a guarantee that execution will terminate.

In .NET, cancellation is cooperative. The executing operation must observe the cancellation request and decide how to respond.

## 2. CancellationTokenSource vs CancellationToken

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

## 3. Why CPU-bound operations must explicitly support cancellation

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

### Why not stop immediately at an arbitrary instruction?

Forcibly interrupting execution could leave partially updated state or resources in an unsafe condition.

Cooperative cancellation allows an operation to stop at appropriate boundaries.

For long-running calculations, cancellation checks may occur periodically rather than after every instruction.

The frequency depends on the desired responsiveness and the cost of checking.

### Does cancellation always require an exception?

Not necessarily.

An operation might return a partial result if its contract supports partial completion.

However, throwing `OperationCanceledException` with the canceled token is the conventional way to communicate cancellation through .NET's task-based asynchronous model.

## 4. Cancellation propagation through a call chain

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

### What happens if nested methods create independent sources?

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

## Related concepts

- [[linked-cancellation-and-timeouts]]
- [[cancellation-boundaries-and-business-consistency]]
