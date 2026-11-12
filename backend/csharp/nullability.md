# Nullability

Nullability expresses whether the absence of a value is valid. In C#, `T?` has different mechanisms for value types and reference types.

## Nullable value types: `Nullable<T>`

```csharp
int? quantity = null; // Nullable<int>
Console.WriteLine(quantity.HasValue); // False
quantity = 5;
Console.WriteLine(quantity.Value);    // 5
```

A nullable value type stores a `HasValue` flag and the underlying value. Accessing `.Value` when no value exists throws `InvalidOperationException`.

## Nullable reference types: compiler flow analysis

```csharp
#nullable enable
string? nickname = GetOptionalName();
string display = nickname ?? "Guest";
```

For ordinary reference types, `?` is an annotation used by compiler analysis rather than a new runtime wrapper. With nullable analysis enabled, the compiler warns about potentially unsafe dereferences. Annotations do not enforce non-null values at runtime; reflection, legacy code, or external data may violate them.

## Null-forgiving operator and validation boundaries

`value!` suppresses a nullable warning but performs no runtime check. Use it only when the non-null invariant is established independently. Validate required data from HTTP, files, databases, or other trust boundaries at runtime.

```csharp
string email = request.Email
    ?? throw new ArgumentException("Email is required");
```

For `?.`, `??`, and `??=`, see [Null-handling operators](null-handling-operators.md). For type semantics and allocation costs, see [Boxing and unboxing](boxing-and-unboxing.md).
