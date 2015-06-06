using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace ExpressiveData
{
	internal class ExpressiveDbDataReaderResultSet : ExpressiveIDataReaderResultSet, IExpressiveResultSetAsync
	{
		private readonly DbDataReader _dataReader;
		private readonly ExpressionMetaDataProvider _expressionMetaDataProvider;

		internal ExpressiveDbDataReaderResultSet(DbDataReader dataReader, ExpressionMetaDataProvider expressionMetaDataProvider) 
			: base(dataReader, expressionMetaDataProvider)
		{
			if (dataReader == null)
				throw new ArgumentNullException("dataReader");

			_dataReader = dataReader;
			_expressionMetaDataProvider = expressionMetaDataProvider;
		}

		public async Task ReadResultSetAsync<TModel>(Func<IExpressiveReaderAsync<TModel>, Task> callback)
		{
			await ProcessCallbackAsync(callback);
		}

		public async Task ReadResultSetsAsync<TModel1, TModel2>(Func<IExpressiveReaderAsync<TModel1>, Task> callback1, Func<IExpressiveReaderAsync<TModel2>, Task> callback2)
		{
			await ProcessCallbackAsync(callback1);
			if (!await _dataReader.NextResultAsync())
				return;
			await ProcessCallbackAsync(callback2);
		}

		public async Task ReadResultSetsAsync<TModel1, TModel2, TModel3>(Func<IExpressiveReaderAsync<TModel1>, Task> callback1, Func<IExpressiveReaderAsync<TModel2>, Task> callback2, Func<IExpressiveReaderAsync<TModel3>, Task> callback3)
		{
			await ProcessCallbackAsync(callback1);
			if (!await _dataReader.NextResultAsync())
				return;
			await ProcessCallbackAsync(callback2);
			if (!await _dataReader.NextResultAsync())
				return;
			await ProcessCallbackAsync(callback3);
		}

		public async Task<IEnumerable<TModel>> GetModelsForResultSetAsync<TModel>(Func<IExpressiveReaderAsync<TModel>, Task<TModel>> generator)
		{
			return await FillAsync(generator);
		}

		public async Task<ModelsForResultSetsResult> GetModelsForResultSetsAsync<TModel>(Func<IExpressiveReaderAsync<TModel>, Task<TModel>> modelGeneratorFunc)
		{
			var item = await FillAsync(modelGeneratorFunc);

			return new ModelsForResultSetsResult(new IEnumerable[] {item});
		}

		public async Task<ModelsForResultSetsResult> GetModelsForResultSetsAsync<TModel1, TModel2>(
			Func<IExpressiveReaderAsync<TModel1>, Task<TModel1>> modelGeneratorFunc1, 
			Func<IExpressiveReaderAsync<TModel2>, Task<TModel2>> modelGeneratorFunc2)
		{
			var item1 = await FillAsync(modelGeneratorFunc1);
			var item2 = await _dataReader.NextResultAsync() ? await FillAsync(modelGeneratorFunc2) : null;

			return new ModelsForResultSetsResult(new IEnumerable[] {item1, item2});
		}

		public async Task<ModelsForResultSetsResult> GetModelsForResultSetsAsync<TModel1, TModel2, TModel3>(
			Func<IExpressiveReaderAsync<TModel1>, Task<TModel1>> modelGeneratorFunc1, 
			Func<IExpressiveReaderAsync<TModel2>, Task<TModel2>> modelGeneratorFunc2,
			Func<IExpressiveReaderAsync<TModel3>, Task<TModel3>> modelGeneratorFunc3)
		{
			var item1 = await FillAsync(modelGeneratorFunc1);
			var item2 = await _dataReader.NextResultAsync() ? await FillAsync(modelGeneratorFunc2) : null;
			var item3 = item2 != null && await _dataReader.NextResultAsync() ? await FillAsync(modelGeneratorFunc3) : null;

			return new ModelsForResultSetsResult(new IEnumerable[] {item1, item2, item3});
		}

		private async Task ProcessCallbackAsync<TModel>(Func<IExpressiveReaderAsync<TModel>, Task> callback)
		{
			ClearMap();
			var reader = new ExpressiveDbDataReader<TModel>(this, _dataReader, _expressionMetaDataProvider);
			while (await _dataReader.ReadAsync())
				await callback(reader);
		}

		private async Task<IEnumerable<TModel>> FillAsync<TModel>(Func<IExpressiveReaderAsync<TModel>, Task<TModel>> generator)
		{
			ClearMap();
			var list = new List<TModel>();
			var reader = new ExpressiveDbDataReader<TModel>(this, _dataReader, _expressionMetaDataProvider);
			while (await _dataReader.ReadAsync())
			{
				list.Add(await generator(reader));
			}

			return list;
		}
	}
}
