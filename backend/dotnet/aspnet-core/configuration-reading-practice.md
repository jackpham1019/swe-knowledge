# ASP.NET Core Configuration — Reading Practice

**Status:** Practice  
**Related:** [C# language notes](csharp-fundamentals.md) · [ASP.NET Core index](aspnet-core.md)

## Goal

Learn to read and explain common .NET configuration code, including where configuration values originate, how they are accessed, and how they become available to application components.

## 1. Application settings

`appsettings.json`

```
{
  "App": {
    "Name": "Banking API",
    "MaxTransferAmount": 5000
  },
  "ConnectionStrings": {
    "Default": "Host=localhost;Database=banking"
  }
}
```

Read the configuration:

```
var builder = WebApplication.CreateBuilder(args);

string? name =
    builder.Configuration["App:Name"];

int maxTransfer =
    builder.Configuration.GetValue<int>(
        "App:MaxTransferAmount");

string? connection =
    builder.Configuration.GetConnectionString(
        "Default");
```

**Reading exercise**

- Where does `builder.Configuration` come from?
    
- What does the colon in `"App:Name"` represent?
    
- Why does `GetValue<int>()` require a type argument?
    
- Why might `name` and `connection` be nullable?
    

## 2. Configuration binding

Instead of retrieving individual values, configuration can be bound to a strongly typed class.

```
public class AppSettings
{
    public string Name { get; set; } = "";
    public int MaxTransferAmount { get; set; }
}
```

```
var settings = builder.Configuration
    .GetSection("App")
    .Get<AppSettings>();
```

**Reading exercise**

- What does `GetSection("App")` select?
    
- What does `Get<AppSettings>()` produce?
    
- How does the configuration binder associate keys with properties?
    
- What happens if a required configuration value is missing?
    

## 3. Options pattern and dependency injection

```
builder.Services.Configure<AppSettings>(
    builder.Configuration.GetSection("App"));
```

A service can then consume the options:

```
public class TransferService
{
    private readonly AppSettings _settings;

    public TransferService(
        IOptions<AppSettings> options)
    {
        _settings = options.Value;
    }

    public bool CanTransfer(decimal amount)
    {
        return amount <= _settings.MaxTransferAmount;
    }
}
```

**Reading exercise**

- What responsibility belongs to `builder.Services`?
    
- What does `Configure<AppSettings>()` establish?
    
- Why inject `IOptions<AppSettings>` instead of reading configuration directly?
    
- Where does the `TransferService` obtain its settings?
    

## 4. Configuration sources and precedence

Typical ASP.NET Core application configuration sources include:

- `appsettings.json`
    
- `appsettings.Development.json`
    
- Environment variables
    
- Command-line arguments
    
- Development user secrets
    

Later configuration providers can override earlier providers for the same key.

For example, the environment variable:

```
App__MaxTransferAmount=10000
```

can override the JSON value of `5000`.

The double underscore represents a configuration hierarchy separator in environment-variable keys.

**Reading exercise**

Why should application code depend on configuration keys rather than assume every value comes from a particular JSON file?

## 5. Practice checklist

- Read configuration values from JSON
    
- Understand configuration hierarchy
    
- Bind a configuration section to a class
    
- Explain generic type arguments in configuration APIs
    
- Understand dependency injection of options
    
- Explain configuration source precedence
    
- Test overriding settings with environment variables
    

## Key takeaways

**Configuration separates deployment-specific values from application behavior.**

Strongly typed configuration binding provides a convenient representation of settings, but binding alone does not guarantee that values are valid.

Applications should validate required settings and avoid committing production secrets into configuration files.

## Notes from practice

_Add unfamiliar syntax, mistakes, and explanations here after completing the exercises._