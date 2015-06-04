using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;

namespace ExpressiveData
{
	public class DataReaderWrapper
	{
		private readonly ConcurrentDictionary<string, int> _fieldMap;

		internal DataReaderWrapper(IDataReader dataReader, ExpressionMetaDataProvider expressionMetaDataProvider)
		{
			MetaDataProvider = expressionMetaDataProvider;
			DataReader = dataReader;
			_fieldMap = new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase);
		}

		public IDataReader DataReader { get; private set; }
		internal ExpressionMetaDataProvider MetaDataProvider { get; private set; }

		internal int GetOrdinal(string fieldName)
		{
			return _fieldMap.GetOrAdd(fieldName, name => DataReader.GetOrdinal(name));
		}

		public INewModelContext<TModel> CreateModel<TModel>(Func<TModel> modelFunc)
		{
			return new NewModelContext<TModel>(this, modelFunc());
		}

		public INewModelContext<TModel> CreateModel<TModel>(TModel modelFunc)
		{
			return new NewModelContext<TModel>(this, modelFunc);
		}
	}
}
