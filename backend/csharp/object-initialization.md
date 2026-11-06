# Object Initialization in C#

> [!question] What problem do C# initialization features solve?
> They express when state can be supplied, what the caller must provide, and which constructors are responsible for establishing a valid object.

## Constructors vs. object initializers

```csharp
public class Customer
{
    public string Name { get; }
    public string City { get; init; } = "Unknown";

    public Customer(string name) => Name = name;
}

var customer = new Customer("Alex") { City = "Dallas" };
//                 constructor argument       object initializer
```

An **object initializer** assigns accessible fields/properties during construction, after the constructor runs. It does not replace the constructor. An `init` property may be assigned from an object initializer and permitted constructor/init contexts, but not ordinarily changed later.

## Property syntax

| Syntax | Meaning |
| --- | --- |
| `get; set;` | Readable and writable (subject to access modifiers) |
| `get; private set;` | Public getter, private setter (for public property) |
| `get; init;` | Assignment permitted during initialization contexts |
| `get;` | Getter-only auto-property; can be initialized in declaration or constructor |
| `required` | Caller must initialize this member unless an applicable constructor has `[SetsRequiredMembers]` |
| `=> expression` | Expression-bodied computed getter |

`required` and `init` solve different problems: **must supply a value** versus **when assignment is allowed**. Neither guarantees that the value is semantically valid.

## Primary constructors

```csharp
public class Account(string accountId)
{
    public string AccountId { get; } = accountId;
}

var account = new Account("ACC-101");
```

A primary-constructor parameter is in scope in the type's body, but for ordinary classes it **doesn't automatically create a property**. `AccountId` above is explicitly declared. A class primary constructor has no separate accessibility modifier; the containing type's accessibility and ordinary constructor rules apply.

A class with a primary constructor may also declare secondary constructors, but they must chain via `this(...)` to the primary constructor.

```csharp
public class Account(string accountId)
{
    public string AccountId { get; } = accountId;

    public Account() : this("UNKNOWN") { }
}
```

## Custom constructor + `required init`

Consider a bank account:

```csharp
public class BankAccount
{
    public required string AccountNumber { get; init; }

    public BankAccount(string accountNumber)
    {
        AccountNumber = accountNumber;
    }
}

// Compile error: the caller has not satisfied 'required'.
// var account = new BankAccount("ACC-101");
```

**Why?** The compiler doesn't normally infer from the constructor body that a `required` member has been initialized. `required` places an obligation on the **caller**.

This compiles but permits an unintended overwrite:

```csharp
var account = new BankAccount("ACC-101")
{
    AccountNumber = "ACC-202"
};
```

### `[SetsRequiredMembers]`

```csharp
using System.Diagnostics.CodeAnalysis;

public class BankAccount
{
    public required string AccountNumber { get; init; }

    [SetsRequiredMembers]
    public BankAccount(string accountNumber)
    {
        AccountNumber = accountNumber;
    }
}

var account = new BankAccount("ACC-101"); // Compiles
```

`[SetsRequiredMembers]` means: **this constructor takes responsibility for satisfying required members, so the caller needn't supply them**. The compiler trusts the declaration; it does **not** verify that all required members actually receive valid values. Nullable analysis may still warn.

> [!important] Prefer simple invariants
> If the only legitimate way to set a value is through the constructor, consider dropping `required` and using a getter-only property (`public string AccountNumber { get; }`). Use `[SetsRequiredMembers]` when it serves a real API requirement, not just to silence a compiler diagnostic.

## Primary constructor + custom constructor + `required init`

```csharp
using System.Diagnostics.CodeAnalysis;

public class TransferRequest(string accountId)
{
    public string AccountId { get; } = accountId;
    public required decimal Amount { get; init; }

    [SetsRequiredMembers]
    public TransferRequest(string accountId, decimal amount) : this(accountId)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        Amount = amount;
    }
}
```

**Path A — primary constructor and object initializer:**

```csharp
var a = new TransferRequest("ACC-101") { Amount = 500m };
```

**Path B — secondary constructor accepts responsibility:**

```csharp
var b = new TransferRequest("ACC-101", 500m);
```

**Invalid — required member omitted:**

```csharp
// Compile error:
// var c = new TransferRequest("ACC-101");
```

**Design caution:** Path A permits `Amount = 0m` because an auto-implemented `init` accessor doesn't validate the value. Path B validates it. If both paths must enforce positivity, validate inside `init` or route all creation through a validating constructor/factory.

## Validating an `init` accessor

```csharp
public class TransferRequest
{
    private decimal _amount;

    public required decimal Amount
    {
        get => _amount;
        init => _amount = value > 0
            ? value
            : throw new ArgumentOutOfRangeException(nameof(value));
    }
}

var request = new TransferRequest { Amount = 500m }; // Valid
// new TransferRequest { Amount = 0m };             // Throws at runtime
```

The `required` check is a **compile-time obligation** to assign; the `init` accessor's validation is a **runtime guard** on the supplied value.

**Cross-property invariants:** If `StartDate` must precede `EndDate`, validation within each `init` accessor can be awkward because the other property may not be initialized yet. A validating constructor or factory is often clearer.

## Important constraints

| Constraint | Why it matters |
| --- | --- |
| Custom constructor with `required` | Callers still must assign required members unless the constructor is marked `[SetsRequiredMembers]`. |
| Primary + secondary constructors | A secondary constructor on a class with a primary constructor must delegate through `this(...)`. |
| `required` vs. `init` | Required assignment versus restricted assignment timing; they are independent. |
| `[SetsRequiredMembers]` | Suppresses caller's required-member obligation; it doesn't verify correct initialization. |
| Chained annotated constructors | A constructor that chains to one marked `[SetsRequiredMembers]` must also be marked. |
| Accessibility | A required member's setter/init accessor must be at least as accessible as its containing type, and it cannot be inaccessible to its callers. |
| `where T : new()` | A type with `required` members cannot satisfy this generic constraint. |

## Code-reading checklist

1. Which values are supplied through constructor arguments, and which through an object initializer?
2. Are primary-constructor parameters stored as explicitly declared properties or just used in the body?
3. Does `required` place an obligation on the caller?
4. Does `[SetsRequiredMembers]` claim constructor responsibility?
5. Does `init` enforce actual validation, or just limit assignment timing?
6. Could different construction paths violate the same invariant?

## Related notes

- [C# index](backend.md)
- [Language fundamentals](csharp-fundamentals.md)
- [Inheritance and polymorphism](inheritance-and-polymorphism.md)
