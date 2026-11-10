# C# Collection Interfaces

Collection interfaces describe **capabilities available through a contract**, not guarantees about implementation, mutability, performance, or ownership.

| Interface | Exposes | Appropriate when |
|---|---|---|
| `IEnumerable<T>` | Enumeration | Consumer only needs to iterate |
| `IReadOnlyCollection<T>` | Enumeration and `Count` | Consumer needs size, not indexing |
| `IReadOnlyList<T>` | Enumeration, count, indexing | Consumer needs positional reads |
| `ICollection<T>` | Count and collection mutation API | Consumer manages membership |
| `IList<T>` | Indexing and indexed mutation API | Consumer needs position-based edits |
| `IQueryable<T>` | Expression and query provider | Query must be interpreted by a provider |

## Important limitations

- `IEnumerable<T>` does not promise cheap, repeatable, side-effect-free enumeration. An iterator can run code or perform I/O each time it is enumerated.
- `IReadOnlyList<T>` restricts operations available **through that reference**; the underlying `List<T>` may still change, and mutable elements can still be modified.
- `ICollection<T>` and `IList<T>` declare mutation members, but implementations can reject operations (for example, read-only or fixed-size implementations).
- `IQueryable<T>` inherits `IEnumerable<T>` but adds an inspectable expression and a provider. It does not promise server-side execution or successful translation.

```csharp
List<string> names = new() { "A" };
IReadOnlyList<string> exposed = names;
names.Add("B");
Console.WriteLine(exposed.Count); // 2: not a snapshot
```

## Design decision

Accept the narrowest interface your method needs. For public APIs, do not confuse an interface that *does not expose mutation* with a guarantee that no one can mutate the underlying state. For actual isolation see [[collection-views-snapshots-and-immutability]]. For provider-backed queries see [[query-providers-and-iqueryable]].
