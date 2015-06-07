using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;

namespace ExpressiveData.Tests
{
	// ick, seems there is a bug in Moq that prevents
	// Mock<DbDataReader> from working properly when dealing with async methods
	// The debugger will step into the my Returns() callback, and return the value
	// yet when the code calling it returns the value would be null
	// Creating our own implementation works fine though
	class MyDbDataReader : DbDataReader
	{
		private readonly IDictionary<string, int> _ordinalMap;
		private readonly IDictionary<int, object> _valueMap;

		public MyDbDataReader(IDictionary<string, int> ordinalMap, IDictionary<int, object> valueMap)
		{
			if (ordinalMap == null)
				throw new ArgumentNullException("ordinalMap");
			if (valueMap == null)
				throw new ArgumentNullException("valueMap");

			_ordinalMap = ordinalMap;
			_valueMap = valueMap;
		}

		public override bool IsDBNull(int ordinal)
		{
			return false;
		}

		public override object this[int ordinal]
		{
			get { return _valueMap[ordinal]; }
		}

		public override object this[string name]
		{
			get { return _valueMap[_ordinalMap[name]]; }
		}

		public override int GetOrdinal(string name)
		{
			return _ordinalMap[name];
		}

		public override object GetValue(int ordinal)
		{
			return _valueMap[ordinal];
		}

		public override bool NextResult()
		{
			return false;
		}

		private bool _hasRead = false;
		public override bool Read()
		{
			if (_hasRead)
				return false;

			_hasRead = true;
			return true;
		}

		public override int FieldCount
		{
			get { return _valueMap.Count; }
		}

		#region Not Implemented

		public override string GetName(int ordinal)
		{
			throw new NotImplementedException();
		}

		public override int GetValues(object[] values)
		{
			throw new NotImplementedException();
		}

		public override bool HasRows
		{
			get { throw new NotImplementedException(); }
		}

		public override bool IsClosed
		{
			get { throw new NotImplementedException(); }
		}

		public override int RecordsAffected
		{
			get { throw new NotImplementedException(); }
		}

		public override int Depth
		{
			get { throw new NotImplementedException(); }
		}

		public override bool GetBoolean(int ordinal)
		{
			throw new NotImplementedException();
		}

		public override byte GetByte(int ordinal)
		{
			throw new NotImplementedException();
		}

		public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
		{
			throw new NotImplementedException();
		}

		public override char GetChar(int ordinal)
		{
			throw new NotImplementedException();
		}

		public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
		{
			throw new NotImplementedException();
		}

		public override Guid GetGuid(int ordinal)
		{
			throw new NotImplementedException();
		}

		public override short GetInt16(int ordinal)
		{
			throw new NotImplementedException();
		}

		public override int GetInt32(int ordinal)
		{
			throw new NotImplementedException();
		}

		public override long GetInt64(int ordinal)
		{
			throw new NotImplementedException();
		}

		public override DateTime GetDateTime(int ordinal)
		{
			throw new NotImplementedException();
		}

		public override string GetString(int ordinal)
		{
			throw new NotImplementedException();
		}

		public override decimal GetDecimal(int ordinal)
		{
			throw new NotImplementedException();
		}

		public override double GetDouble(int ordinal)
		{
			throw new NotImplementedException();
		}

		public override float GetFloat(int ordinal)
		{
			throw new NotImplementedException();
		}

		public override string GetDataTypeName(int ordinal)
		{
			throw new NotImplementedException();
		}

		public override Type GetFieldType(int ordinal)
		{
			throw new NotImplementedException();
		}

		public override IEnumerator GetEnumerator()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
