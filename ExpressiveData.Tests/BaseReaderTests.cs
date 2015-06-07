using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
	}
}
