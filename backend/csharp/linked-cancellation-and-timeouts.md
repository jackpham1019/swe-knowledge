# Linked Cancellation and Timeouts

> **Status:** Working reference

## 5. Combining caller cancellation with a timeout

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

### Example timeline

- At 0 seconds, the database query begins.
    
- At 1 second, the caller requests cancellation.
    
- The linked token immediately becomes cancellation-requested.
    
- The database driver observes the request and attempts to interrupt the query.
    

The three-second timeout does not need to expire.

However, the database driver must support and honor cancellation. A token alone cannot guarantee immediate termination.

### Cancellation versus timeout

Caller cancellation means the caller no longer wants the operation to continue.

A timeout means the operation exceeded an allowed duration.

Both can use cancellation tokens, but they represent different reasons for stopping.

A linked token does not automatically communicate which source caused cancellation. The application may need to inspect the original sources to distinguish the outcomes.

## Related concepts

- [[cancellation-tokens-and-propagation]]
- [[cancellation-boundaries-and-business-consistency]]
