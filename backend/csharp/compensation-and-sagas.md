# Compensation and Sagas

> **Status:** Introductory reference — saga failure modes remain to be explored

## 2. Why compensation is a separate mechanism

In a database transaction, we may have atomic rollback.

But inventory and payment may belong to independent systems that cannot participate in one atomic transaction.

The application may need a compensating action:

```
await ReserveInventoryAsync();

try
{
    await ChargePaymentAsync();
}
catch
{
    await ReleaseInventoryAsync();
    throw;
}
```

This is a simplified example. Production workflows must also account for compensation failures and uncertain payment outcomes.

Notice that I've changed the workflow from concurrent to sequential execution.

Why?

Because charging payment and reserving inventory are not necessarily independent business operations. Their ordering affects the failure scenarios we must handle.

This is the underlying idea behind the Saga pattern in distributed systems: coordinating a business process across independently committed operations, with compensating actions when necessary.

## 3. A useful architectural distinction

|Mechanism|Responsibility|
|---|---|
|`Task.WhenAll()`|Coordinate asynchronous task completion|
|`CancellationToken`|Cooperatively request that work stop|
|Database transaction|Provide atomicity within a supported transactional boundary|
|Compensation / Saga|Recover business consistency across independently committed operations|

These mechanisms solve different problems and should not be conflated.

## 3. The architectural distinction

Cancellation and rollback solve different problems.

|Mechanism|Responsibility|
|---|---|
|Cancellation|Request that ongoing execution stop|
|Transaction|Define which changes commit or roll back atomically|
|Compensation|Recover from committed side effects that cannot be rolled back together|

A cancellation token does not provide transactional guarantees.

## Related concepts

- [[cancellation-and-transaction-recovery]]
- [[task-concurrency-and-whenall]]
