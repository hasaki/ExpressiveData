using System;
using System.Data.Common;
using System.Threading;
using Moq;

namespace ExpressiveData.Tests
{
	class BaseReaderTests
	{
		protected const string Ord1 = "Ord1";
		protected readonly DateTime Ord2 = DateTime.Now;
		protected const int Ord3 = 3;
		protected const float Ord4 = 3.14f;
		protected const double Ord5 = Math.PI;
		protected const decimal Ord6 = 6M;
		protected readonly Guid Ord7 = Guid.NewGuid();
		protected readonly TestEnum Ord8 = TestEnum.Item1;

		protected readonly float PiFloat = (float)Convert.ChangeType(Math.PI, typeof(float));
		protected readonly TestEnum Enum1 = TestEnum.Item1;
		protected const TestEnum Enum2 = TestEnum.Item2;

		public enum TestEnum
		{
			Item1 = 1,
			Item2 = 2,
			Item3 = 3
		}

		public class TestClass
		{
			public string Ord1 { get; set; }
			public DateTime Ord2 { get; set; }
			public int Ord3 { get; set; }
			public float Ord4 { get; set; }
			public double Ord5 { get; set; }
			public decimal Ord6 { get; set; }
			public Guid Ord7 { get; set; }
			public TestEnum Ord8 { get; set; }
		}

		public class TestClassChangingTypes
		{
			[ExpressiveColumn(DatabaseType = typeof(string))]
			public Guid Guid { get; set; }

			public TestEnum Enum1 { get; set; }

			[ExpressiveColumn(DatabaseType = typeof(int))]
			public TestEnum Enum2 { get; set; }

			[ExpressiveColumn(DatabaseType = typeof(double))]
			public float PiFloat { get; set; }
		}

		protected DbDataReader GetDbDataReader()
		{
			var readerMock = new Mock<DbDataReader>(MockBehavior.Strict);

			// Can't use loop, messes up generic types in the Mock
			SetupMock(readerMock, "Ord1", 0, Ord1);
			SetupMock(readerMock, "Ord2", 1, Ord2);
			SetupMock(readerMock, "Ord3", 2, Ord3);
			SetupMock(readerMock, "Ord4", 3, Ord4);
			SetupMock(readerMock, "Ord5", 4, Ord5);
			SetupMock(readerMock, "Ord6", 5, Ord6);
			SetupMock(readerMock, "Ord7", 6, Ord7);
			SetupMock(readerMock, "Ord8", 7, Ord8.ToString());

			return readerMock.Object;
		}

		protected DbDataReader GetDbDataReaderForChangingTypes()
		{
			var readerMock = new Mock<DbDataReader>(MockBehavior.Strict);

			SetupMock(readerMock, "Guid", 0, Ord7.ToString());
			SetupMock(readerMock, "Enum1", 1, Enum1.ToString());
			SetupMock(readerMock, "Enum2", 2, (int)Enum2);
			SetupMock(readerMock, "PiFloat", 3, PiFloat);

			return readerMock.Object;
		}

		private void SetupMock<TValue>(Mock<DbDataReader> mock, string column, int ordinal, TValue value)
		{
			mock.Setup(reader => reader.GetOrdinal(column)).Returns(ordinal);
			mock.Setup(reader => reader.GetValue(ordinal)).Returns(value);
			mock.Setup(reader => reader.GetFieldValue<TValue>(ordinal)).Returns(value);
			mock.Setup(reader => reader.GetFieldValueAsync<TValue>(ordinal, It.IsAny<CancellationToken>())).ReturnsAsync(value);
			mock.Setup(reader => reader.IsDBNull(ordinal)).Returns(false);
			mock.Setup(reader => reader.IsDBNullAsync(ordinal, It.IsAny<CancellationToken>())).ReturnsAsync(false);
		}
	}
}
