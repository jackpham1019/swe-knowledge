# System.Object: Common Members

Every C# type ultimately derives from `System.Object` (alias `object`), though value types have special value semantics and may be boxed when treated as objects.

| Member | Role | Caveat |
|---|---|---|
| `ToString()` | Human-readable representation | Override for useful diagnostics; not a stable serialization format |
| `Equals(object?)` | Equality comparison | Default reference equality for ordinary classes; value types and records differ |
| `GetHashCode()` | Hash-based collection support | Equal values must have equal hash codes; hash not persistent ID |
| `GetType()` | Actual runtime type | Nonvirtual runtime type inspection |

```csharp
object value = "hello";
Console.WriteLine(value.GetType() == typeof(string)); // True
Console.WriteLine(value.ToString()); // hello
```

When implementing value equality, override `Equals` and `GetHashCode` consistently, and consider `IEquatable<T>`. Hash codes are not guaranteed stable across processes or framework versions. Avoid mutable equality keys in dictionaries/sets. Related: [[boxing-and-unboxing]].
