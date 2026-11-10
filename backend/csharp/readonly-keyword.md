# C# readonly: Fields, Structs, and Members

The `readonly` keyword has several distinct uses. It does **not** universally make an object graph immutable.

## Readonly field

```csharp
class Cart
{
    private readonly List<string> _items = new();
    public void Add(string item) => _items.Add(item); // allowed
    // _items = new List<string>(); // not allowed here
}
```

A `readonly` field generally cannot be reassigned outside its declaration or the relevant constructor, but a referenced object can still mutate.

## Readonly struct

```csharp
public readonly struct Point
{
    public int X { get; }
    public int Y { get; }
    public Point(int x, int y) => (X, Y) = (x, y);
}
```

A `readonly struct` prevents instance state from being mutated through its members and requires instance fields to be readonly; it is useful for value objects. It does not deeply freeze objects referenced by its fields.

## Readonly struct member

```csharp
public struct Counter
{
    public int Value;
    public readonly int Read() => Value;
}
```

A `readonly` instance member promises not to mutate that struct instance. This can also help avoid defensive copies in certain readonly contexts. Compare `readonly` to `init` (initialization-only property setter) and immutable collections; they address different mutation boundaries. Related: [[collection-views-snapshots-and-immutability]].
