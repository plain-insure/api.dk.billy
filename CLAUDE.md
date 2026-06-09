# Billy API — Agent & Developer Guide

## Project overview

.NET 8 library that wraps the [Billy Billing REST API](https://www.billy.dk) (`https://api.billysbilling.com/v2/`). Uses RestSharp for HTTP and `System.Text.Json` for serialization. The architecture is convention-driven: adding a new resource is mostly a matter of following the patterns below.

## Repository hierarchy

```
Base<T, TRoot>                   ← Get + List (read-only resources)
  └─ BaseWithCreate<T, TRoot>    ← + Create + Update
       └─ BaseWithDelete<T, TRoot> ← + Delete
```

| Repository class | Base | Notes |
|---|---|---|
| `Accounts` | `Base` | read-only; sideloads Group + Organization |
| `AccountGroups` | `Base` | read-only |
| `Organizations` | `Base` | read-only |
| `TaxRates` | `Base` | read-only |
| `SalesTaxRulesets` | `Base` | read-only |
| `Contacts` | `BaseWithDelete` | optional SideloadCountry / SideloadLocale |
| `Bills` | `BaseWithDelete` | optional SideloadLines |
| `Invoices` | `BaseWithDelete` | custom MostRecent helper |
| `Products` | `BaseWithDelete` | |
| `Daybooks` | `BaseWithDelete` | |
| `DaybookTransactions` | `BaseWithDelete` | sideloads Lines in constructor |

## How to add a new resource

Every resource in the library — Bills, Invoices, Contacts, etc. — is just a class that inherits one of the three base repository classes. To add a new endpoint, you must create two things: a **model file** (the C# representation of what the API returns) and a **repository file** (the class callers instantiate). Library consumers can do the exact same thing in their own project to reach endpoints not yet covered by this library, or to subclass an existing repository to add custom helper methods.

### 1. Create the model + root in `Billy.Api/Models/Xxx.cs`

Follow this template exactly — the base classes use reflection to find these properties by name:

```csharp
namespace Billy.Api.Models
{
    // Used by BaseWithDelete to extract the deleted ID from Meta.DeletedRecords
    public class XxxIdList
    {
        public List<string>? Xxxs { get; set; }  // must match class name, pluralized
    }

    // The envelope returned by the Billy API for all Xxx endpoints
    public class XxxRoot : Root<XxxIdList>
    {
        public Xxx? Xxx { get; set; }          // singular — used by Get()
        public List<Xxx>? Xxxs { get; set; }   // plural  — used by List() and Create()
        // Add any sideloaded sibling collections here:
        // public List<SomeOtherModel>? SomeOtherModels { get; set; }
    }

    public class Xxx : IEntity
    {
        public string? Id { get; set; }
        public string OrganizationId { get; set; }

        [JsonIgnoreOnWrite]          // server-computed; omit on POST/PUT
        public DateTime CreatedTime { get; set; }

        // Date-only fields must use BillyDateConverter
        [JsonConverter(typeof(Converters.BillyDateConverter))]
        public DateTime SomeDate { get; set; }

        // Sideloaded reference: expose the Id for serialization, hide the object
        public string? FooId { get; set; }
        [JsonIgnore]
        public Foo? Foo { get; set; }

        // Sideloaded collection: expose Ids for serialization, hide the list
        [JsonIgnoreOnWrite]
        public List<string>? LineIds { get; set; }
        public List<XxxLine>? Lines { get; set; }
    }
}
```

**Critical naming rules (enforced by reflection, not code):**
- `TRoot` must have a property named exactly `T.Name` (e.g. `Xxx`) for single-item access.
- `TRoot` must have a property named exactly after the repository class (e.g. `Xxxs`) for list access — this is the constructor argument `CompileDefaultMultiple(this.GetType().Name)`.
- `XxxIdList` must have a property named `Xxxs` (plural of entity type) for delete to work.
- Sideload id properties follow `{PropertyName}Id` (single) or `{SingularPropertyName}Ids` (collection).

### 2. Create the repository class `Billy.Api/Xxx.cs`

**Read-only (Get + List only):**
```csharp
using Billy.Api.Models;

namespace Billy.Api
{
    public class Xxxs : Repositories.Base<Xxx, XxxRoot>
    {
        public Xxxs(RestSharp.RestClient client) : base(client) { }
        public Xxxs(string key) : base(key) { }
    }
}
```

**Full CRUD (Get + List + Create + Update + Delete):**
```csharp
using Billy.Api.Models;

namespace Billy.Api
{
    public class Xxxs : Repositories.BaseWithDelete<Xxx, XxxRoot>
    {
        public Xxxs(RestSharp.RestClient client) : base(client) { }
        public Xxxs(string key) : base(key) { }
    }
}
```

The URL is auto-derived: class name `Xxxs` → `/xxxs/` (camelCase of `this.GetType().Name`).

### 3. (Optional) Subclass an existing repository to extend it

If you only want to add helpers on top of a resource that already exists, just inherit the concrete class:

```csharp
public class MyBills : Billy.Api.Bills
{
    public MyBills(string key) : base(key) { }

    public async Task<IList<Bill>?> GetApprovedAsync()
        => await ListAsync(new { state = "approved" });
}
```

Library consumers can do this from their own project without modifying the library at all.

## Sideloading

Sideloads tell Billy to return related objects in the same response envelope. They are configured per-repository instance and automatically applied after each Get/List call.

### Single-object sideload (e.g. a foreign-key reference)

```csharp
// Auto-discovers FooId on T and Foos on TRoot:
AddSideload(root => root.Foos, item => item.Foo);

// Explicit include key override:
AddSideload(root => root.Foos, item => item.Foo, "xxx.foo");
```

Convention: include key defaults to `{camelCase(T.Name)}.{camelCase(PropertyName)}` — e.g. `account.group`.

### Collection sideload (e.g. line items)

```csharp
// Auto-discovers LineIds on T and Lines on TRoot:
AddSideload(root => root.XxxLines, item => item.Lines);
```

Convention: singularizes the property name (`Lines` → `Line`) and looks for `LineIds` on the entity.

## Key attributes

| Attribute | Effect |
|---|---|
| `[JsonIgnoreOnWrite]` | Property is deserialized (read) but excluded from POST/PUT body |
| `[JsonConverter(typeof(BillyDateConverter))]` | Serializes `DateTime` as `yyyy-MM-dd` |
| `[JsonIgnore]` | Property is never serialized/deserialized; use for sideloaded object references |

## Partial updates — DeltaObject\<T\>

Use `DeltaObject<T>` to send only the changed fields in a PUT request. Per-property `[JsonConverter]` attributes are honoured during serialization.

```csharp
var delta = new DeltaObject<Bill>()
    .Set(b => b.State, BillStates.Approved)
    .Set(b => b.DueDate, DateTime.Today.AddDays(30));

await bills.UpdateAsync(id, delta);

// Alternatively via tuple syntax (less type-safe):
var delta2 = new DeltaObject<Bill>(
    (b => b.State, BillStates.Approved)
);
```

`DeltaObject<T>` can also be passed as a filter to `ListAsync` — it uses only its tracked modifications as query parameters.

## Filtering, sorting, and paging

```csharp
// Anonymous object filter (property names are camelCased automatically)
var result = await contacts.ListAsync(new { isCustomer = true });

// Sort by expression
var result = await invoices.ListAsync(i => i.ApprovedTime, SortOrder.DESC);

// Filter + sort + page
var result = await bills.ListAsync(
    new { state = "approved" },
    b => b.EntryDate,
    SortOrder.ASC,
    pageSize: 50
);
```

## Client construction

```csharp
// From API key (creates its own HttpClient)
var bills = new Bills("your-api-key");

// From a shared RestClient (for DI / lifetime control)
var client = ClientExtensions.CreateBillyClient("your-api-key");
var bills  = new Bills(client);
var invoices = new Invoices(client);

// With debug logging
var debugLog = new BillyDebugLog(Console.WriteLine);
var client = ClientExtensions.CreateBillyClient(httpClient, "key", debugLog);
```

## Serialization conventions

All JSON uses **camelCase** property names (`RestJsonOptions.Instance`). Settings:
- `PropertyNamingPolicy = JsonNamingPolicy.CamelCase`
- `DefaultIgnoreCondition = WhenWritingDefault`
- `JsonStringEnumConverter` with camelCase enum values
- `IgnoreOnWriteTypeInfoResolver` — enforces `[JsonIgnoreOnWrite]`

## Testing

Set environment variable `BILLY_TEST_APIKEY` to a real Billy test account API key, then run the test project `Billy.Api.Tests`. Restart Visual Studio / your shell after setting the variable.

## File layout

```
Billy.Api/
  Converters/
    BillyDateConverter.cs       ← yyyy-MM-dd DateTime converter
  Models/
    Meta.cs                     ← Root<TList>, Meta, Paging
    Bill.cs / BillRoot          ← per-resource model + envelope
    ...
  Repositories/
    Base.cs                     ← Get, List; auto-compiles accessors
    Base.SideloadDescriptor.cs  ← sideload matching logic
    BaseHelpers.cs              ← reflection/expression helpers
    BaseWithDelete.cs           ← adds Delete
  Utils/
    ClientExtensions.cs         ← factory helpers for RestClient
    DeltaObject.cs              ← partial-update tracker
    IgnoreOnWriteTypeInfoResolver.cs
    JsonIgnoreOnWriteAttribute.cs
    RequestExtensions.cs        ← AddFilter, AddSorting, AddPaging, AddIncludes
    ReflectionExtensions.cs     ← expression helpers for sideload wiring
    RestJsonOptions.cs          ← shared JsonSerializerOptions singleton
  IEntity.cs                    ← interface: string Id
  Bills.cs / Invoices.cs / ...  ← concrete repository classes
Billy.Api.Tests/                ← integration tests (need real API key)
```
