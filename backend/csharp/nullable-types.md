# Nullable Value Types and Nullable Reference Types

`T?` has different meaning depending on whether `T` is a value type or reference type.

## Value types

```csharp
int? quantity = null; // Nullable<int>
quantity = 5;
Console.WriteLine(quantity.HasValue); // True
Console.WriteLine(quantity.Value);    // 5
```

`Nullable<T>` wraps a non-nullable value type with a `HasValue` flag and stored `Value`. Reading `.Value` when absent throws `InvalidOperationException`.

## Reference types

```csharp
#nullable enable
string? nickname = null;
string requiredName = "Ada";
```

For ordinary reference types, `?` is primarily a compiler-analysis annotation, not a new runtime wrapper. With nullable reference analysis enabled, the compiler tracks possible null flows and warns about unsafe dereferences; warnings are not runtime enforcement. Code from unannotated libraries or reflection can still return null despite non-null annotations.

## Design decision

Express optionality in the type contract, but validate required values at runtime at trust boundaries. `!` suppresses a nullable warning; it does not perform a runtime null check. Related: [[null-handling-operators]], [[boxing-and-unboxing]].
