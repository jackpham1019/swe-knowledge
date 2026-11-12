# C# Expression Trees

An expression tree is a structured, inspectable representation of a supported C# expression. It preserves *how a computation is expressed* rather than only providing executable behavior.

```csharp
using System.Linq.Expressions;
Func<Transaction, bool> predicate = t => t.Amount > 1000m;
Expression<Func<Transaction, bool>> tree = t => t.Amount > 1000m;
Func<Transaction, bool> compiled = tree.Compile();
```

The same lambda syntax is converted according to its target type. The compiler can create an expression tree from a supported lambda expression, but it cannot generally reconstruct the original expression tree from an already compiled delegate.

## Tree structure

`t => t.Amount > 1000m` conceptually becomes:

```text
Lambda(t)
└── GreaterThan
    ├── MemberAccess(t.Amount)
    └── Constant(1000m)
```

| Node | Meaning |
|---|---|
| `ParameterExpression` | Parameter or variable |
| `ConstantExpression` | Constant |
| `MemberExpression` | Field/property access |
| `BinaryExpression` | Binary operator |
| `MethodCallExpression` | Method invocation |
| `LambdaExpression` | Lambda body and parameters |

```csharp
var body = (BinaryExpression)tree.Body;
Console.WriteLine(body.NodeType); // GreaterThan
Console.WriteLine(body.Left);     // t.Amount
Console.WriteLine(body.Right);    // 1000
```

## Why preserve the structure?

A query provider can inspect and translate nodes without parsing C# source. This is an example of an **intermediate representation**: separate the description of computation from the mechanism that executes it. The same idea appears in compilers and query planners.

Expression trees support only certain language constructs, and a provider may not understand every valid tree. See [Query providers](query-providers.md) and [delegates-and-lambdas](delegates-and-lambdas.md).
