## 1. Obsidian — Collection Interface Reference

|Interface|Capabilities|Typical use|
|---|---|---|
|`IEnumerable<T>`|Sequential enumeration|Process a sequence|
|`IReadOnlyCollection<T>`|Enumeration + `Count`|Read a collection's size|
|`IReadOnlyList<T>`|Enumeration + `Count` + indexing|Read items by position|
|`ICollection<T>`|Enumeration + `Count` + collection mutation methods|Manage collection membership|
|`IList<T>`|Indexed access and mutation methods|Modify elements by position|
|`IQueryable<T>`|Query expression representation|Compose provider-executed queries|

Important nuances to include in your note:

- `IEnumerable<T>` doesn't guarantee repeatable, inexpensive, or side-effect-free enumeration.
    
- Read-only interfaces restrict what consumers can do through that contract, not whether the underlying collection or its elements can change.
    
- `ICollection<T>` and `IList<T>` expose mutation operations, but an implementation can still reject them when read-only or fixed-size.
    
- `IQueryable<T>` extends `IEnumerable<T>`, but its defining capability is allowing a provider to interpret a query expression rather than merely enumerating values.


Obsidian: Retain that `delegate` declares a custom callable type, `Func<T, TResult>` and `Action<T>` are predefined delegate types, and compatible methods or lambdas can be converted to delegate instances.



## 3. What to retain in Obsidian

I'd organize your LINQ reference around the mechanism and its Java equivalent:

| Concern                 | C# LINQ                                                        | Java Streams                                                                                            |
| ----------------------- | -------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------- |
| Data processing         | `Where`, `Select`, `Sum`                                       | `filter`, `map`, `sum` through a primitive stream                                                       |
| Intermediate operations | `Where`, `Select`, `Take`                                      | `filter`, `map`, `limit`                                                                                |
| Evaluation trigger      | Enumeration or immediate operator                              | Terminal operation                                                                                      |
| Materialization         | `ToList()`, `ToArray()`                                        | `toList()`, `toArray()`                                                                                 |
| Reuse                   | Deferred `IEnumerable<T>` queries can be enumerated repeatedly | A Java `Stream` generally cannot be reused after consumption                                            |
| Source changes          | Later enumeration may observe changes                          | Modifying a stream source during execution is generally unsafe unless its source supports that behavior |


## Final mental model

LINQ query lifecycle

1. Define a logical pipeline

Compose deferred operators

2. Trigger evaluation

Enumerate or invoke an immediate operator

3. Execute the pipeline

Stream incrementally or buffer input as required

4. Produce requested results

Partial or complete consumption, depending on the operations

I'd retain your summary in Obsidian with one particularly useful sentence:

> Deferred execution determines when processing begins; streaming versus buffering determines how processing occurs; and consumer demand determines how many results are requested—not necessarily how much input must be examined.

## 5. What should go into Obsidian?

I'd retain the following implementation distinction rather than creating another Anki card.

```
// Executable behavior
Func<Transaction, bool> predicate =
    t => t.Amount > 1000;

// Inspectable computation
Expression<Func<Transaction, bool>> expression =
    t => t.Amount > 1000;

// LINQ-to-Objects: expects delegate
IEnumerable<Transaction> sequence = ...;
sequence.Where(t => t.Amount > 1000);

// Query provider: expects expression tree
IQueryable<Transaction> query = ...;
query.Where(t => t.Amount > 1000);
```



# C# Expression Trees & Query Providers

## 1. Expression trees vs delegates

The same lambda syntax can be converted into different representations depending on the target type.

```
using System.Linq.Expressions;

// Executable behavior
Func<Transaction, bool> predicate =
    t => t.Amount > 1000;

// Inspectable computation
Expression<Func<Transaction, bool>> expression =
    t => t.Amount > 1000;
```

- `Func<T, TResult>`: Delegate type representing executable behavior.
    
- `Expression<Func<T, TResult>>`: Expression tree representing the structure of a computation.
    
- The C# compiler performs the appropriate conversion.
    
- An existing compiled delegate cannot generally be converted back into an expression tree.
    

An expression tree can also be compiled into a delegate:

```
Func<Transaction, bool> executable =
    expression.Compile();
```

## 2. Expression trees as an intermediate representation

An expression tree represents supported C# expressions using structured nodes exposed through `System.Linq.Expressions`.

For example:

```
t => t.Amount > 1000
```

Conceptually becomes:

```
Lambda(t)
└── GreaterThan
    ├── MemberAccess(t.Amount)
    └── Constant(1000)
```

A provider can inspect nodes through a common .NET API rather than parsing C# source code.

Example:

