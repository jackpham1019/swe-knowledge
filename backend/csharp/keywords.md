# C# Keywords and Modifiers

Keywords and contextual keywords control different language contracts. Group them by the decision they affect rather than treating them as interchangeable forms of immutability or visibility.

## Mutation and initialization

| Keyword | Applies to | Contract |
| --- | --- | --- |
| `readonly` | Fields, structs, struct instance members | Restricts reassignment or mutation of the declared receiver, not the entire object graph |
| `const` | Compile-time constant fields/local variables | Value must be a compile-time constant |
| `init` | Property/indexer accessor | Assignment is limited to initialization contexts |
| `required` | Fields/properties | Callers must initialize required members unless a constructor is marked as satisfying them |

### `readonly`

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

A `readonly` instance member promises not to mutate that struct instance. This can also help avoid defensive copies in certain readonly contexts. Compare `readonly` to `init` (initialization-only property setter) and immutable collections; they address different mutation boundaries. Related: [Collections](collections.md).

### `const` versus `readonly`

```csharp
class Limits
{
    public const int MaxRetries = 3;               // Compile-time constant
    public static readonly Guid InstanceId = Guid.NewGuid(); // Runtime initialization
}
```

A `const` value is embedded at use sites and changing a public constant may require consumers to recompile. `readonly` fields can be initialized at declaration or in an appropriate constructor and can hold runtime values.

### `init` and `required`

```csharp
public class Customer
{
    public required string Name { get; init; }
}
var customer = new Customer { Name = "Ada" };
// customer.Name = "Grace"; // Not allowed after initialization
```

`required` is a compile-time initialization contract, not automatic runtime validation. `init` limits when an assignment can occur; it does not make a referenced object deeply immutable. For constructors, primary constructors, and `[SetsRequiredMembers]`, see [Object initialization](object-initialization.md).

## Parameter passing: `ref`, `out`, and `in`

| Modifier | Meaning | Typical use |
| --- | --- | --- |
| `ref` | Pass variable by reference; caller must initialize | Method reads and writes caller's variable |
| `out` | Pass variable by reference; callee must assign before return | Try-pattern outputs |
| `in` | Pass argument by readonly reference | Avoid copying larger structs while disallowing direct reassignment |

```csharp
void Increment(ref int x) => x++;
bool TryGet(out int value) { value = 42; return true; }
int Read(in int x) => x;
```

These modifiers affect parameter passing, not whether a class instance is intrinsically mutable. See [C# fundamentals](csharp-fundamentals.md) for value/reference semantics.

## Member and dispatch modifiers

`static` belongs to the type rather than an instance; `virtual` permits overriding; `override` supplies an override; `sealed override` prevents further overriding; `new` can hide an inherited member. Their behavior depends on the member and call-site type. See [Inheritance and polymorphism](inheritance-and-polymorphism.md) for examples and dispatch rules.

## Scope of this note

This is a decision-oriented reference, not an exhaustive list of every C# reserved or contextual keyword. Expand a section when a new keyword is encountered; extract a separate note only if it becomes a substantial independent topic.
