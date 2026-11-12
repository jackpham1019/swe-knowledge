# Exceptions

Exceptions report failures that normal return values cannot adequately represent. Handle them at boundaries where the program can recover, translate meaning, or terminate an operation appropriately.

## Propagation and rethrowing

C# has no Java-style checked-exception declaration requirement: the compiler does not require callers to catch or declare thrown exceptions.

```csharp
try
{
    await gateway.SendAsync();
}
catch (HttpRequestException)
{
    throw; // Preserve the original exception's stack information
}
```

Inside a `catch`, prefer `throw;` to `throw ex;`: the latter resets relevant stack-trace information. Avoid catching exceptions solely to log and immediately rethrow at every layer; this creates duplicate diagnostics without adding useful context.

## Exception translation at abstraction boundaries

Translate when the caller needs a more meaningful failure category, retaining the original cause as the inner exception.

```csharp
try
{
    await gateway.SendAsync();
}
catch (HttpRequestException ex)
{
    throw new PaymentGatewayException("Gateway unavailable", ex);
}
```

Translation is appropriate when an infrastructure failure becomes an application-level contract. Do not translate indiscriminately: preserving original exceptions can be better when the boundary adds no meaning.

## Exceptions during cleanup

`using` is implemented conceptually with `try/finally`. Disposal occurs when leaving scope, including exceptional exits.

```csharp
using var stream = File.OpenRead(path);
// Stream is disposed when the scope exits.
```

If both the operation and `Dispose()` throw, the disposal exception can replace the earlier failure. Unlike Java try-with-resources, C# does not automatically preserve suppressed cleanup exceptions. Design disposal to be reliable and handle cleanup failures explicitly when diagnostics require it.

## Related mechanisms

- [Resource ownership and disposal](resource-ownership-and-disposal.md) explains who must clean up and when, including `IAsyncDisposable` and `await using`.
- [Async/await task flow](async-await-task-flow.md) explains how failures are observed through tasks.
- [Cancellation](cancellation.md) explains cooperative cancellation and why canceled operations are not equivalent to arbitrary failures.
