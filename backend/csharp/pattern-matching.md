# C# Pattern Matching

Patterns describe the *shape or properties of data* that qualify for a branch, often replacing chains of type tests and boolean checks.

```csharp
string Review(Payment payment) => payment switch
{
    { Amount: > 1000m, IsVerified: false } => "Manual review",
    _ => "Continue"
};
```

Equivalent conditional:

```csharp
if (payment.Amount > 1000m && !payment.IsVerified)
    return "Manual review";
return "Continue";
```

| Pattern | Example | Use |
|---|---|---|
| Type | `CardPayment card` | Match runtime type and bind variable |
| Constant | `null`, `0` | Match specific value |
| Relational | `> 1000m` | Numeric comparison |
| Property | `{ IsVerified: false }` | Match members |
| Logical | `and`, `or`, `not` | Combine patterns |
| Positional | `Payment(> 1000m, _, false)` | Match deconstructed positions |

The positional example requires a compatible `Deconstruct` method or record shape. Patterns can be used with `is`, `switch` statements, and switch expressions. Switch arms are evaluated in order, so put specific matches before general ones. Pattern syntax differs from Java's record patterns, guards, and switch expressions; do not assume feature-for-feature equivalence.

See [[null-handling-operators]] for null-specific operators.
