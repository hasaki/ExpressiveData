using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExpressiveData
{
	public interface IExpressiveResultSet
	{
		void ReadResultSet<TModel>(Action<IExpressiveReader<TModel>> callback);

		void ReadResultSets<TModel1, TModel2>(
			Action<IExpressiveReader<TModel1>> callback1,
			Action<IExpressiveReader<TModel2>> callback2);

		void ReadResultSets<TModel1, TModel2, TModel3>(
			Action<IExpressiveReader<TModel1>> callback1,
			Action<IExpressiveReader<TModel2>> callback2,
			Action<IExpressiveReader<TModel3>> callback3);

		IEnumerable<TModel> GetModelsForResultSet<TModel>(
			Func<IExpressiveReader<TModel>, TModel> modelGeneratorFunc);

		ModelsForResultSetsResult GetModelsForResultSets<TModel>(
			Func<IExpressiveReader<TModel>, TModel> modelGeneratorFunc);

		ModelsForResultSetsResult GetModelsForResultSets<TModel1, TModel2>(
			Func<IExpressiveReader<TModel1>, TModel1> modelGeneratorFunc1,
			Func<IExpressiveReader<TModel2>, TModel2> modelGeneratorFunc2);

		ModelsForResultSetsResult GetModelsForResultSets<TModel1, TModel2, TModel3>(
			Func<IExpressiveReader<TModel1>, TModel1> modelGeneratorFunc1,
			Func<IExpressiveReader<TModel2>, TModel2> modelGeneratorFunc2,
			Func<IExpressiveReader<TModel3>, TModel3> modelGeneratorFunc3);
	}
}
