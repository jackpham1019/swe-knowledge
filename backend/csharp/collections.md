# Collections

Collections expose capabilities through interfaces, but an interface alone does not establish ownership, immutability, or snapshot semantics.

## Choosing a collection contract

| Interface | Exposes | Key limitation |
| --- | --- | --- |
| `IEnumerable<T>` | Enumeration | May be deferred, expensive, or side-effecting |
| `IReadOnlyCollection<T>` | Enumeration and `Count` | Source can still change |
| `IReadOnlyList<T>` | Count and indexing | Read-only access is not immutability |
| `ICollection<T>` | Mutation members | An implementation may reject mutations |
| `IList<T>` | Indexed mutation | An implementation may reject mutations |

Accept the narrowest interface needed by the consumer. For provider-backed query expressions, see [Query providers](query-providers.md); `IQueryable<T>` is not just another in-memory collection contract.

## Read-only interface versus runtime wrapper

```csharp
var items = new List<string> { "A" };
IReadOnlyList<string> exposed = items;
var wrapped = items.AsReadOnly();
items.Add("B");
Console.WriteLine(exposed.Count); // 2
Console.WriteLine(wrapped.Count); // 2
```

`IReadOnlyList<T>` prevents calling mutation members *through that interface*, but does not prevent access to the original list (or casting when the runtime object is a `List<T>`). `AsReadOnly()` returns a `ReadOnlyCollection<T>` wrapper that rejects mutation through the wrapper even when cast to its mutable collection interfaces. Both still reflect changes to the original list.

## Live views, snapshots, and immutable collections

```csharp
using System.Collections.Immutable;
var source = new List<string> { "A", "B" };
IReadOnlyList<string> contract = source;
var view = source.AsReadOnly();
var snapshot = source.ToList();
var immutable = source.ToImmutableList();

source.Add("C");
Console.WriteLine(contract.Count);  // 3
Console.WriteLine(view.Count);      // 3
Console.WriteLine(snapshot.Count);  // 2
Console.WriteLine(immutable.Count); // 2

snapshot.Add("D");                // Mutable independent copy
// view.Add("D");                 // NotSupportedException if invoked via IList<T>
var updated = immutable.Add("D"); // New immutable collection
```

| Mechanism | Live updates from source? | Mutate through exposed object? | Independent structure? |
| --- | --- | --- | --- |
| `IReadOnlyList<T>` view | Yes | Not through interface; original may be accessible | No |
| `AsReadOnly()` | Yes | No, wrapper rejects writes | No |
| `ToList()` | No | Yes | Yes |
| `ToImmutableList()` | No | No; operations produce new values | Yes |

## Shallow copying and element mutability

Structural independence is not deep immutability. A copied list of reference-type objects retains references to the same elements.

```csharp
class Item { public string Status { get; set; } = "Pending"; }
var original = new List<Item> { new Item() };
var copy = original.ToList();
copy[0].Status = "Done";
Console.WriteLine(original[0].Status); // Done
```

Choose immutable element models or explicitly copy elements when a truly independent snapshot is required. A `readonly` field holding a list also does not freeze that list; see [C# keywords](keywords.md#readonly).

## Design decisions

- Use a narrow interface when consumers only need fewer operations.
- Use `AsReadOnly()` to prevent writes through the exposed wrapper while retaining a live view.
- Use a defensive copy to isolate collection membership from later changes.
- Use immutable collections when callers should not mutate the collection structure.
- Check element mutability separately from collection mutability.

For lazy iteration and repeated enumeration, see [LINQ](linq.md).
