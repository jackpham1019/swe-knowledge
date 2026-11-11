# C# Language

The language itself: types, syntax, equality, copying, generics, and resource management. Keep language-specific notes here, not ASP.NET Core configuration or framework behavior.

## Notes

- [C# Language Fundamentals](csharp-fundamentals.md) — generic constraints, properties, records, `using`, and extension methods
- [Extension method example](examples/extension-method-example.cs) — a small executable example
- [Examples index](csharp-example.md)

## Related

- [.NET platform](dotnet.md)
- [ASP.NET Core configuration practice](../dotnet/aspnet-core/configuration-reading-practice.md)
- [Backend index](backend.md)

## Consolidated learning references

- [[nullability-and-boxing]] — nullable types and boxing semantics
- [[collections-and-immutability]] — collection contracts, wrappers, and snapshots
- [[linq-and-query-providers]] — deferred execution, delegates, expression trees, query providers
- [[exceptions-and-resource-lifetimes]] — exception propagation, `IDisposable`, ownership
- [[pattern-matching-and-null-handling]] — patterns and null-related operators

The `readonly` keyword has different meanings for fields, structs, and struct members; track this as a follow-up rather than treating it as one interchangeable rule.

## Detailed reference notes

- [[nullable-types]], [[boxing-and-unboxing]], [[object-members]]
- [[collection-interfaces]], [[collection-views-snapshots-and-immutability]]
- [[delegates-and-lambdas]], [[linq-execution]], [[expression-trees]], [[query-providers-and-iqueryable]]
- [[pattern-matching]], [[null-handling-operators]]
- [[exception-propagation]], [[resource-ownership-and-disposal]], [[async-await-task-flow]], [[readonly-keyword]]

## Asynchronous execution and recovery

- [[threads-tasks-and-execution]] — Thread pool, tasks, and execution resources
- [[io-bound-vs-cpu-bound-work]] — Choosing asynchronous I/O or CPU offloading
- [[concurrency-asynchrony-and-parallelism]] — Distinct execution concepts
- [[task-concurrency-and-whenall]] — Independent operations and coordination
- [[cancellation-tokens-and-propagation]] — Cooperative cancellation and ownership
- [[linked-cancellation-and-timeouts]] — Combining caller cancellation and deadlines
- [[cancellation-boundaries-and-business-consistency]] — Safe stopping points
- [[cancellation-and-transaction-recovery]] — Rollback and recovery scope
- [[compensation-and-sagas]] — Consistency across independently committed work
