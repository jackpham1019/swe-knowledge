# Query Providers

A query provider interprets a query representation and decides how to execute it. In .NET, `IQueryProvider` and `IQueryable<T>` enable LINQ expressions to be interpreted by systems such as Entity Framework Core instead of necessarily running as in-memory delegates.

## Why a provider exists

An in-memory `Func<T,bool>` can execute a predicate against objects, but a database cannot generally inspect the original C# lambda's meaning from a compiled delegate. Providers instead receive an [expression tree](expression-trees.md) that represents supported operations as data.

## IQueryable and IQueryProvider

`IQueryable<T>` extends `IEnumerable<T>` and exposes `Expression`, `Provider`, and `ElementType`. `Queryable.Where` accepts an `Expression<Func<T,bool>>` and builds a new query expression. The provider controls translation and execution.

```csharp
IEnumerable<Transaction> inMemory = transactions;
var local = inMemory.Where(t => t.Amount > 1000m);
// Enumerable.Where takes Func<Transaction, bool>

IQueryable<Transaction> remote = db.Transactions;
var query = remote.Where(t => t.Amount > 1000m);
// Queryable.Where takes Expression<Func<Transaction, bool>>
var results = query.Select(t => t.AccountId).ToList();
```

## Execution pipeline

```text
LINQ operators → expression tree → provider (for example, EF Core)
               → supported translation → SQL → database → results
```

Illustrative translation:

```sql
SELECT AccountId FROM Transactions WHERE Amount > @p0;
```

The **provider**, not the expression tree, translates and executes the query. An expression tree is an inspectable description, not a database connection.

## Translation limits and execution boundaries

- Not every C# method or expression is translatable by a given provider.
- `IQueryable<T>` alone does not prove that work is executed remotely.
- Switching to `AsEnumerable()` changes subsequent LINQ operations to LINQ-to-Objects; earlier provider operations may still execute remotely when enumerated.
- Materialization (`ToList`, `ToArray`) executes the query and holds the returned results.
- Moving a filter to the client can transfer far more rows and increase memory use.

## Architectural boundaries

[LINQ](linq.md) expresses the query and describes deferred execution; [expression trees](expression-trees.md) preserve inspectable structure; EF Core is an ORM and provider; ADO.NET provides lower-level database access. These layers should not be conflated.

## Related topics

- [LINQ](linq.md#ienumerable-and-iqueryable) — provider-backed versus in-memory pipelines.
- [Expression trees](expression-trees.md) — representation and translation.
- [Delegates and lambdas](delegates-and-lambdas.md) — executable predicates.
