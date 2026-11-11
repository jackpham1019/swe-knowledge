# Task Concurrency and Task.WhenAll

## Core Mental Model

Asynchronous execution and concurrent execution are separate concepts.

`await` allows a method to suspend without blocking a thread.

However, awaiting operations sequentially can still impose sequential dependencies.

**Concurrency requires operations to be initiated before waiting for all of them to complete.**

## Sequential Await

```
Account account = await GetAccountAsync();
Balance balance = await GetBalanceAsync();
Transactions transactions = await GetTransactionsAsync();
```

Execution:

1. Start account retrieval.
    
2. Await its completion.
    
3. Start balance retrieval.
    
4. Await its completion.
    
5. Start transaction retrieval.
    
6. Await its completion.
    

Even if each operation uses genuine asynchronous I/O, the workflow is sequential.

Assuming each operation takes 200 ms, the total elapsed time is approximately 600 ms, excluding overhead.

## Concurrent Task Initiation

```
Task<Account> accountTask = GetAccountAsync();
Task<Balance> balanceTask = GetBalanceAsync();
Task<Transactions> transactionsTask = GetTransactionsAsync();

await Task.WhenAll(
    accountTask,
    balanceTask,
    transactionsTask);

Account account = await accountTask;
Balance balance = await balanceTask;
Transactions transactions = await transactionsTask;
```

Execution:

1. Initiate account retrieval.
    
2. Initiate balance retrieval.
    
3. Initiate transaction retrieval.
    
4. Await completion of all three operations.
    
5. Retrieve their results.
    

Assuming all three operations take approximately 200 ms and can overlap, total elapsed time approaches 200 ms plus initiation and scheduling overhead.

The final individual awaits do not suspend if the tasks have already completed successfully.

## What Task.WhenAll Does

`Task.WhenAll()` creates a task representing the collective completion of the supplied tasks.

It does not itself start the operations or create threads.

The supplied operations have already been initiated, assuming their APIs return active tasks.

When all tasks finish, the combined task completes:

- Successfully if all succeed.
    
- Faulted if one or more fail.
    
- Canceled if none fault and at least one is canceled.
    

Multiple task failures are retained by the combined task. Awaiting it ordinarily propagates one exception; the combined task's `Exception` property can be inspected for the aggregate.

## When to Use Concurrency

Use concurrent initiation when operations:

- Are independent.
    
- Do not require results from one another.
    
- Can safely overlap.
    
- Are supported by the capacity of external systems.
    

Prefer sequential execution when operations have genuine dependencies.

Example:

```
Account account = await GetAccountAsync();

Balance balance = await GetBalanceAsync(account.Id);
```

Balance retrieval depends on the account result, so it cannot be initiated independently.

## Concurrency Is Not Free

Increasing concurrency may introduce:

- Database connection pool pressure
    
- Rate limiting
    
- Memory consumption
    
- External service saturation
    
- Resource contention
    

Concurrent execution improves latency only when overlapping operations is useful and the underlying systems can support it.

## Decision Rule

Sequential await is appropriate for dependent operations.

Concurrent initiation followed by `Task.WhenAll()` is appropriate for independent operations whose waiting periods can overlap.

The goal is to remove unnecessary sequencing, not to maximize concurrency unconditionally.

## Related concepts

- [[concurrency-asynchrony-and-parallelism]]
- [[cancellation-tokens-and-propagation]]
