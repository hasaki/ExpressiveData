using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;

namespace ExpressiveData
{
	public class ExpressiveIDataReaderResultSet : IExpressiveResultSet, IOrdinalProvider
	{
		private readonly ConcurrentDictionary<string, int> _fieldMap;

		internal ExpressiveIDataReaderResultSet(IDataReader dataReader, ExpressionMetaDataProvider expressionMetaDataProvider)
		{
			MetaDataProvider = expressionMetaDataProvider;
			DataReader = dataReader;
			_fieldMap = new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase);
		}

		public IDataReader DataReader { get; private set; }
		internal ExpressionMetaDataProvider MetaDataProvider { get; private set; }

		public int GetOrdinal(string fieldName)
		{
			return _fieldMap.GetOrAdd(fieldName, name => DataReader.GetOrdinal(name));
		}

		protected void ClearMap()
		{
			_fieldMap.Clear();
		}

		public IExpressiveReader<TModel> CreateModel<TModel>(Func<TModel> modelFunc)
		{
			return new ExpressiveIDataReader<TModel>(this, DataReader, MetaDataProvider);
		}

		public IExpressiveReader<TModel> FillModel<TModel>(TModel modelFunc)
		{
			return new ExpressiveIDataReader<TModel>(this, DataReader, MetaDataProvider);
		}

		public IEnumerable<TModel> FillModels<TModel>(Func<IExpressiveReader<TModel>, TModel> generator)
		{
			return Fill(generator);
		}

		public Tuple<IEnumerable<TModel1>, IEnumerable<TModel2>> FillModels<TModel1, TModel2>(
			Func<IExpressiveReader<TModel1>, TModel1> generator1, 
			Func<IExpressiveReader<TModel2>, TModel2> generator2)
		{
			var item1 = Fill(generator1);
			var item2 = DataReader.NextResult() ? Fill(generator2) : null;

			return new Tuple<IEnumerable<TModel1>, IEnumerable<TModel2>>(item1, item2);
		}

		public Tuple<IEnumerable<TModel1>, IEnumerable<TModel2>, IEnumerable<TModel3>> FillModels<TModel1, TModel2, TModel3>(
			Func<IExpressiveReader<TModel1>, TModel1> generator1,
			Func<IExpressiveReader<TModel2>, TModel2> generator2,
			Func<IExpressiveReader<TModel3>, TModel3> generator3)
		{
			var item1 = Fill(generator1);
			var item2 = DataReader.NextResult() ? Fill(generator2) : null;
			var item3 = item2 != null && DataReader.NextResult() ? Fill(generator3) : null;

			return new Tuple<IEnumerable<TModel1>, IEnumerable<TModel2>, IEnumerable<TModel3>>(item1, item2, item3);
		}

		private IEnumerable<TModel> Fill<TModel>(Func<IExpressiveReader<TModel>, TModel> generator)
		{
			_fieldMap.Clear();
			var list = new List<TModel>();
			var reader = new ExpressiveIDataReader<TModel>(this, DataReader, MetaDataProvider);
			while (DataReader.Read())
			{
				list.Add(generator(reader));
			}

			return list;
		}
	}
}
