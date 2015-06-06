using System;
using System.Collections;
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

		public void ReadResultSet<TModel>(Action<IExpressiveReader<TModel>> callback)
		{
			ProcessCallback(callback);
		}

		public void ReadResultSets<TModel1, TModel2>(Action<IExpressiveReader<TModel1>> callback1, Action<IExpressiveReader<TModel2>> callback2)
		{
			ProcessCallback(callback1);
			if (!DataReader.NextResult())
				return;
			ProcessCallback(callback2);
		}

		public void ReadResultSets<TModel1, TModel2, TModel3>(Action<IExpressiveReader<TModel1>> callback1, Action<IExpressiveReader<TModel2>> callback2, Action<IExpressiveReader<TModel3>> callback3)
		{
			ProcessCallback(callback1);
			if (!DataReader.NextResult())
				return;
			ProcessCallback(callback2);
			if (!DataReader.NextResult())
				return;
			ProcessCallback(callback3);
		}

		public IEnumerable<TModel> GetModelsForResultSet<TModel>(Func<IExpressiveReader<TModel>, TModel> modelGeneratorFunc)
		{
			return Fill(modelGeneratorFunc);
		}

		public ModelsForResultSetsResult GetModelsForResultSets<TModel>(Func<IExpressiveReader<TModel>, TModel> modelGeneratorFunc)
		{
			var item1 = Fill(modelGeneratorFunc);

			return new ModelsForResultSetsResult(new IEnumerable[] { item1 });
		}

		public ModelsForResultSetsResult GetModelsForResultSets<TModel1, TModel2>(Func<IExpressiveReader<TModel1>, TModel1> modelGeneratorFunc1, Func<IExpressiveReader<TModel2>, TModel2> modelGeneratorFunc2)
		{
			var item1 = Fill(modelGeneratorFunc1);
			var item2 = DataReader.NextResult() ? Fill(modelGeneratorFunc2) : null;

			return new ModelsForResultSetsResult(new IEnumerable[] { item1, item2 });
		}

		public ModelsForResultSetsResult GetModelsForResultSets<TModel1, TModel2, TModel3>(Func<IExpressiveReader<TModel1>, TModel1> modelGeneratorFunc1, Func<IExpressiveReader<TModel2>, TModel2> modelGeneratorFunc2,
			Func<IExpressiveReader<TModel3>, TModel3> modelGeneratorFunc3)
		{
			var item1 = Fill(modelGeneratorFunc1);
			var item2 = DataReader.NextResult() ? Fill(modelGeneratorFunc2) : null;
			var item3 = item2 != null && DataReader.NextResult() ? Fill(modelGeneratorFunc3) : null;

			return new ModelsForResultSetsResult(new IEnumerable[] {item1, item2, item3});
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

		private void ProcessCallback<TModel>(Action<IExpressiveReader<TModel>> callback)
		{
			ClearMap();
			var reader = new ExpressiveIDataReader<TModel>(this, DataReader, MetaDataProvider);
			while (DataReader.Read())
				callback(reader);
		}
	}
}
