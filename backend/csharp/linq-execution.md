# LINQ Execution: Deferred, Streaming, and Buffering

LINQ provides a common syntax for querying and transforming data. **Deferral determines when work begins; streaming versus buffering determines how work proceeds; consumer demand determines how many outputs are requested, not necessarily how many inputs are examined.**

## Lifecycle

1. Compose a logical pipeline (`Where`, `Select`, `Take`).
2. Trigger execution by enumeration or an immediate operator (`ToList`, `Count`, `Sum`, `First`).
3. Process incrementally where possible, or buffer as required.
4. Produce partial or complete results according to the operation.

```csharp
var numbers = new List<int> { 1, 2, 3 };
var query = numbers.Where(n => n > 1); // no iteration yet
numbers.Add(4);
Console.WriteLine(string.Join(",", query)); // 2,3,4
Console.WriteLine(string.Join(",", query)); // enumerates again
var snapshot = query.ToList(); // materialize now
numbers.Add(5);
Console.WriteLine(snapshot.Count); // 3
```

## Streaming is not the same as deferred

`Where` can yield matches incrementally. `OrderBy` is deferred, but typically must buffer all source elements before it can yield the first sorted result. `Take(1)` may still need to inspect many inputs to find the first match. `ToList()` executes and stores results, but does not deep-clone referenced objects.

## Comparison with Java Streams

| Concern | C# LINQ-to-Objects | Java Streams |
|---|---|---|
| Filter/map | `Where` / `Select` | `filter` / `map` |
| Limit | `Take` | `limit` |
| Materialize | `ToList`, `ToArray` | `toList`, `toArray` |
| Trigger | Enumeration or immediate operator | Terminal operation |
| Reuse | Deferred query may be re-enumerated | Stream is generally single-use |
| Source modification | Later enumeration may see changes | Modification during stream traversal is generally unsafe unless supported |

Be cautious with repeated enumeration of expensive sources, side effects, changing data, and provider-backed queries. See [[query-providers-and-iqueryable]].
