# C# Exception Propagation and Translation

C# does not have Java-style checked exceptions: callers are not required by the compiler to catch or declare thrown exception types.

```csharp
throw new InvalidOperationException("Invalid state");
```

Within a `catch`, use `throw;` to rethrow the current exception while preserving its original stack information. `throw ex;` resets relevant stack information and is usually not what you want.

```csharp
try
{
    await gateway.SendAsync();
}
catch (HttpRequestException ex)
{
    throw new PaymentGatewayException("Gateway unavailable", ex);
}
```

Exception translation adds domain/application meaning while preserving the original cause in `InnerException`. Translate only when the abstraction boundary adds meaningful context; otherwise propagate. Avoid catching broad exceptions just to log and rethrow repeatedly.

## Cleanup exceptions

C# `using` compiles conceptually to `try/finally`. If `Dispose()` throws during unwinding, that exception can replace an exception from the body; unlike Java try-with-resources, C# does not automatically collect a suppressed-exception list. Resource cleanup should normally be reliable, and diagnostics may need explicit handling when cleanup can fail. See [[resource-ownership-and-disposal]] and [[async-await-task-flow]].
