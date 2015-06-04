using System;

namespace ExpressiveData
{
	[AttributeUsage(AttributeTargets.Property)]
	public class DbAttribute : Attribute
	{
		public string ColumnName { get; set; }
	}
}
