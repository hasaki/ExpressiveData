using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ExpressiveData
{
	public interface IExpressiveReaderAsync<TModel> : IExpressiveReader<TModel>
	{
		Task ReadAsync<TResult>(Expression<Func<TModel, TResult>> expression);
		Task ReadAsync<TResult>(Expression<Func<TModel, TResult>> expression, TResult defaultValue);
		Task ReadFromAsync<TResult>(Expression<Func<TModel, TResult>> expression, string columnName);
		Task ReadFromAsync<TResult>(Expression<Func<TModel, TResult>> expression, string columnName, TResult defaultValue);
	}
}
