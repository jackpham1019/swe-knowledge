# C# Collections and Immutability

## Choosing a collection contract

| Interface | Exposes | Caveat |
| --- | --- | --- |
| `IEnumerable<T>` | Enumeration | May be deferred, expensive, or side-effecting |
| `IReadOnlyCollection<T>` | Enumeration and `Count` | Underlying collection can change |
| `IReadOnlyList<T>` | Enumeration, `Count`, indexing | Read-only contract, not necessarily immutable |
| `ICollection<T>` | Collection mutation API | Implementation may reject mutation |
| `IList<T>` | Indexed mutation API | Implementation may reject mutation |
| `IQueryable<T>` | Query expression and provider | Execution depends on provider; see [[linq-and-query-providers]] |

A read-only interface restricts operations through the interface; it does not guarantee an immutable object, and consumers may be able to cast to the mutable implementation.

## Live read-only wrapper versus snapshot

```csharp
using System.Collections.Immutable;

var original = new List<string> { "A", "B" };
var view = original.AsReadOnly();       // Live wrapper, rejects mutation via wrapper
var snapshot = original.ToList();       // Independent mutable list
var immutable = original.ToImmutableList(); // Immutable collection structure

original.Add("C");
Console.WriteLine(view.Count);      // 3
Console.WriteLine(snapshot.Count);  // 2
Console.WriteLine(immutable.Count); // 2
var updated = immutable.Add("D");  // New immutable collection
```

These are shallow boundaries: mutable objects held as elements may still change. Choose an immutable model or copy element objects when deep independence is required.
