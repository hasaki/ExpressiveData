using NUnit.Framework;

namespace ExpressiveData.Tests
{
	[TestFixture]
	class ExpressiveIDataReaderTests : BaseReaderTests
	{
		[Test]
		public void ReadsFromReaderAndAssignsToModel()
		{
			var model = new TestClass();

			var fakeReader = GetDbDataReader();
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

			var fakeReader = GetDbDataReaderForChangingTypes();
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
