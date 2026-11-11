# Threads, Tasks, and Execution

## Core Mental Model

A **thread** is an execution resource that runs instructions.

A **Task** represents the eventual completion of an operation.

They solve different problems:

- Threads provide execution capacity.
    
- Tasks provide a way to represent, coordinate, and observe operations.
    
- `async`/`await` provides structured asynchronous control flow.
    

A Task is not necessarily associated with a dedicated thread.

## Threads and the Thread Pool

A thread executes instructions and consumes operating-system resources.

Creating a dedicated thread for every operation can be expensive, especially when many threads spend most of their time blocked.

The .NET thread pool maintains reusable worker threads for executing work.

```
Task<int> task = Task.Run(() => CalculateTotal());
```

`Task.Run()` schedules the calculation on the thread pool.

The calculation still requires CPU execution.

### Important Distinction

Moving an operation to another thread does not necessarily make the underlying operation more efficient.

It changes which thread performs the work.

This can be useful when the calling thread must remain responsive, such as a desktop UI thread.

## Task and Task<T>

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

## Async/Await

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

## Related concepts

- [[async-await-task-flow]]
- [[io-bound-vs-cpu-bound-work]]
- [[task-concurrency-and-whenall]]
