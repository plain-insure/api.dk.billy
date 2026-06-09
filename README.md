# Billy API — .NET client

A .NET 8 library for the [Billy Billing](https://www.billy.dk) REST API (`https://api.billysbilling.com/v2/`).

[![Build status](https://github.com/plain-insure/api.dk.billy/actions/workflows/publish.yml/badge.svg)](https://github.com/plain-insure/api.dk.billy/actions/workflows/publish.yml)

## Install

```
install-package BillyService
```

## Supported resources

| Resource | Get | List | Create | Update | Delete |
|---|:---:|:---:|:---:|:---:|:---:|
| BankPayments | ✓ | ✓ | ✓ | ✓ | ✓ |
| Bills | ✓ | ✓ | ✓ | ✓ | ✓ |
| Invoices | ✓ | ✓ | ✓ | ✓ | ✓ |
| Contacts | ✓ | ✓ | ✓ | ✓ | ✓ |
| Products | ✓ | ✓ | ✓ | ✓ | ✓ |
| Daybooks | ✓ | ✓ | ✓ | ✓ | ✓ |
| DaybookTransactions | ✓ | ✓ | ✓ | ✓ | ✓ |
| Accounts | ✓ | ✓ | | | |
| AccountGroups | ✓ | ✓ | | | |
| Organizations | ✓ | ✓ | | | |
| TaxRates | ✓ | ✓ | | | |
| SalesTaxRulesets | ✓ | ✓ | | | |

## Quick start

```csharp
// Construct from an API key — creates its own HttpClient
var bills = new Bills("your-api-key");

// Or share a single RestClient across repositories (better for DI scenarios)
var client = ClientExtensions.CreateBillyClient("your-api-key");
var bills    = new Bills(client);
var invoices = new Invoices(client);
var contacts = new Contacts(client);
```

### Get and list

```csharp
var bill = await bills.GetAsync("bill-id");

var allBills = await bills.ListAsync();

// Filter, sort, page
var recent = await invoices.ListAsync(
    new { contactId = "abc123" },
    i => i.ApprovedTime,
    SortOrder.DESC,
    pageSize: 25
);
```

### Create and update

```csharp
var id = await bills.CreateAsync(new Bill
{
    OrganizationId = "org-id",
    ContactId      = "contact-id",
    EntryDate      = DateTime.Today,
    DueDate        = DateTime.Today.AddDays(30),
    TaxMode        = "incl",
    Lines = [
        new BillLine { AccountId = "acc-id", Amount = 10000 }
    ]
});

// Full update (sends the whole object)
await bills.UpdateAsync(bill);

// Partial update — only sends the changed fields
var delta = new DeltaObject<Bill>()
    .Set(b => b.State, BillStates.Approved)
    .Set(b => b.DueDate, DateTime.Today.AddDays(14));

await bills.UpdateAsync(id, delta);
```

### Delete

```csharp
await bills.DeleteAsync("bill-id");
```

### Sideloading related objects

Some repositories expose opt-in sideloads that fetch related data in a single API call:

```csharp
// Bills — loads BillLine objects and attaches them to Bill.Lines
bills.SideloadLines();
var bill = await bills.GetAsync("bill-id");  // bill.Lines is populated

// Contacts — attach Country and Locale objects
contacts.SideloadCountry();
contacts.SideloadLocale();
var contact = await contacts.GetAsync("contact-id");  // contact.Country is populated

// Accounts always sideloads Group and Organization automatically
var account = await accounts.GetAsync("account-id");  // account.Group is populated
```

### Debug logging

```csharp
var client = ClientExtensions.CreateBillyClient(
    httpClient,
    "your-api-key",
    new BillyDebugLog(Console.WriteLine)
);
```

## Testing

1. Register at [www.billy.dk](https://www.billy.dk) and create an API key in account settings.
2. Set the environment variable `BILLY_TEST_APIKEY` to your key.
3. Restart Visual Studio so it picks up the new variable.
4. Run the tests in `Billy.Api.Tests`.

## Extending — add your own endpoints

Every repository in this library is simply a class that inherits one of three base classes. You can do the same thing in your own project to reach Billy endpoints that haven't been implemented here yet, or to add custom helper methods on top of existing ones.

### Read-only endpoint (Get + List)

```csharp
// Your model
public class BankPaymentIdList { public List<string>? BankPayments { get; set; } }
public class BankPaymentRoot : Root<BankPaymentIdList>
{
    public BankPayment? BankPayment { get; set; }
    public List<BankPayment>? BankPayments { get; set; }
}
public class BankPayment : IEntity
{
    public string? Id { get; set; }
    public string OrganizationId { get; set; }
    // ... other fields
}

// Your repository
public class BankPayments : Billy.Api.Repositories.Base<BankPayment, BankPaymentRoot>
{
    public BankPayments(string key) : base(key) { }
    public BankPayments(RestClient client) : base(client) { }
}
```

### Full CRUD endpoint (Get + List + Create + Update + Delete)

```csharp
public class BankPayments : Billy.Api.Repositories.BaseWithDelete<BankPayment, BankPaymentRoot>
{
    public BankPayments(string key) : base(key) { }
    public BankPayments(RestClient client) : base(client) { }
}
```

### Add custom methods to an existing repository

Inherit from the existing class and add whatever you need:

```csharp
public class MyInvoices : Billy.Api.Invoices
{
    public MyInvoices(string key) : base(key) { }

    public async Task<IList<Invoice>?> GetDraftAsync()
        => await ListAsync(new { state = "draft" });
}
```

The URL is auto-derived from the class name (`BankPayments` → `/bankPayments/`). See [CLAUDE.md](CLAUDE.md) for the full naming conventions and sideload wiring guide.

## Architecture

See [CLAUDE.md](CLAUDE.md) for a detailed guide on the internal patterns and step-by-step instructions for adding new resources.
