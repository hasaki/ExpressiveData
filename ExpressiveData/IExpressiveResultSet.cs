using System;
using System.Collections.Generic;

namespace ExpressiveData
{
	public interface IExpressiveResultSet
	{
		IEnumerable<TModel> FillModels<TModel>(Func<IExpressiveReader<TModel>, TModel> generator);

		Tuple<IEnumerable<TModel1>, IEnumerable<TModel2>> FillModels<TModel1, TModel2>(
			Func<IExpressiveReader<TModel1>, TModel1> generator1, 
			Func<IExpressiveReader<TModel2>, TModel2> generator2);

		Tuple<IEnumerable<TModel1>, IEnumerable<TModel2>, IEnumerable<TModel3>> FillModels<TModel1, TModel2, TModel3>(
			Func<IExpressiveReader<TModel1>, TModel1> generator1, 
			Func<IExpressiveReader<TModel2>, TModel2> generator2,
			Func<IExpressiveReader<TModel3>, TModel3> generator3);
	}
}
