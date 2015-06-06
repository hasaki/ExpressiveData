using System;
using System.Data;
using System.Data.SqlClient;

namespace ExpressiveData
{
	public static class DataReaderExtensions
	{
		private static readonly Lazy<ExpressionMetaDataProvider> MetaDataProvider =
			new Lazy<ExpressionMetaDataProvider>(() => new ExpressionMetaDataProvider());
 
		public static IExpressiveResultSet GetExpressive(this IDataReader reader)
		{
			return new ExpressiveIDataReaderResultSet(reader, MetaDataProvider.Value);
		}

		public static IExpressiveResultSetAsync GetExpressive(this SqlDataReader reader)
		{
			return new ExpressiveDbDataReaderResultSet(reader, MetaDataProvider.Value);
		}
	}
}