```
var body = (BinaryExpression)expression.Body;

Console.WriteLine(body.NodeType); // GreaterThan
Console.WriteLine(body.Left);     // t.Amount
Console.WriteLine(body.Right);    // 1000
```

Common node types:

|Type|Represents|
|---|---|
|`ParameterExpression`|Parameter or variable|
|`ConstantExpression`|Constant value|
|`MemberExpression`|Property or field access|
|`BinaryExpression`|Binary operation|
|`MethodCallExpression`|Method invocation|
|`LambdaExpression`|Lambda with parameters and a body|

## 3. `IEnumerable<T>` vs `IQueryable<T>`

```
// LINQ-to-Objects
IEnumerable<Transaction> sequence = transactions;

var a = sequence.Where(t => t.Amount > 1000);
// Enumerable.Where accepts Func<Transaction, bool>
```

```
// Provider-backed query
IQueryable<Transaction> query = db.Transactions;

var b = query.Where(t => t.Amount > 1000);
// Queryable.Where accepts Expression<Func<Transaction, bool>>
```

`IQueryable<T>` extends `IEnumerable<T>` and exposes:

- `Expression`: The query representation.
    
- `Provider`: The provider responsible for interpreting or executing the query.
    
- `ElementType`: The element type produced.
    

`IQueryable<T>` does **not** convert compiled delegates back into expression trees.

## 4. Query-provider execution

Example:

```
var results = db.Transactions
    .Where(t => t.Amount > 1000)
    .Select(t => t.AccountId)
    .ToList();
```

Conceptual flow:

```
C# LINQ query
    ↓
Expression tree
    ↓
Query provider (e.g., EF Core)
    ↓
Translate supported operations
    ↓
Database-specific SQL
    ↓
Database execution
    ↓
Results returned to application
```

Example translation:

```
SELECT AccountId
FROM Transactions
WHERE Amount > @p0;
```

The provider interprets the expression tree. The tree itself does not translate SQL or communicate with the database.

### Limitations

- Expression trees represent a supported subset of C# expressions.
    
- A provider may be unable to translate some valid expression-tree nodes.
    
- A common LINQ syntax does not guarantee identical execution behavior across providers.
    

## 5. Connection to database infrastructure

|Component|Responsibility|
|---|---|
|LINQ|Express declarative queries|
|Expression trees|Represent inspectable computations|
|`IQueryable<T>`|Expose the query representation and provider|
|EF Core|ORM, query translation, and database integration|
|ADO.NET|Database communication|

**Learning boundary:** EF Core configuration, database providers, and ADO.NET belong to Part 2 — .NET Platform Fundamentals.

## 6. Related concepts

