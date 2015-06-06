using System;

namespace ExpressiveData
{
	[AttributeUsage(AttributeTargets.Property)]
	public class ExpressiveColumnAttribute : Attribute
	{
		public string ColumnName { get; set; }
		public Type DatabaseType { get; set; }
	}
}
