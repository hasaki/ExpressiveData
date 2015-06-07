using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExpressiveData
{
	public interface IExpressiveResultSetAsync : IExpressiveResultSet
	{
		Task ReadResultSetAsync<TModel>(Func<IExpressiveReaderAsync<TModel>, Task> callback);

		Task ReadResultSetsAsync<TModel1, TModel2>(
			Func<IExpressiveReaderAsync<TModel1>, Task> callback1,
			Func<IExpressiveReaderAsync<TModel2>, Task> callback2);

		Task ReadResultSetsAsync<TModel1, TModel2, TModel3>(
			Func<IExpressiveReaderAsync<TModel1>, Task> callback1,
			Func<IExpressiveReaderAsync<TModel2>, Task> callback2,
			Func<IExpressiveReaderAsync<TModel3>, Task> callback3);

		Task<IEnumerable<TModel>> GetModelsForResultSetAsync<TModel>(
			Func<IExpressiveReaderAsync<TModel>, Task<TModel>> modelGeneratorFunc);

		Task<ModelsForResultSetsResult> GetModelsForResultSetsAsync<TModel>(
			Func<IExpressiveReaderAsync<TModel>, Task<TModel>> modelGeneratorFunc);

		Task<ModelsForResultSetsResult> GetModelsForResultSetsAsync<TModel1, TModel2>(
			Func<IExpressiveReaderAsync<TModel1>, Task<TModel1>> modelGeneratorFunc1,
			Func<IExpressiveReaderAsync<TModel2>, Task<TModel2>> modelGeneratorFunc2);

		Task<ModelsForResultSetsResult> GetModelsForResultSetsAsync<TModel1, TModel2, TModel3>(
			Func<IExpressiveReaderAsync<TModel1>, Task<TModel1>> modelGeneratorFunc1,
			Func<IExpressiveReaderAsync<TModel2>, Task<TModel2>> modelGeneratorFunc2,
			Func<IExpressiveReaderAsync<TModel3>, Task<TModel3>> modelGeneratorFunc3);
	}
}
