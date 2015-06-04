# ExpressiveData
A .NET library that reads data through lambda expressions

Born from a "what-if" discussion, this library aims to make it easier for devs to read data in from applications without requiring a ton of configuration.

```csharp
var model = new Model();
var wrapper = sqlDataReader.GetExpressive();
var context = wrapper.CreateModel(model);

context.ReadFrom(m => m.MyStringProperty, "SomeColumn");
context.ReadFrom(m => m.SomeObject.AnotherProperty, "AnotherColumn");
```