- [[C# Delegates and Lambdas]]
    
- [[C# LINQ]]
    
- [[C# Collection Interfaces]]
    

The central architectural pattern is **intermediate representation**: separate the representation of a computation from the systems that interpret or execute it.


## 5. The deeper architectural principle

This connects to the broader idea of information preservation.

If another component needs to inspect or transform a computation, preserve its structure before reducing it to an executable representation.

The same principle appears in compilers, query planners, serialization, and code-generation tools.

For your Obsidian note, I'd retain:

> Expression trees can be created and manipulated at runtime. What C# doesn't generally support is reconstructing the original expression tree from a compiled delegate. Doing so would require decompilation and analysis of executable code, whereas expression trees preserve the relevant structure directly.


using System.Collections.Immutable;

var original = new List<string> { "A", "B" };

// 1. Read-only wrapper: live view, mutation rejected
var view = original.AsReadOnly();

// 2. Defensive copy: independent, mutable list
var snapshot = original.ToList();

// 3. Immutable collection: independent structure
var immutable = original.ToImmutableList();

original.Add("C");

Console.WriteLine(view.Count);      // 3
Console.WriteLine(snapshot.Count);  // 2
Console.WriteLine(immutable.Count); // 2

snapshot.Add("D");                  // Allowed
// view.Add("D");                   // NotSupportedException
var updated = immutable.Add("D");   // New immutable list



Traditional branching:

```
if (payment.Amount > 1000 &&
    !payment.IsVerified)
{
    return "Manual review";
}

return "Continue";
```

Using a property pattern:

```
return payment switch
{
    { Amount: > 1000, IsVerified: false }
        => "Manual review",

    _ => "Continue"
};
```

This pattern combines several conditions into a description of the data that qualifies.

C# supports several useful pattern categories:

|Pattern|Example|Purpose|
|---|---|---|
|Type|`CardPayment card`|Match runtime type and capture|
|Constant|`0`, `null`|Match a specific value|
|Relational|`> 1000`|Match a range condition|
|Property|`{ IsVerified: false }`|Inspect named properties|
|Logical|`and`, `or`, `not`|Combine patterns|
|Positional|`Payment(> 1000, _, false)`|Match positional components|

The positional example assumes `Payment` is the record declared above.

The most notable differences are in the pattern vocabulary and language integration.

|Capability|C#|Modern Java|
|---|---|---|
|Type patterns|Yes|Yes|
|Switch expressions|Yes|Yes|
|Property patterns|Yes, directly|Typically use guards or deconstruction|
|Relational patterns|`> 1000`|Typically use guarded patterns or conditions|
|Logical patterns|`and`, `or`, `not`|Different syntax and restrictions|
|Positional patterns|Deconstruction|Record patterns|


Section 4.2 — Null-Handling Operators (`?.`, `??`, `??=`)
## 1. The distinction is between optional and required data

Consider three situations:

|Situation|Appropriate behavior|
|---|---|
|Missing value is acceptable|Use `?.` to propagate null|
|Missing value has a meaningful default|Use `??` to supply a fallback|
|Missing value violates a contract|Reject it through validation or an exception|
For example, a customer's optional display name:

```
string displayName = customer?.Nickname ?? "Guest";
```

This is reasonable if the application explicitly permits an anonymous or unnamed customer.

But an email address required for sending a receipt:

```
string email = customer?.Email ?? "unknown@example.com";
```

# C# Async/Await — Task Execution and Exception Handling

## Core Mental Model

`Task<T>` represents an operation that may eventually produce a result, fail, or be canceled.

- `async` enables `await` and supports asynchronous method execution.
    
- `await` is a **conditional suspension point**: if the task is incomplete, the method suspends without blocking the executing thread.
    
- The compiler-generated state machine preserves the method's execution state, including local variables and exception-handling context.
    
- When the task completes, execution resumes, potentially on a different thread.
    
- If the task is already complete, `await` can continue synchronously without suspension.
    

**Key distinction:** A suspended method remains logically incomplete, but is not actively executing.

## Returning a Task vs Awaiting It

### 1. Direct Task Forwarding

```
public Task<Account> GetAccountAsync(int id)
{
    return accountClient.GetAccountAsync(id);
}
```

The method initiates the operation and returns its task directly.

- No local suspension or continuation is needed.
    
- The method's execution ends when it returns the task.
    
- The caller can await the task to observe its eventual result or failure.
    

Prefer this when the method simply delegates the operation and requires no additional processing.

### 2. Awaiting Before Returning

```
public async Task<Account> GetAccountAsync(int id)
{
    Account account = await accountClient.GetAccountAsync(id);

    Console.WriteLine("Account retrieved");

    return account;
}
```

If the task is incomplete, the method suspends at `await`.

After the operation completes, execution resumes and the remaining statements run.

`**return await operation**` **is conceptually equivalent to awaiting the result and then returning that result.**

## Exception Handling Difference

### Direct Return

```
public Task<Account> GetAccountAsync(int id)
{
    try
    {
        return accountClient.GetAccountAsync(id);
    }
    catch (HttpRequestException)
    {
        Console.WriteLine("Request failed");
        throw;
    }
}
```

The `try/catch` covers only the synchronous invocation.

If the call successfully returns a task that faults later, the local `catch` does not execute.

However, an exception thrown synchronously before the task is returned can still be caught.

### Await Inside Try/Catch

```
public async Task<Account> GetAccountAsync(int id)
{
    try
    {
        Account account =
            await accountClient.GetAccountAsync(id);

        return account;
    }
    catch (HttpRequestException)
    {
        Console.WriteLine("Request failed");
        throw;
    }
}
```

The `await` propagates a fault from the awaited task into the method's logical execution flow.

Because the async state machine preserves the exception-handling context, the local `catch` can handle the failure even after suspension.

**Decision rule:** Use `await` when the method needs to process the result, handle asynchronous exceptions locally, or perform additional work after completion. Otherwise, direct task forwarding is often sufficient.

## Asynchrony vs Parallelism

- **Concurrency:** Multiple operations can make progress during overlapping periods.
    
- **Parallelism:** Multiple operations execute simultaneously.
    
- **Asynchronous I/O:** Allows threads to perform other work while external operations are pending.
    

Async I/O improves thread utilization and scalability, but does not inherently accelerate database queries or guarantee parallel execution.

## Key Takeaways

- `await` requires an `async` context, but an `async` method does not strictly require `await`.
    
- `Task<T>` is not a dedicated thread.
    
- `await` suspends the asynchronous method, not the executing thread.
    
- Directly returning a task does not preserve the method's local `try/catch` for later task failures.
    
- Awaiting inside `try/catch` allows the method to handle those failures.
    
- Use `async`/`await` when its continuation semantics provide a meaningful benefit.