using System;
using System.Linq.Expressions;

namespace ExpressiveData
{
	public interface IExpressiveReader<TModel>
	{
		TModel Model { get; set; }

		void Read<TResult>(Expression<Func<TModel, TResult>> expression);
		void Read<TResult>(Expression<Func<TModel, TResult>> expression, TResult defaultValue);
		void ReadFrom<TResult>(Expression<Func<TModel, TResult>> expression, string columnName);
		void ReadFrom<TResult>(Expression<Func<TModel, TResult>> expression, string columnName, TResult defaultValue);
	}
}
