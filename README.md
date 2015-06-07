# ExpressiveData
A .NET library that reads data through lambda expressions

Born from a "what-if" discussion, this library aims to make it easier for devs to read data in without requiring a ton of configuration.  

```csharp
using(var dataReader = await cmd.ExecuteReaderAsync())
{
	var expressive = dataReader.GetExpressive();
	var items = await expressive.GetModelsForResultSetAsync<Customer>(async r => 
	{
		r.Model = new Customer();

		await r.ReadAsync(m => m.Id);
		await r.ReadAsync(m => m.Name);
		await r.ReadAsync(m => m.Address);
		await r.ReadAsync(m => m.City);
		await r.ReadAsync(m => m.State);
		await r.ReadAsync(m => m.ZipCode);
	});
}
```

## FAQ
### So, yet another ORM?

No, absolutely not!  The goal of this project was to eliminate repeating myself when reading data from the database.  In this case, why specify the type when doing the `IDataReader.Get*` call when the type is specified on the field or property?

Its not providing a new way of managing objects.

### Is it stable?

LOL, sorry for laughing, but no....the API is quickly changing but rather than burn another private repo I'm doing it out in the open.
