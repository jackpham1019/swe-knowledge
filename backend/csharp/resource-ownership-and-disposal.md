# Resource Ownership, Lifetime, and Disposal

Garbage collection manages memory, not timely release of files, sockets, database connections, or other external resources. `IDisposable` (and `IAsyncDisposable`) provides an explicit cleanup contract.

```csharp
using var stream = File.OpenRead(path);
// stream.Dispose() runs on scope exit, including exceptions
```

```csharp
await using var stream = new FileStream(path, FileMode.Open,
    FileAccess.Read, FileShare.Read, 4096, useAsync: true);
// DisposeAsync() runs on scope exit
```

## Ownership versus lifetime

- **Ownership**: who is responsible for disposing the resource?
- **Lifetime**: how long must it remain usable?

A method that creates and owns a stream commonly disposes it. A method that receives a stream from a caller should not assume ownership unless the API contract says so. Returning a stream created inside `using` returns an already disposed object.

```csharp
Stream OpenBad(string path)
{
    using var s = File.OpenRead(path);
    return s; // disposed before caller can use it
}
```

For dependency-injection-managed objects, the container often owns disposal; manually disposing them can break later consumers. Prefer clear ownership conventions over indiscriminate `using`. Related: [exception-propagation](exceptions.md).
