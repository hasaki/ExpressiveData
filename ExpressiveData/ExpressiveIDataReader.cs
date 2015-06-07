using System;
using System.Data;
using System.Linq.Expressions;

namespace ExpressiveData
{
	internal class ExpressiveIDataReader<TModel> : IExpressiveReader<TModel>
	{
		private readonly IOrdinalProvider _ordinalProvider;
		private readonly IDataReader _reader;
		private readonly ExpressionMetaDataProvider _metaDataProvider;

		internal ExpressiveIDataReader(IOrdinalProvider ordinalProvider, IDataReader reader, ExpressionMetaDataProvider metaDataProvider)
		{
			_ordinalProvider = ordinalProvider;
			_reader = reader;
			_metaDataProvider = metaDataProvider;
		}

		public TModel Model { get; set; }

		public void Read<TResult>(Expression<Func<TModel, TResult>> expression)
		{
			Read(expression, null, default(TResult));
		}

		public void Read<TResult>(Expression<Func<TModel, TResult>> expression, TResult defaultValue)
		{
			Read(expression, null, defaultValue);
		}

		public void ReadFrom<TResult>(Expression<Func<TModel, TResult>> expression, string columnName)
		{
			Read(expression, columnName, default(TResult));
		}

		public void ReadFrom<TResult>(Expression<Func<TModel, TResult>> expression, string columnName, TResult defaultValue)
		{
			Read(expression, columnName, defaultValue);
		}

		private void Read<TResult>(Expression<Func<TModel, TResult>> expression, string columnName, TResult defaultValue)
		{
			var metaData = _metaDataProvider.GetExpressionMetaData(expression);
			if (metaData == null)
				throw new InvalidOperationException("Cannot create meta-data for this expression");

			columnName = columnName ?? metaData.ColumnName;
			var value = ReadValue(columnName, defaultValue, metaData.DatabaseType);

			var setter = metaData.SetValueFn as Action<TModel, TResult>;
			if (setter == null)
				throw new InvalidOperationException("Could not convert the setter back to the correct type!");

			setter(Model, value);
		}

		private TResult ReadValue<TResult>(string columnName, TResult defaultValue, Type databaseType)
		{
			var ordinal = _ordinalProvider.GetOrdinal(columnName);

			if (_reader.IsDBNull(ordinal))
				return defaultValue;

			var resultType = typeof (TResult);
			var value = _reader.GetValue(ordinal);
			if (databaseType == resultType)
				return (TResult) value;

			return CoerseType<TResult>(databaseType, resultType, value);
		}

		protected TResult CoerseType<TResult>(Type databaseType, Type resultType, object value)
		{
			if (!resultType.IsEnum)
				return (TResult) ChangeType(value, databaseType, resultType);

			if (databaseType == typeof (string))
				return (TResult) Enum.Parse(resultType, (string) value);

			return (TResult) value;
		}

		private object ChangeType(object value, Type dbType, Type resultType)
		{
			if (resultType == typeof (Guid))
				return ChangeToGuid(value, dbType);

			return Convert.ChangeType(value, resultType);
		}

		private Guid ChangeToGuid(object value, Type valueType)
		{
			if (valueType == typeof (string))
				return Guid.Parse((string) value);
			if (valueType == typeof (byte[]))
				return new Guid((byte[]) value);

			var msg = string.Format("Cannot convert from {0} ({1}) to GUID", value, valueType);
			throw new ArgumentException(msg);
		}
	}
}
