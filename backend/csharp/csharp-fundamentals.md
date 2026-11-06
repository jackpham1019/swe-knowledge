# C# Language Fundamentals

A practical reference for differences from Java and common C# syntax. This consolidates material from the original `C#.md` note.

## Generic constraints

Constraints specify the kinds of type arguments a generic type or method can accept.

| Constraint | Meaning |
| --- | --- |
| `where T : class` | Reference type (nullable annotations can refine intent) |
| `where T : struct` | Non-nullable value type |
| `where T : notnull` | Non-nullable reference or value type (nullable warnings depend on context) |
| `where T : new()` | Public parameterless constructor |
| `where T : BaseClass` | Inherits from the named base class |
| `where T : IInterface` | Implements the named interface |
| `where T : unmanaged` | Unmanaged value type |

```csharp
public static T Create<T>() where T : new() => new T();
```

## Properties and initialization

| Syntax              | Meaning                                                                                                                           |
| ------------------- | --------------------------------------------------------------------------------------------------------------------------------- |
| `get; set;`         | Publicly readable and writable property (when property is public)                                                                 |
| `get; private set;` | Public read, private write                                                                                                        |
| `get; init;`        | Can be assigned during allowed initialization contexts; not normally reassigned later                                             |
| `get;`              | Getter-only property; can be assigned from a constructor if auto-implemented                                                      |
| `=> expression`     | Expression-bodied computed getter                                                                                                 |
| `required`          | Caller must initialize member in a supported object-creation expression, unless constructor is marked as setting required members |

```csharp
public class Payment
{
    public required string Currency { get; init; }
    public decimal Amount { get; init; }
    public decimal WithFee => Amount * 1.02m;
}

var payment = new Payment { Currency = "USD", Amount = 100m };
```

**Design note:** `required` enforces initialization at compile time, not semantic validity. For invariants across multiple members (for example, `StartDate < EndDate`), prefer a validating constructor or factory.

## Class, struct, and record

| Type | Assignment semantics | Default equality | Common use |
| --- | --- | --- | --- |
| `class` | Copies a reference | Reference identity, unless overridden | Entities and services |
| `struct` | Copies the value | Value-based (`ValueType` equality, unless overridden) | Small value types |
| `record class` | Copies a reference | Generated value-based equality | DTOs and immutable-style models |
| `record struct` | Copies the value | Generated value-based equality | Value objects |

A positional `record` is a `record class` by default.

```csharp
public record Position(int X, int Y);
var first = new Position(10, 20);
var equal = new Position(10, 20);
Console.WriteLine(first == equal); // True
var moved = first with { X = 15 };
```

**Important:** A struct copy is shallow when it contains references; both copies can still point at the same mutable object.

## Resource management

Garbage collection reclaims managed memory; `IDisposable` and `using` handle deterministic cleanup of disposable resources.

```csharp
using var stream = File.OpenRead("data.txt");
// stream.Dispose() runs when the containing scope exits, including exceptions.
```

Java analogy: `AutoCloseable` and `try-with-resources`. Disposing an object does not directly reclaim its managed memory.

## Extension methods

An extension method is a static method callable with instance-like syntax. It does not modify the extended type.

```csharp
public static class PaymentExtensions
{
    public static string FormatAmount(this decimal amount) => $"${amount:N2}";
}

string display = 12.5m.FormatAmount();
```

Conceptual translation: `amount.FormatAmount()` is resolved to a static method call such as `PaymentExtensions.FormatAmount(amount)`.

For a runnable-style practice example, see [extension-method-example.cs](examples/extension-method-example.cs).

## Further study

- Interfaces, abstract classes, accessibility, and static abstract interface members.
- LINQ and how extension methods enable fluent query operations.
- `required`, `init`, constructors, object initializers, and immutable designs.
- Equality, copying, and reference fields inside structs.

## Related topics

- [C# hub](csharp.md)
- [.NET hub](dotnet.md)
