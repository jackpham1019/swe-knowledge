# C# Null-Handling Operators

The correct response to missing data depends on whether absence is **allowed**, **defaultable**, or **invalid**.

| Situation | Typical response |
|---|---|
| Missing data is allowed | `?.` propagates null |
| Missing data has a meaningful default | `??` supplies fallback |
| Initialize only if absent | `??=` assigns fallback |
| Missing data violates contract | Validate or throw |

```csharp
string displayName = customer?.Nickname ?? "Guest";
settings.Cache ??= new Dictionary<string, string>();
string email = customer?.Email
    ?? throw new InvalidOperationException("Receipt email is required");
```

`?.` short-circuits access when the receiver is null; `??` returns its right operand only when the left operand is null; `??=` assigns only if the left side is null. These are not replacements for domain validation. A fabricated fallback such as `"unknown@example.com"` for a required receipt email can conceal broken invariants. Nullable annotations and operators are distinct concepts: see [nullable-types](nullability.md) and [validation-boundaries](../validation-boundaries.md).
