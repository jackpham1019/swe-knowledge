# LINQ

LINQ (Language Integrated Query) provides composable operators for querying and transforming sequences and provider-backed data. Its execution behavior depends on the operator and whether the source is an `IEnumerable<T>` or `IQueryable<T>`.

## Building a query

`Where` filters, `Select` projects, and `Take` limits output. Query syntax and method syntax are alternative ways to express many operations; query syntax is translated by the compiler into method calls.

```csharp
var adults = people.Where(p => p.Age >= 18)
                   .Select(p => p.Name);
```

## Deferred execution

Many LINQ operators construct a pipeline without immediately reading the source. Enumeration (such as `foreach`) triggers evaluation; immediate operators such as `Count()`, `First()`, `ToList()` and `Sum()` execute work when called.

```csharp
var numbers = new List<int> { 1, 2, 3 };
var query = numbers.Where(n => n > 1); // no iteration yet
numbers.Add(4);
Console.WriteLine(string.Join(",", query)); // 2,3,4
Console.WriteLine(string.Join(",", query)); // enumerates again
var snapshot = query.ToList(); // materialize now
numbers.Add(5);
Console.WriteLine(snapshot.Count); // 3
```

Deferred queries can observe changes to the source on later enumeration. Re-enumerating may repeat expensive work, database calls, or side effects. A materialized list stores results at that moment but does not deep-copy referenced objects.

## Streaming versus buffering

**Deferred** describes *when* processing begins; **streaming/buffering** describes *how* elements are processed. `Where` can yield matches incrementally, while `OrderBy` must generally buffer all input before yielding the first sorted element. `Take(1)` limits output but might still inspect many input elements.

## IEnumerable and IQueryable

For `IEnumerable<T>`, LINQ-to-Objects operators typically accept executable delegates (`Func<T,bool>`). For `IQueryable<T>`, `Queryable` operators generally build expression trees that a [query provider](query-providers.md) can interpret and translate. See [Expression trees](expression-trees.md) and [Delegates and lambdas](delegates-and-lambdas.md).

```csharp
IEnumerable<Transaction> local = transactions;
var filteredLocal = local.Where(t => t.Amount > 1000m);

IQueryable<Transaction> remote = db.Transactions;
var filteredRemote = remote.Where(t => t.Amount > 1000m);
```

Do not assume an `IQueryable<T>` executes remotely or that all expressions can be translated. Switching to LINQ-to-Objects too early can move filtering to application memory.

## Comparison with Java Streams

| Concern | C# LINQ-to-Objects | Java Streams |
| --- | --- | --- |
| Filter/map | `Where` / `Select` | `filter` / `map` |
| Limit | `Take` | `limit` |
| Materialize | `ToList`, `ToArray` | `toList`, `toArray` |
| Trigger | Enumeration or immediate operator | Terminal operation |
| Reuse | Deferred query can generally be re-enumerated | Stream is generally single-use |
| Source mutation | Later enumeration may observe changes | Concurrent modification during traversal is generally unsafe unless supported |

## Practical failure modes

- Multiple enumeration can repeat I/O or expensive transformations.
- Side effects in predicates or projections can run multiple times.
- A query can yield stale or different results when the underlying data changes between enumerations.
- Buffering operators can consume substantial memory.
- Provider-backed queries may fail translation or unexpectedly execute locally after a boundary change.

## Related topics

- [Query providers](query-providers.md) — translation and execution boundaries.
- [Expression trees](expression-trees.md) — inspectable computation structure.
- [Collections](collections.md) — sequence interfaces, snapshots, and mutability.
