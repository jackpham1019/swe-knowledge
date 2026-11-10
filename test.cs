// A - Direct task forwarding
public Task<Account> GetAccountAsync(int id)
{
    try
    {
        // If invocation returns a task successfully,
        // the method exits its try block and returns.
        return accountClient.GetAccountAsync(id);
    }
    catch (HttpRequestException)
    {
        Console.WriteLine("Request failed");
        throw;
    }
}