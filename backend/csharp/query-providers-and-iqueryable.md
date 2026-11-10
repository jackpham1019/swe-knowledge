# IQueryable and Query Providers

`IQueryable<T>` extends `IEnumerable<T>` and exposes `Expression`, `Provider`, and `ElementType`. It allows a provider to interpret a query rather than necessarily executing the whole pipeline in application memory.

```csharp
IEnumerable<Transaction> inMemory = transactions;
var local = inMemory.Where(t => t.Amount > 1000m);
// Enumerable.Where takes Func<Transaction,bool>

IQueryable<Transaction> remote = db.Transactions;
var query = remote.Where(t => t.Amount > 1000m);
// Queryable.Where takes Expression<Func<Transaction,bool>>
var results = query.Select(t => t.AccountId).ToList();
```

## Conceptual execution path

```text
C# LINQ expression
  → expression tree
  → provider (e.g., EF Core)
  → translation of supported operations
  → database-specific SQL
  → database execution
  → result materialization
```

An illustrative SQL translation:

```sql
SELECT AccountId FROM Transactions WHERE Amount > @p0;
```

The **provider**, not the expression tree itself, translates and executes queries. An `IQueryable<T>` is not proof that a query will execute remotely, and not all expressions can be translated. Switching from provider-backed queries to LINQ-to-Objects can move filtering to the application, potentially fetching far more rows.

## Boundaries

LINQ expresses the query; expression trees describe it; EF Core is an ORM/query provider; ADO.NET handles lower-level database access. EF Core and database-provider configuration belong to .NET platform learning. Related: [[expression-trees]], [[linq-execution]], [[collection-interfaces]].
