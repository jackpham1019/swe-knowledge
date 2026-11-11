# I/O-Bound vs CPU-Bound Work

## I/O-Bound vs CPU-Bound Work

### I/O-Bound

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

### CPU-Bound

The application spends time actively executing computations.

Examples:

- Data transformations
    
- Complex statistical calculations
    
- Image processing
    

```
Report report = CalculateStatistics(transactions);
```

CPU-bound work requires CPU execution regardless of whether the surrounding method is asynchronous.

## Task.Run vs Genuine Asynchronous I/O

### Task.Run Wrapping Blocking I/O

```
Account account = await Task.Run(
    () => repository.GetAccount(id));
```

This frees the calling thread, but a thread-pool worker remains blocked while the synchronous database operation waits.

### Genuine Asynchronous I/O

```
Account account = await repository.GetAccountAsync(id);
```

Assuming the driver implements non-blocking I/O, the operation can remain pending without occupying a dedicated blocked thread.

### Decision Rule

- For I/O-bound operations, prefer genuine asynchronous APIs.
    
- For CPU-bound operations in desktop UI applications, `Task.Run()` can preserve responsiveness.
    
- For CPU-bound operations in ASP.NET Core, `Task.Run()` generally does not improve throughput because request processing already uses thread-pool threads.
    
- For expensive or long-running server workloads, consider background processing rather than keeping an HTTP request open.

## Architectural Principle

Choose the execution mechanism according to the resource constraint:

- Waiting on external systems → avoid blocking threads.
    
- Performing computation → allocate CPU execution capacity.
    
- Protecting a responsive execution context → offload expensive work.
    
- Scaling long-running workloads → consider separating execution from request handling.
    

The goal is not to make every method asynchronous. It is to use execution resources efficiently.#

## Related concepts

- [[threads-tasks-and-execution]]
- [[async-await-task-flow]]
