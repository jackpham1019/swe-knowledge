# Concurrency


## Concurrency, asynchrony, and parallelism

### Concurrency vs Parallelism

**Concurrency:** Multiple operations can be in progress during overlapping periods.

**Parallelism:** Multiple operations execute simultaneously.

Asynchronous I/O enables concurrency without requiring a dedicated blocked thread for every pending operation.

`Task.Run()` schedules computation on worker threads, which may execute in parallel when sufficient CPU resources are available.

Neither approach automatically makes the underlying work faster.

|Concept|Primary Focus|Core Question|Requires Multiple Cores?|
|---|---|---|---|
|**Concurrency**|**Structure** of a program|_How do I deal with multiple things happening?_|No (can run on a single core via switching)|
|**Parallelism**|**Hardware** execution|_How do I run multiple things at the same time?_|Yes (requires multi-core/multi-CPU)|
|**Asynchrony**|**Control flow** & I/O|_How do I avoid waiting idly for a slow task?_|No (often used on a single thread)|
Concurrency describes the organization of overlapping work. Asynchrony describes how execution waits for completion. Parallelism describes simultaneous execution.




## Threads, tasks, and execution

### Mental model

A **thread** is an execution resource that runs instructions.

A **Task** represents the eventual completion of an operation.

They solve different problems:

- Threads provide execution capacity.
    
- Tasks provide a way to represent, coordinate, and observe operations.
    
- `async`/`await` provides structured asynchronous control flow.
    

A Task is not necessarily associated with a dedicated thread.

### Threads and the Thread Pool

A thread executes instructions and consumes operating-system resources.

Creating a dedicated thread for every operation can be expensive, especially when many threads spend most of their time blocked.

The .NET thread pool maintains reusable worker threads for executing work.

```
Task<int> task = Task.Run(() => CalculateTotal());
```

`Task.Run()` schedules the calculation on the thread pool.

The calculation still requires CPU execution.

#### Important Distinction

Moving an operation to another thread does not necessarily make the underlying operation more efficient.

It changes which thread performs the work.

This can be useful when the calling thread must remain responsive, such as a desktop UI thread.

### Task and Task<T>

`Task` represents an operation without a result value.

`Task<T>` represents an operation that may eventually produce a value of type `T`.

```
Task saveTask = SaveAsync();

Task<Account> accountTask = GetAccountAsync();
```

Tasks can complete successfully, fault, or be canceled.

A Task does not guarantee that:

- A new thread was created.
    
- The operation is executing in parallel.
    
- The operation is currently running.
    
- The operation is non-blocking internally.
    

The Task abstraction represents completion, not the implementation strategy.

### Async/Await

```
public async Task<Account> LoadAccountAsync(int id)
{
    Account account = await repository.GetAccountAsync(id);

    return account;
}
```

When `await` encounters an incomplete task:

1. The method preserves its execution state.
    
2. The method suspends without blocking its executing thread.
    
3. Its caller receives a task representing eventual completion.
    
4. When the awaited operation completes, the continuation can resume.
    

The continuation may execute on a different thread, depending on the execution context and scheduler.

If the awaited task is already complete, execution may continue synchronously.

**Suspension preserves logical execution state, not ownership of a thread.**



## I/O-bound versus CPU-bound work

### I/O-bound work

The application spends most of its time waiting for an external operation.

Examples:

- Database queries
    
- HTTP requests
    
- File reads and writes
    

```
Account account = await repository.GetAccountAsync(id);
```

With genuine asynchronous I/O, no dedicated thread needs to remain blocked throughout the wait.

This improves thread utilization under concurrent workloads.

### CPU-bound work

The application spends time actively executing computations.

Examples:

- Data transformations
    
- Complex statistical calculations
    
- Image processing
    

```
Report report = CalculateStatistics(transactions);
```

CPU-bound work requires CPU execution regardless of whether the surrounding method is asynchronous.

### Task.Run vs Genuine Asynchronous I/O

#### Task.Run Wrapping Blocking I/O

```
Account account = await Task.Run(
    () => repository.GetAccount(id));
```

This frees the calling thread, but a thread-pool worker remains blocked while the synchronous database operation waits.

#### Genuine Asynchronous I/O

```
Account account = await repository.GetAccountAsync(id);
```

Assuming the driver implements non-blocking I/O, the operation can remain pending without occupying a dedicated blocked thread.

#### Decision rule

- For I/O-bound operations, prefer genuine asynchronous APIs.
    
- For CPU-bound operations in desktop UI applications, `Task.Run()` can preserve responsiveness.
    
- For CPU-bound operations in ASP.NET Core, `Task.Run()` generally does not improve throughput because request processing already uses thread-pool threads.
    
- For expensive or long-running server workloads, consider background processing rather than keeping an HTTP request open.

### Architectural principle

Choose the execution mechanism according to the resource constraint:

- Waiting on external systems → avoid blocking threads.
    
- Performing computation → allocate CPU execution capacity.
    
- Protecting a responsive execution context → offload expensive work.
    
- Scaling long-running workloads → consider separating execution from request handling.
    

The goal is not to make every method asynchronous. It is to use execution resources efficiently.#



## Coordinating independent tasks with `Task.WhenAll`

### Mental model

Asynchronous execution and concurrent execution are separate concepts.

`await` allows a method to suspend without blocking a thread.

However, awaiting operations sequentially can still impose sequential dependencies.

**Concurrency requires operations to be initiated before waiting for all of them to complete.**

### Sequential Await

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

### Concurrent Task Initiation

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

### What Task.WhenAll Does

`Task.WhenAll()` creates a task representing the collective completion of the supplied tasks.

It does not itself start the operations or create threads.

The supplied operations have already been initiated, assuming their APIs return active tasks.

When all tasks finish, the combined task completes:

- Successfully if all succeed.
    
- Faulted if one or more fail.
    
- Canceled if none fault and at least one is canceled.
    

Multiple task failures are retained by the combined task. Awaiting it ordinarily propagates one exception; the combined task's `Exception` property can be inspected for the aggregate.

### When to Use Concurrency

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

### Concurrency Is Not Free

Increasing concurrency may introduce:

- Database connection pool pressure
    
- Rate limiting
    
- Memory consumption
    
- External service saturation
    
- Resource contention
    

Concurrent execution improves latency only when overlapping operations is useful and the underlying systems can support it.

### Decision rule

Sequential await is appropriate for dependent operations.

Concurrent initiation followed by `Task.WhenAll()` is appropriate for independent operations whose waiting periods can overlap.

The goal is to remove unnecessary sequencing, not to maximize concurrency unconditionally.


## Related topics

- [Async/await task flow](async-await-task-flow.md) covers suspension and continuations.
- [Cancellation](cancellation.md) covers stopping cooperative work.
- [Exceptions](exceptions.md) covers failure propagation.
