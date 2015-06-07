using System;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ExpressiveData
{
	internal class ExpressiveDbDataReader<TModel> : ExpressiveIDataReader<TModel>, IExpressiveReaderAsync<TModel>
	{
		private readonly IOrdinalProvider _ordinalProvider;
		private readonly DbDataReader _reader;
		private readonly ExpressionMetaDataProvider _metaDataProvider;

		internal ExpressiveDbDataReader(IOrdinalProvider ordinalProvider, DbDataReader reader, ExpressionMetaDataProvider metaDataProvider) 
			: base(ordinalProvider, reader, metaDataProvider)
		{
			_ordinalProvider = ordinalProvider;
			_reader = reader;
			_metaDataProvider = metaDataProvider;
		}

		public async Task ReadAsync<TResult>(Expression<Func<TModel, TResult>> expression)
		{
			await ReadAsync(expression, default(TResult));
		}

		public async Task ReadAsync<TResult>(Expression<Func<TModel, TResult>> expression, TResult defaultValue)
		{
			await ReadFromAsync(expression, null, defaultValue);
		}

		public async Task ReadFromAsync<TResult>(Expression<Func<TModel, TResult>> expression, string columnName)
		{
			await ReadFromAsync(expression, columnName, default(TResult));
		}

		public async Task ReadFromAsync<TResult>(Expression<Func<TModel, TResult>> expression, string columnName, TResult defaultValue)
		{
			await Read(expression, columnName, defaultValue);
		}

		private async Task Read<TResult>(Expression<Func<TModel, TResult>> expression, string columnName, TResult defaultValue)
		{
			var metaData = _metaDataProvider.GetExpressionMetaData(expression);
			if (metaData == null)
				throw new InvalidOperationException("Cannot create meta-data for this expression");

			columnName = columnName ?? metaData.ColumnName;
			var value = await ReadValue(columnName, defaultValue, metaData.DatabaseType);

			var setter = metaData.SetValueFn as Action<TModel, TResult>;
			if (setter == null)
				throw new InvalidOperationException("Could not convert the setter back to the correct type!");
			
			setter(Model, value);
		}

		private async Task<TResult> ReadValue<TResult>(string columnName, TResult defaultValue, Type databaseType)
		{
			var ordinal = _ordinalProvider.GetOrdinal(columnName);

			var isNull = await _reader.IsDBNullAsync(ordinal);
			if (isNull)
				return defaultValue;

			var resultType = typeof (TResult);
			if (databaseType == resultType)
				return await _reader.GetFieldValueAsync<TResult>(ordinal);
			
			var value = await ReadValueAsync(ordinal);
			return CoerseType<TResult>(databaseType, resultType, value);
		}

		private async Task<object> ReadValueAsync(int ordinal)
		{
			return await Task.FromResult(_reader.GetValue(ordinal));
		}
	}
}
