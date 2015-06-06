using System;
using System.Data;

namespace ExpressiveData
{
	public static class DataReaderExtensions
	{
		private static readonly Lazy<ExpressionMetaDataProvider> MetaDataProvider =
			new Lazy<ExpressionMetaDataProvider>(() => new ExpressionMetaDataProvider());
 
		public static ExpressiveIDataReaderResultSet GetExpressive(this IDataReader reader)
		{
			return new ExpressiveIDataReaderResultSet(reader, MetaDataProvider.Value);
		}
	}
}
