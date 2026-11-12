# Inheritance and Polymorphism in C#

> [!question] What problem does overriding solve?
> It lets a base-class reference call a derived implementation without needing to know the concrete derived type. C# makes the override contract explicit.

## The mental model

C# instance methods **are not overridable by default**. A base method must opt in with `virtual` (or be `abstract`, or be an overridable `override`). A derived method uses `override` to participate in runtime polymorphism.

- `virtual` — base implementation is provided and can be overridden.
- `override` — substitutes an implementation for the same virtual dispatch slot.
- `new` — **hides** an inherited member; method selection depends on the compile-time reference type, not overriding dispatch.
- `sealed override` — overrides the method here, but prevents further overriding of that method.
- `abstract` — declares a member without an implementation that a concrete derived class must implement.

## Payment / CardPayment example

```csharp
public class Payment
{
    public virtual string Process() => "Processing generic payment";
    public string Receipt() => "Generic receipt";
}

public class CardPayment : Payment
{
    public override string Process() => "Processing card payment";
    public new string Receipt() => "Card receipt";
}

Payment payment = new CardPayment();
CardPayment card = new CardPayment();

Console.WriteLine(payment.Process()); // Processing card payment (virtual dispatch)
Console.WriteLine(card.Process());    // Processing card payment
Console.WriteLine(payment.Receipt()); // Generic receipt (base-typed reference)
Console.WriteLine(card.Receipt());    // Card receipt (hidden member)
```

**Why does this happen?** `Process()` is virtual, so its implementation is selected using the **runtime type** of the object. `Receipt()` isn't virtual, so a base-typed variable uses the **base-class** member even when the actual object is `CardPayment`.

> [!warning] Hiding is not overriding
> `new` does not make an inherited method polymorphic. It deliberately declares a separate member with the same name; prefer `override` when the intent is substitutable behavior.

## `sealed override`

```csharp
public class SecureCardPayment : CardPayment
{
    public sealed override string Process() => "Processing secure card payment";
}

// A further subclass may inherit Process(), but cannot override it again.
// public class SpecialPayment : SecureCardPayment
// {
//     public override string Process() => "Custom"; // Compile error
// }
```

`sealed override` seals **that override**, not necessarily the class. `sealed class` prevents inheritance from the entire class.

## Syntax reference

| Syntax                     | Meaning                                           | Virtual dispatch?                               |
| -------------------------- | ------------------------------------------------- | ----------------------------------------------- |
| `virtual void M()`         | Has a base implementation; permits overrides      | Yes                                             |
| `override void M()`        | Overrides inherited virtual/abstract member       | Yes                                             |
| `new void M()`             | Explicitly hides inherited member                 | No, unless independently declared `new virtual` |
| `sealed override void M()` | Overrides once and blocks subsequent overrides    | Yes, up to this implementation                  |
| `abstract void M();`       | Requires an implementation in a concrete subclass | Yes                                             |

## Access modifiers quick reference

| Modifier | Accessible from |
| --- | --- |
| `public` | Anywhere the containing type is accessible |
| `private` | Containing type |
| `protected` | Containing type and derived types |
| `internal` | Same assembly |
| `protected internal` | Same assembly **or** derived types in other assemblies |
| `private protected` | Containing type and derived types **within the same assembly** |

An override cannot arbitrarily change the accessibility of the overridden member. Accessibility rules also apply to which base members are visible to derived code.

## Java comparison

Java instance methods are generally virtual unless constrained (for example, `final`, `private`, or `static`). In C#, the default is the opposite: overriding is explicitly opted into with `virtual`/`abstract`, and implemented with `override`.

## Code-reading checklist

1. Is the member marked `virtual`, `abstract`, or `override`?
2. Does the derived member say `override` or `new`?
3. What is the **declared type** of the reference?
4. What is the **runtime type** of the object?
5. Is further overriding blocked by `sealed override`?

## Related notes

- [C# index](../backend.md)
- [Language fundamentals](csharp-fundamentals.md)
- [Object initialization](object-initialization.md)
