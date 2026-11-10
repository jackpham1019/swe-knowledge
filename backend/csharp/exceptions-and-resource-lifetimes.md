# C# Exceptions and Resource Lifetimes

## Exception propagation

```csharp
throw new InvalidOperationException("Invalid state"); // New failure
throw; // Inside catch: rethrow current exception, preserving stack trace
throw new ApplicationException("Operation failed", ex); // Translate, retain cause
```

C# has no checked-exception declaration requirement. Prefer catching errors where the application can recover, add meaningful context, or translate across a boundary. Avoid `throw ex;` for rethrowing because it resets stack-trace information.

## Resource ownership and lifetime

Garbage collection manages memory but does not guarantee timely release of external resources. `IDisposable` exposes synchronous cleanup, and `using` ensures `Dispose()` runs when leaving the scope (including exceptional exits). `await using` works with `IAsyncDisposable`.

```csharp
using var stream = File.OpenRead(path);
// Stream disposed when scope exits.
```

The code that creates or receives a resource must have a clear ownership contract: who disposes it, and when? A dependency-injection container may own injected services; do not automatically dispose resources you do not own.

**Exception caveat:** If both the body and `Dispose()` throw, the disposal exception can replace the body exception. C# `using` does not automatically preserve the original as a suppressed exception in the way Java try-with-resources does.
