# Cancellation Boundaries and Business Consistency

> **Status:** Working reference

## 7. Cancellation is subject to business and consistency requirements

This became one of the most important conclusions of our discussion.

Consider:

```
public async Task CheckoutAsync(
    CancellationToken token)
{
    await ReserveInventoryAsync(token);

    await ChargePaymentAsync(token);

    await ConfirmOrderAsync(token);
}
```

Suppose inventory reservation succeeds and the payment is charged successfully.

Immediately afterward, the caller requests cancellation.

Should the system abandon order confirmation?

Our reasoning was that this depends on the business requirements and the consistency obligations of the workflow.

A cancellation request does not necessarily mean the customer wants to cancel the purchase.

For example, closing a browser or disconnecting from the server should not automatically invalidate a successfully charged payment.

Once important side effects have committed, the system may need to complete or recover the workflow regardless of caller cancellation.

### Cancellation boundaries

Before important side effects occur, honoring caller cancellation may be safe.

After irreversible or externally committed actions, stopping immediately may leave the system inconsistent.

This introduces the concept of a **point of no cancellation**: a boundary after which the caller's cancellation request should no longer interrupt required consistency work.

```
public async Task CheckoutAsync(
    CancellationToken requestToken)
{
    await ReserveInventoryAsync(requestToken);

    await ChargePaymentAsync(requestToken);

    // Required consistency work should not be
    // abandoned because the caller disconnected.
    await ConfirmOrderAsync(CancellationToken.None);
}
```

This is only a simplified illustration. A production checkout must account for uncertain payment outcomes, idempotency, durable workflow state, and recovery.

**Core principle:** Cancellation is subject to the operation's business and consistency requirements.

The caller expresses a desire to stop, but the system retains responsibility for preserving valid business state.

## 9. Consolidated mental models

### Cancellation is cooperative

A token communicates cancellation intent. Operations must observe it and stop at safe checkpoints.

### Cancellation propagation preserves shared intent

Related subtasks generally receive the same token so that they can respond to the caller's cancellation request.

### Cancellation ownership is separate from execution

The caller may own the cancellation decision without controlling how each operation responds.

### Linked cancellation combines independent conditions

A linked token allows caller cancellation, timeouts, or other cancellation sources to govern the same operation.

### Cancellation is not transactional rollback

Stopping execution does not establish whether committed state has been reversed.

### Business consistency takes precedence over unconditional cancellation

Once important side effects have occurred, the workflow may need to complete or recover independently of the caller.

### Recovery has its own lifecycle

Cleanup, rollback, and compensation should not automatically inherit the cancellation policy of the failed primary operation.

---

## Related concepts

- [[cancellation-tokens-and-propagation]]
- [[cancellation-and-transaction-recovery]]
