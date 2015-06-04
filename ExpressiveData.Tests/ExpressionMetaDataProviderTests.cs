using NUnit.Framework;

namespace ExpressiveData.Tests
{
	[TestFixture]
	class ExpressionMetaDataProviderTests
	{
		public const string ColumnA = "ColumnA";

		private class TestClass
		{
			[DbAttribute(ColumnName = ColumnA)]
			public string PropertyWithAttribute { get; set; }
			public string PropertyWithoutAttribute { get; set; }
		}

		[Test]
		public void GetsColumnNameFromAttribute()
		{
			var provider = new ExpressionMetaDataProvider();
			var metaData = provider.GetExpressionMetaData<TestClass, string>(p => p.PropertyWithAttribute);

			Assert.AreEqual(ColumnA, metaData.ColumnName);
			Assert.AreEqual(typeof(string), metaData.ResultType);
		}

		[Test]
		public void GetsColumnNameFromPropertyName()
		{
			var provider = new ExpressionMetaDataProvider();
			var metaData = provider.GetExpressionMetaData<TestClass, string>(p => p.PropertyWithoutAttribute);
			
			Assert.AreEqual("PropertyWithoutAttribute", metaData.ColumnName);
			Assert.AreEqual(typeof(string), metaData.ResultType);
		}

		[Test]
		public void CachesMetaDataResults()
		{
			var provider = new ExpressionMetaDataProvider();
			provider.GetExpressionMetaData<TestClass, string>(p => p.PropertyWithoutAttribute);

			var typeCache = provider.MetaData;
			var expressionCache = typeCache[typeof (TestClass)];

			Assert.AreEqual(1, typeCache.Count);
			Assert.AreEqual(1, expressionCache.Count);
			
			provider.GetExpressionMetaData<TestClass, string>(p => p.PropertyWithoutAttribute);
			Assert.AreEqual(1, typeCache.Count);
			Assert.AreEqual(1, expressionCache.Count);

			provider.GetExpressionMetaData<TestClass, string>(p => p.PropertyWithAttribute);
			Assert.AreEqual(1, typeCache.Count);
			Assert.AreEqual(2, expressionCache.Count);
		}
	}
}
