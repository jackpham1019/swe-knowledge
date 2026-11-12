# C# Delegates and Lambdas

A delegate is a type-safe representation of a callable method. The `delegate` keyword declares a custom delegate type; `Func<...>` and `Action<...>` are built-in generic delegate families.

```csharp
delegate bool PaymentRule(decimal amount);
PaymentRule highValue = amount => amount > 1000m;
Func<decimal, bool> alsoHighValue = amount => amount > 1000m;
Action<string> log = Console.WriteLine;
Console.WriteLine(highValue(1200m)); // True
```

`Func<T, TResult>` returns a value; `Action<T>` returns `void`. A compatible method group or lambda can be converted to a delegate. Delegates can capture variables in a closure; captured variables may outlive the original scope, so consider mutation and lifetime.

## Delegate versus interface

A delegate is useful for a small operation or callback (`Predicate`, selector, event handler). An interface is useful when an object needs several related operations, state, configuration, or a named behavioral contract. Both can support dependency injection, but a delegate usually models **one action**, whereas an interface models a **role**.

## Crucial distinction

`Func<Transaction,bool>` is executable code. `Expression<Func<Transaction,bool>>` is a structured description of an expression; they are not interchangeable representations. See [expression-trees](expression-trees.md).
