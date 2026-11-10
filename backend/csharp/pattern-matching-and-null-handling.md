# C# Pattern Matching and Null Handling

## Pattern matching

Patterns express conditions on a value's shape or contents. C# supports type, constant, relational, property, positional, and logical patterns.

```csharp
string decision = payment switch
{
    { Amount: > 1000, IsVerified: false } => "Manual review",
    null => "Missing payment",
    _ => "Continue"
};
```

Use patterns when they make branching easier to read, not merely to replace simple `if` statements. Positional patterns require suitable `Deconstruct` support.

## Null-handling operators and contracts

| Situation | Choice |
| --- | --- |
| Missing data is acceptable | `?.` to propagate null |
| Missing data has a valid default | `??` to supply fallback |
| Assign only when null | `??=` |
| Missing data violates a contract | Validate and reject explicitly |

```csharp
string displayName = customer?.Nickname ?? "Guest";
string email = customer?.Email
    ?? throw new InvalidOperationException("Receipt email required");
```

Do not hide missing required data behind invented defaults. See [[nullability-and-boxing]] for compile-time nullable analysis.
