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

			return innerDictionary.GetOrAdd(expression.ToString(), _ => GetExpressionMetaDataImpl(expression, new ExpressionMetaData
			{
				DatabaseType = typeof(TResult).IsEnum ? typeof(string) : typeof(TResult),
				ResultType = typeof(TResult),
				SetValueFn = GetSetter(expression)
			}));
		}

		private ExpressionMetaData GetExpressionMetaDataImpl<TModel, TResult>(Expression<Func<TModel, TResult>> expression, ExpressionMetaData metaData)
		{
			var memberAccess = expression.Body as MemberExpression;

			if (memberAccess != null)
				UpdateMetaDataForMemberInfo(metaData, memberAccess.Member);

			return metaData;
		}

		private void UpdateMetaDataForMemberInfo(ExpressionMetaData metaData, MemberInfo memberInfo)
		{
			var att =
				memberInfo.GetCustomAttributes(typeof (ExpressiveColumnAttribute))
					.Cast<ExpressiveColumnAttribute>()
					.SingleOrDefault();

			var useFieldName = (att == null || string.IsNullOrWhiteSpace(att.ColumnName));
			metaData.ColumnName = useFieldName ? memberInfo.Name : att.ColumnName;

			if (att != null && att.DatabaseType != null)
				metaData.DatabaseType = att.DatabaseType;
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
