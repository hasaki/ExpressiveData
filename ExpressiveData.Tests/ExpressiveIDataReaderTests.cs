using System;
using System.Data;
using Moq;
using NUnit.Framework;

namespace ExpressiveData.Tests
{
	[TestFixture]
	class ExpressiveIDataReaderTests
	{
		private const string Ord1 = "Ord1";
		private readonly DateTime Ord2 = DateTime.Now;
		private const int Ord3 = 3;
		private const float Ord4 = 3.14f;
		private const double Ord5 = Math.PI;
		private const decimal Ord6 = 6M;
		private readonly Guid Ord7 = Guid.NewGuid();

		public class TestClass
		{
			public string Ord1 { get; set; }
			public DateTime Ord2 { get; set; }
			public int Ord3 { get; set; }
			public float Ord4 { get; set; }
			public double Ord5 { get; set; }
			public decimal Ord6 { get; set; }
			public Guid Ord7 { get; set; }
		}

		private IDataReader GetIDataReader()
		{
			var values = new object[] {Ord1, Ord2, Ord3, Ord4, Ord5, Ord6, Ord7};
			var readerMock = new Mock<IDataReader>();

			for (int i = 1; i < 8; i++)
			{
				var ordinal = i;
				var name = "Ord" + ordinal;
				readerMock.Setup(reader => reader.GetOrdinal(name)).Returns(ordinal - 1);
				readerMock.Setup(reader => reader.GetValue(ordinal - 1)).Returns(values[ordinal - 1]);
			}

			return readerMock.Object;
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

			Assert.AreEqual(Ord1, model.Ord1);
			Assert.AreEqual(Ord2, model.Ord2);
			Assert.AreEqual(Ord3, model.Ord3);
			Assert.AreEqual(Ord4, model.Ord4);
			Assert.AreEqual(Ord5, model.Ord5);
			Assert.AreEqual(Ord6, model.Ord6);
			Assert.AreEqual(Ord7, model.Ord7);
		}
	}
}
