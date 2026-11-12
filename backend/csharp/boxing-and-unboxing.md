# Boxing and Unboxing

Boxing converts a value-type value into a reference to a boxed object (commonly `object` or an implemented interface). The box contains a **copy** of the value. Unboxing checks the actual boxed value type and extracts its value.

```csharp
int original = 10;
object boxed = original; // boxing
original = 20;
Console.WriteLine(boxed); // 10: independent boxed copy
int restored = (int)boxed; // unboxing
// long wrong = (long)boxed; // InvalidCastException
long converted = (long)(int)boxed; // unbox int, then numeric conversion
```

Unboxing is not numeric conversion: a boxed `int` cannot be unboxed directly as `long`. Boxing may allocate and copy data; avoid accidental boxing in performance-sensitive loops when generic, strongly typed APIs are available. A boxed mutable struct can have subtler mutation behavior because modifying an unboxed copy does not modify the box. Related: [nullable-types](nullability.md), [object-members](object-members.md).
