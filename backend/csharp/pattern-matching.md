# Pattern Matching

Pattern matching expresses conditions on a value's type, structure, or contents. Use it when it clarifies branching compared with nested type checks and boolean expressions.

## Common patterns

| Pattern | Example | Purpose |
| --- | --- | --- |
| Type | `CardPayment card` | Check runtime type and bind a variable |
| Constant | `null`, `0` | Match a specific value |
| Relational | `> 1000m` | Compare against a threshold |
| Property | `{ IsVerified: false }` | Match member values |
| Logical | `and`, `or`, `not` | Combine conditions |
| Positional | `Payment(> 1000m, _, false)` | Match deconstructed positions |

Positional patterns require compatible `Deconstruct` support or a record with an appropriate positional shape.

## `is` and `switch` expressions

```csharp
string Review(Payment? payment) => payment switch
{
    null => "Missing payment",
    { Amount: > 1000m, IsVerified: false } => "Manual review",
    _ => "Continue"
};
```

The equivalent conditional logic can be easier for very simple cases:

```csharp
if (payment is null) return "Missing payment";
if (payment.Amount > 1000m && !payment.IsVerified)
    return "Manual review";
return "Continue";
```

Switch arms are considered in order: place specific patterns before broad catch-all patterns. `switch` expressions must handle all possible inputs or throw at runtime for unmatched values. Pattern syntax and exhaustiveness behavior are not identical to Java's switch and record patterns.

## Pattern matching versus null-handling operators

`is null` tests a condition; `?.` propagates a missing receiver and `??` supplies a fallback. Use patterns for branching on shape or state and [null-handling operators](null-handling-operators.md) for null-specific access/defaulting. See [Nullability](nullability.md) for compile-time null analysis.
