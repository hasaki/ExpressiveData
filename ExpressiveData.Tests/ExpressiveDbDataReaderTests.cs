using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Moq;
using NUnit.Framework;

namespace ExpressiveData.Tests
{
	class ExpressiveDbDataReaderTests : BaseReaderTests
	{
		private DbDataReader GetDbDataReader()
		{
			var ordinalMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
			var valueMap = new Dictionary<int, object>();
			var values = new object[] { Ord1, Ord2, Ord3, Ord4, Ord5, Ord6, Ord7 };

			for (int i = 1; i <= values.Length; i++)
			{
				var ordinal = i;
				var name = "Ord" + ordinal;
				SetupMock(name, ordinal - 1, values[ordinal - 1], ordinalMap, valueMap);
			}
			SetupMock("Ord8", 7, Ord8.ToString(), ordinalMap, valueMap);

			return new MyDbDataReader(ordinalMap, valueMap);
		}

		private DbDataReader GetDbDataReaderForChangingTypes()
		{
			var ordinalMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
			var valueMap = new Dictionary<int, object>();

			SetupMock("Guid", 0, Ord7.ToString(), ordinalMap, valueMap);
			SetupMock("Enum1", 1, Enum1.ToString(), ordinalMap, valueMap);
			SetupMock("Enum2", 2, (int)Enum2, ordinalMap, valueMap);
			SetupMock("PiFloat", 3, PiFloat, ordinalMap, valueMap);

			return new MyDbDataReader(ordinalMap, valueMap);
		}

		private void SetupMock(string column, int ordinal, object value, IDictionary<string, int> ordinalMap, IDictionary<int, object> valueMap)
		{
			ordinalMap.Add(column, ordinal);
			valueMap.Add(ordinal, value);
		}

		[Test]
		public async Task MockDbDataReaderFails()
		{
			var mock = new Mock<DbDataReader>(MockBehavior.Strict);

			mock.Setup(reader => reader.GetOrdinal("Test")).Returns(1);
			const string expectedString = "Hello";
			mock.Setup(reader => reader.GetValue(1)).Returns(expectedString);
			mock.Setup(reader => reader.GetFieldValue<string>(1)).Returns(expectedString);
			mock.Setup(reader => reader.GetFieldValueAsync<string>(1, new CancellationToken())).ReturnsAsync(expectedString);
			mock.Setup(reader => reader.IsDBNull(1)).Returns(false);

			var dataReader = mock.Object;

			var ordinal = dataReader.GetOrdinal("Test");
			Assert.AreEqual(1, ordinal);

			var nonAsyncValue = (string) dataReader.GetFieldValue<string>(ordinal);
			Assert.AreEqual(expectedString, nonAsyncValue);

			var asyncValue = await dataReader.GetFieldValueAsync<string>(ordinal);
			Assert.AreEqual(expectedString, asyncValue);
		}

		[Test]
		public async Task ReadsFromReaderAndAssignsToModelAsync()
		{
			var model = new TestClass();

			var fakeReader = GetDbDataReader();
			var metaDataProvider = new ExpressionMetaDataProvider();
			var wrapper = new ExpressiveDbDataReaderResultSet(fakeReader, metaDataProvider);
			var context = new ExpressiveDbDataReader<TestClass>(wrapper, fakeReader, metaDataProvider)
			{
				Model = model
			};

			await context.ReadAsync(m => m.Ord1);
			await context.ReadAsync(m => m.Ord2);
			await context.ReadAsync(m => m.Ord3);
			await context.ReadAsync(m => m.Ord4);
			await context.ReadAsync(m => m.Ord5);
			await context.ReadAsync(m => m.Ord6);
			await context.ReadAsync(m => m.Ord7);
			await context.ReadAsync(m => m.Ord8);

			Assert.AreEqual(Ord1, model.Ord1);
			Assert.AreEqual(Ord2, model.Ord2);
			Assert.AreEqual(Ord3, model.Ord3);
			Assert.AreEqual(Ord4, model.Ord4);
			Assert.AreEqual(Ord5, model.Ord5);
			Assert.AreEqual(Ord6, model.Ord6);
			Assert.AreEqual(Ord7, model.Ord7);
			Assert.AreEqual(Ord8, model.Ord8);
		}

		[Test]
		public async Task ReadsFromReaderWithOneTypeAndAssignsToDifferentTypeAsync()
		{
			var model = new TestClassChangingTypes();

			var fakeReader = GetDbDataReaderForChangingTypes();
			var metaDataProvider = new ExpressionMetaDataProvider();
			var wrapper = new ExpressiveDbDataReaderResultSet(fakeReader, metaDataProvider);
			var context = new ExpressiveDbDataReader<TestClassChangingTypes>(wrapper, fakeReader, metaDataProvider)
			{
				Model = model
			};

			context.Read(m => m.Guid);
			context.Read(m => m.Enum1);
			context.Read(m => m.Enum2);
			context.Read(m => m.PiFloat);

			Assert.AreEqual(Ord7, model.Guid);
			Assert.AreEqual(Enum1, model.Enum1);
			Assert.AreEqual(Enum2, model.Enum2);
			Assert.AreEqual(PiFloat, model.PiFloat);
		}
	}
}
