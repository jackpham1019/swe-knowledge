# Concurrency, Asynchrony, and Parallelism

## Concurrency vs Parallelism

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

## Related concepts

- [[task-concurrency-and-whenall]]
- [[threads-tasks-and-execution]]
