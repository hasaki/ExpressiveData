using System.Threading.Tasks;
using NUnit.Framework;

namespace ExpressiveData.Tests
{
	class ExpressiveDbDataReaderTests : BaseReaderTests
	{
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

			await context.ReadAsync(m => m.Guid);
			await context.ReadAsync(m => m.Enum1);
			await context.ReadAsync(m => m.Enum2);
			await context.ReadAsync(m => m.PiFloat);

			Assert.AreEqual(Ord7, model.Guid);
			Assert.AreEqual(Enum1, model.Enum1);
			Assert.AreEqual(Enum2, model.Enum2);
			Assert.AreEqual(PiFloat, model.PiFloat);
		}
	}
}
