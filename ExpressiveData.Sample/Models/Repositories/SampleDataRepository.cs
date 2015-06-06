using System.Configuration;
using System.Data.SqlClient;

namespace ExpressiveData.Sample.Models.Repositories
{
	public abstract class SampleDataRepository
	{
		protected SqlConnection GetDb()
		{
			var connectionString = ConfigurationManager.ConnectionStrings["SampleDB"].ConnectionString;
			return new SqlConnection(connectionString);
		}
	}
}
