# ExpressiveData
A .NET library that reads data through lambda expressions

Born from a "what-if" discussion, this library aims to make it easier for devs to read data in without requiring a ton of configuration.  

```csharp
var expressive = sqlDataReader.GetExpressive();
var context = expressive.CreateModel(() => new Model());

context.ReadFrom(m => m.MyStringProperty, "SomeColumn");
context.ReadFrom(m => m.SomeObject.AnotherProperty, "AnotherColumn");

// if the column name is the same as the property
// -or- you apply the included DbAttribute to your property, to specify the name to use
context.Read(m => m.MyStringProperty);
context.ReadFrom(m => m.SomeObject.AnotherProperty);

// also available
// context.ReadAsync
// context.ReadFromAsync
```

## So, yet another ORM?

No, absolutely not!  The goal of this project was to eliminate repeating myself when reading data from the database.  In this case, why specify the type when doing the `IDataReader.Get*` call when the type is specified on the field or property?

Its not providing a new way of managing objects.

## Is it stable?

LOL, sorry for laughing, but no....the API is quickly changing but rather than burn another private repo I'm doing it out in the open.
