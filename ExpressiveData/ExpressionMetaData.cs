using System;

namespace ExpressiveData
{
	internal class ExpressionMetaData
	{
		public string ColumnName { get; set; }
		public Type DatabaseType { get; set; }
		public Type ResultType { get; set; }
		public Delegate SetValueFn { get; set; }
	}
}
