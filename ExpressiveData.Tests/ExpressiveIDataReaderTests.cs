using System;
using System.Data;
using System.Data.Common;
using Moq;
using NUnit.Framework;

namespace ExpressiveData.Tests
{
	[TestFixture]
	class ExpressiveIDataReaderTests : BaseReaderTests
	{
		private IDataReader GetIDataReader()
		{
			var values = new object[] {Ord1, Ord2, Ord3, Ord4, Ord5, Ord6, Ord7};
			var readerMock = new Mock<IDataReader>();

			for (int i = 1; i <= values.Length; i++)
			{
				var ordinal = i;
				var name = "Ord" + ordinal;
				SetupMock(readerMock, name, ordinal - 1, values[ordinal - 1]);
			}
			SetupMock(readerMock, "Ord8", 7, Ord8.ToString());

			return readerMock.Object;
		}

		private IDataReader GetIDataReaderForChangingTypes()
		{
			var readerMock = new Mock<IDataReader>();

			SetupMock(readerMock, "Guid", 0, Ord7.ToString());
			SetupMock(readerMock, "Enum1", 1, Enum1.ToString());
			SetupMock(readerMock, "Enum2", 2, (int) Enum2);
			SetupMock(readerMock, "PiFloat", 3, PiFloat);

			return readerMock.Object;
		}

		private void SetupMock<TValue>(Mock<IDataReader> mock, string column, int ordinal, TValue value)
		{
			mock.Setup(reader => reader.GetOrdinal(column)).Returns(ordinal);
			mock.Setup(reader => reader.GetValue(ordinal)).Returns(value);
		}

		[Test]
		public void ReadsFromReaderAndAssignsToModel()
		{
			var model = new TestClass();

			var fakeReader = GetIDataReader();
			var metaDataProvider = new ExpressionMetaDataProvider();
			var wrapper = new ExpressiveIDataReaderResultSet(fakeReader, metaDataProvider);
			var context = new ExpressiveIDataReader<TestClass>(wrapper, fakeReader, metaDataProvider)
			{
				Model = model
			};

			context.Read(m => m.Ord1);
			context.Read(m => m.Ord2);
			context.Read(m => m.Ord3);
			context.Read(m => m.Ord4);
			context.Read(m => m.Ord5);
			context.Read(m => m.Ord6);
			context.Read(m => m.Ord7);
			context.Read(m => m.Ord8);

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
		public void ReadsFromReaderWithOneTypeAndAssignsToDifferentType()
		{
			var model = new TestClassChangingTypes();

			var fakeReader = GetIDataReaderForChangingTypes();
			var metaDataProvider = new ExpressionMetaDataProvider();
			var wrapper = new ExpressiveIDataReaderResultSet(fakeReader, metaDataProvider);
			var context = new ExpressiveIDataReader<TestClassChangingTypes>(wrapper, fakeReader, metaDataProvider)
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
