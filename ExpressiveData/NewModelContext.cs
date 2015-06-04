using System;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;

namespace ExpressiveData
{
	internal class NewModelContext<TModel> : INewModelContext<TModel>
	{
		private readonly DataReaderWrapper _context;
		private readonly IDataReader _reader;
		private readonly TModel _model;

		internal NewModelContext(DataReaderWrapper context, TModel model)
		{
			_context = context;
			_reader = context.DataReader;

			_model = model;
		}

		public TModel Model { get { return _model; } }

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
			var metaData = _context.MetaDataProvider.GetExpressionMetaData(expression);
			if (metaData == null)
				throw new InvalidOperationException("Cannot create meta-data for this expression");

			columnName = columnName ?? metaData.ColumnName;
			var value = ReadValue(columnName, defaultValue);

			var setter = metaData.SetValueFn as Action<TModel, TResult>;
			if (setter == null)
				throw new InvalidOperationException("Could not convert the setter back to the correct type!");

			setter(_model, value);
		}

		private TResult ReadValue<TResult>(string columnName, TResult defaultValue)
		{
			var ordinal = _context.GetOrdinal(columnName);

			if (_reader.IsDBNull(ordinal))
				return defaultValue;

			var readerFunc = GetDbReaderFuncForType(typeof (TResult));

			return (TResult) readerFunc(ordinal);
		}

		private Func<int, object> GetDbReaderFuncForType(Type returnType)
		{
			if (returnType == typeof (string))
				return _reader.GetString;

			return GetObject;
		}

		private TResult GetObject<TResult>(int ordinal)
		{
			return (TResult) GetObject(ordinal, typeof (TResult));
		}

		private object GetObject(int ordinal)
		{
			return _reader.GetValue(ordinal);
		}

		private object GetObject(int ordinal, Type type)
		{
			return Convert.ChangeType(GetObject(ordinal), type);
		}
	}
}
