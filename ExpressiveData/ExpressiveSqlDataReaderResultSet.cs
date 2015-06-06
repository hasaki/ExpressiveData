using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ExpressiveData
{
	internal class ExpressiveSqlDataReaderResultSet : ExpressiveIDataReaderResultSet, IExpressiveResultSetAsync
	{
		private readonly SqlDataReader _dataReader;
		private readonly ExpressionMetaDataProvider _expressionMetaDataProvider;

		internal ExpressiveSqlDataReaderResultSet(SqlDataReader dataReader, ExpressionMetaDataProvider expressionMetaDataProvider) 
			: base(dataReader, expressionMetaDataProvider)
		{
			if (dataReader == null)
				throw new ArgumentNullException("dataReader");

			_dataReader = dataReader;
			_expressionMetaDataProvider = expressionMetaDataProvider;
		}

		public async Task<IEnumerable<TModel>> FillModelsAsync<TModel>(Func<IExpressiveReaderAsync<TModel>, Task<TModel>> generator)
		{
			return await FillAsync(generator);
		}

		public Task<Tuple<IEnumerable<TModel1>, IEnumerable<TModel2>>> FillModelsAsync<TModel1, TModel2>(
			Func<IExpressiveReaderAsync<TModel1>, Task<TModel1>> generator1,
			Func<IExpressiveReaderAsync<TModel2>, Task<TModel2>> generator2)
		{
			throw new NotImplementedException();
		}

		public Task<Tuple<IEnumerable<TModel1>, IEnumerable<TModel2>, IEnumerable<TModel3>>> FillModelsAsync<TModel1, TModel2, TModel3>(
			Func<IExpressiveReaderAsync<TModel1>, Task<TModel1>> generator1,
			Func<IExpressiveReaderAsync<TModel2>, Task<TModel2>> generator2,
			Func<IExpressiveReaderAsync<TModel3>, Task<TModel3>> generator3)
		{
			throw new NotImplementedException();
		}

		private async Task<IEnumerable<TModel>> FillAsync<TModel>(Func<IExpressiveReaderAsync<TModel>, Task<TModel>> generator)
		{
			ClearMap();
			var list = new List<TModel>();
			var reader = new ExpressiveSqlDataReader<TModel>(this, _dataReader, _expressionMetaDataProvider);
			while (await _dataReader.ReadAsync())
			{
				list.Add(await generator(reader));
			}

			return list;
		}
	}
}
