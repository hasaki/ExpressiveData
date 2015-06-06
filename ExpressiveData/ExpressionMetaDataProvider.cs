using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressiveData
{
	internal class ExpressionMetaDataProvider
	{
		internal readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, ExpressionMetaData>> MetaData;

		public ExpressionMetaDataProvider()
		{
			MetaData = new ConcurrentDictionary<Type, ConcurrentDictionary<string, ExpressionMetaData>>();
		}

		public ExpressionMetaData GetExpressionMetaData<TModel, TResult>(Expression<Func<TModel, TResult>> expression)
		{
			var innerDictionary = MetaData.GetOrAdd(typeof(TModel),
				(type) => new ConcurrentDictionary<string, ExpressionMetaData>(StringComparer.OrdinalIgnoreCase));

			return innerDictionary.GetOrAdd(expression.ToString(), _ => GetExpressionMetaDataImpl(expression));
		}

		private ExpressionMetaData GetExpressionMetaDataImpl<TModel, TResult>(Expression<Func<TModel, TResult>> expression)
		{
			var memberAccess = expression.Body as MemberExpression;

			if (memberAccess != null)
			{
				var propertyInfo = memberAccess.Member as PropertyInfo;
				if (propertyInfo != null)
				{
					return new ExpressionMetaData
					{
						ColumnName = GetColumnNameForPropertyInfo(propertyInfo),
						ResultType = typeof(TResult),
						SetValueFn = GetSetter(expression)
					};
				}
			}

			return null;
		}

		private string GetColumnNameForPropertyInfo(PropertyInfo propertyInfo)
		{
			var att = propertyInfo.GetCustomAttributes(typeof(ExpressiveColumnAttribute)).Cast<ExpressiveColumnAttribute>().FirstOrDefault();

			if (att == null)
				return propertyInfo.Name;

			return att.ColumnName;
		}

		// via: http://stackoverflow.com/questions/13769780/how-to-assign-a-value-via-expression
		private Action<TModel, TResult> GetSetter<TModel, TResult>(Expression<Func<TModel, TResult>> expression)
		{
			ParameterExpression valueParameterExpression = Expression.Parameter(typeof(TResult));
			Expression targetExpression = expression.Body is UnaryExpression
				? ((UnaryExpression)expression.Body).Operand
				: expression.Body;

			var assign = Expression.Lambda<Action<TModel, TResult>>
				(
					Expression.Assign(targetExpression, Expression.Convert(valueParameterExpression, targetExpression.Type)),
					expression.Parameters.Single(), valueParameterExpression
				);

			return assign.Compile();
		}
	}
}
