# C# Nullability and Boxing

## Nullable value types versus nullable references

`Nullable<T>` (written `T?` for a non-nullable value type) represents a value of type `T` or the absence of a value. For example, `int? count = null;` is shorthand for `Nullable<int>`.

Nullable reference annotations, such as `string?`, are different: they normally do not change the runtime representation of a reference. With nullable analysis enabled, the compiler uses annotations and flow analysis to warn about possible null misuse; runtime validation is still necessary at trust boundaries.

```csharp
int? count = null;
Console.WriteLine(count.HasValue); // false

string? name = GetOptionalName();
string display = name ?? "Guest";
```

## Boxing and unboxing

Boxing copies a value type into an object that can be treated as `object` or a compatible interface. Unboxing requires a compatible underlying boxed value type; it is not a numeric conversion.

```csharp
object boxed = 42;             // Box int
int value = (int)boxed;        // Unbox int
long widened = (long)(int)boxed; // Unbox, then convert
// long invalid = (long)boxed; // InvalidCastException
```

Boxing may allocate and copy a value. Prefer generics when possible to avoid unnecessary boxing in hot paths.

## Related concepts

See [[csharp-fundamentals]] for type semantics and [[object-initialization]] for compile-time initialization contracts.
