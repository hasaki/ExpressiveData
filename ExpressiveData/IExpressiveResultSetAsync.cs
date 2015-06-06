using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExpressiveData
{
	public interface IExpressiveResultSetAsync
	{
		Task<IEnumerable<TModel>> FillModelsAsync<TModel>(
			Func<IExpressiveReaderAsync<TModel>, Task<TModel>> generator);

		Task<Tuple<IEnumerable<TModel1>, IEnumerable<TModel2>>> FillModelsAsync<TModel1, TModel2>(
			Func<IExpressiveReaderAsync<TModel1>, Task<TModel1>> generator1,
			Func<IExpressiveReaderAsync<TModel2>, Task<TModel2>> generator2);

		Task<Tuple<IEnumerable<TModel1>, IEnumerable<TModel2>, IEnumerable<TModel3>>> FillModelsAsync<TModel1, TModel2, TModel3>(
			Func<IExpressiveReaderAsync<TModel1>, Task<TModel1>> generator1,
			Func<IExpressiveReaderAsync<TModel2>, Task<TModel2>> generator2,
			Func<IExpressiveReaderAsync<TModel3>, Task<TModel3>> generator3);
	}
}
