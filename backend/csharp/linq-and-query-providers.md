# C# LINQ and Query Providers

## Execution model

LINQ expresses operations such as `Where`, `Select`, and `Take` as a pipeline. Many sequence operations defer work until enumeration. Operators such as `ToList`, `Count`, and `Sum` cause evaluation (with different consumption behavior). Some operators stream results; others buffer input.

> Deferred execution determines **when** processing starts; streaming versus buffering determines **how** processing occurs; and consumer demand determines how much output is requested, not necessarily how much input must be examined.

Unlike a Java Stream, a deferred `IEnumerable<T>` pipeline can generally be enumerated again, possibly observing a changed source. Repeated enumeration may repeat work or side effects.

| Concern | C# LINQ | Java Streams |
| --- | --- | --- |
| Filter/map | `Where`, `Select` | `filter`, `map` |
| Limit | `Take` | `limit` |
| Materialize | `ToList`, `ToArray` | `toList`, `toArray` |
| Reuse | Deferred sequence often re-enumerable | Stream generally single-use |

## Delegates versus expression trees

```csharp
using System.Linq.Expressions;

Func<Transaction, bool> predicate = t => t.Amount > 1000;
Expression<Func<Transaction, bool>> expression = t => t.Amount > 1000;

IEnumerable<Transaction> sequence = transactions;
var inMemory = sequence.Where(predicate); // Executable delegate

IQueryable<Transaction> query = db.Transactions;
var providerQuery = query.Where(expression); // Inspectable expression tree
```

`delegate` declares a callable type; `Func<...>` and `Action<...>` are predefined delegate types. Compatible methods and lambdas can be converted to delegates.

An expression tree preserves supported computation structure (such as parameter, member access, constant, comparison, and method call nodes) for inspection or translation. `expression.Compile()` produces an executable delegate, but an existing compiled delegate cannot generally be reconstructed as the original expression tree.

## Query-provider boundary

`IQueryable<T>` extends `IEnumerable<T>` and exposes `Expression`, `Provider`, and `ElementType`. A provider such as EF Core can interpret supported expression nodes and translate them into SQL. The expression tree itself does not communicate with the database. Not every valid expression is translatable, and changing from `IQueryable<T>` to `IEnumerable<T>` may move work into application memory.

```text
LINQ expression -> expression tree -> provider (e.g., EF Core)
                -> translated SQL -> database -> results
```

**Architectural principle:** preserve a computation's structure when another component needs to inspect or transform it; don't reduce it prematurely to executable behavior.

Related: [[collections-and-immutability]]. EF Core and ADO.NET details belong under the .NET platform notes.
