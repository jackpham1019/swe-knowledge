# C# Async/Await: Task Flow and Exceptions

`Task<T>` represents an asynchronous operation that may produce a result, fault, or be canceled. It is not a dedicated thread. `await` is a **conditional suspension point**: if the awaited operation is incomplete, the async method suspends without blocking its executing thread; if complete, execution may continue synchronously.

The compiler-generated state machine preserves locals and exception-handling context. A suspended method is logically incomplete but not actively executing; its continuation may run on a different thread.

## Direct task forwarding

```csharp
public Task<Account> GetAccountAsync(int id)
{
    return accountClient.GetAccountAsync(id);
}
```

This method returns the task and ends its own execution. The returned task can still be incomplete or fault later.

## Awaiting before returning

```csharp
public async Task<Account> GetAccountAsync(int id)
{
    Account account = await accountClient.GetAccountAsync(id);
    Console.WriteLine("Account retrieved");
    return account;
}
```

`return await operation;` is conceptually equivalent to awaiting the result, then returning it. Await when the method needs post-processing, local handling of asynchronous faults, or specific lifetime/cleanup semantics.

## Why try/catch behaves differently

```csharp
public Task<Account> Direct(int id)
{
    try { return accountClient.GetAccountAsync(id); }
    catch (HttpRequestException) { Console.WriteLine("Synchronous throw"); throw; }
}

public async Task<Account> Awaited(int id)
{
    try { return await accountClient.GetAccountAsync(id); }
    catch (HttpRequestException) { Console.WriteLine("Caught fault"); throw; }
}
```

The direct method catches only a **synchronous throw during invocation**, not a task that faults later. The awaited method observes a task fault at `await` inside its preserved `try/catch` and can handle it. Note that `async` changes when some exceptions are surfaced to the caller: exceptions from an async method are generally captured in its returned task.

## Async is not parallelism

Concurrency means overlapping progress; parallelism means simultaneous execution. Asynchronous I/O can free threads while waiting for external work, but does not inherently speed up a database query or guarantee parallel execution. See [[exception-propagation]], [[resource-ownership-and-disposal]].
