# Collection Views, Snapshots, and Immutability

**Read-only interface**, **read-only wrapper**, **defensive copy**, and **immutable collection** solve different problems.

```csharp
using System.Collections.Immutable;
var original = new List<string> { "A", "B" };
IReadOnlyList<string> contract = original;
var view = original.AsReadOnly();
var snapshot = original.ToList();
var immutable = original.ToImmutableList();
original.Add("C");
Console.WriteLine(contract.Count);  // 3
Console.WriteLine(view.Count);      // 3
Console.WriteLine(snapshot.Count);  // 2
Console.WriteLine(immutable.Count); // 2
snapshot.Add("D");                // permitted
// view.Add("D");                 // throws NotSupportedException
var updated = immutable.Add("D"); // returns a new immutable list
```

| Technique | Rejects mutation via exposed object? | Reflects changes to source? | Independent structure? |
|---|---|---|---|
| `IReadOnlyList<T>` reference | No runtime guarantee: original may be cast/reached | Yes | No |
| `AsReadOnly()` wrapper | Yes, via wrapper | Yes | No |
| `ToList()` defensive copy | No: copy is mutable | No structural updates | Yes |
| `ToImmutableList()` | Yes | No structural updates | Yes |

A copy of a collection of class instances is **shallow**: it copies references, not the objects they point to. Immutable collections likewise do not automatically freeze mutable elements. Choose based on whether you need a restricted API, live view, independent snapshot, or persistent immutable structure. Related: [[collection-interfaces]].
