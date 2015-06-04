using System;
using System.Data;

namespace ExpressiveData
{
	public static class DataReaderExtensions
	{
		private static readonly Lazy<ExpressionMetaDataProvider> MetaDataProvider =
			new Lazy<ExpressionMetaDataProvider>(() => new ExpressionMetaDataProvider());
 
		public static DataReaderWrapper GetExpressive(this IDataReader reader)
		{
			return new DataReaderWrapper(reader, MetaDataProvider.Value);
		}
	}
}
