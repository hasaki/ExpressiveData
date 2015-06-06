using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressiveData.Sample.Models.Repositories
{
	public abstract class SampleDataRepository
	{
		protected void ExecuteQuery<TModel>(string sql, IEnumerable<SqlParameter> parameters,
			Action<IExpressiveReader<TModel>> callback)
		{
			using (var cmd = GetCommand(sql, parameters))
			{
				cmd.Connection.Open();
				using (var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
				{
					var expressive = reader.GetExpressive();
					expressive.ReadResultSet(callback);
				}
			}
		}

		protected IEnumerable<TModel> ExecuteQuery<TModel>(string sql, IEnumerable<SqlParameter> parameters,
			Func<IExpressiveReader<TModel>, TModel> callback)
		{
			using (var cmd = GetCommand(sql, parameters))
			{
				cmd.Connection.Open();
				using (var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
				{
					var expressive = reader.GetExpressive();
					return expressive.GetModelsForResultSet(callback);
				}
			}
		}

		protected async Task ExecuteQueryAsync<TModel>(string sql, IEnumerable<SqlParameter> parameters,
			Func<IExpressiveReaderAsync<TModel>, Task> callback)
		{
			using (var cmd = GetCommand(sql, parameters))
			{
				await cmd.Connection.OpenAsync();
				using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
				{
					var expressive = reader.GetExpressive();
					await expressive.ReadResultSetAsync(callback);
				}
			}
		}

		protected async Task<IEnumerable<TModel>> ExecuteQueryAsync<TModel>(string sql, IEnumerable<SqlParameter> parameters,
			Func<IExpressiveReaderAsync<TModel>, Task<TModel>> callback)
		{
			using (var cmd = GetCommand(sql, parameters))
			{
				await cmd.Connection.OpenAsync();
				using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
				{
					var expressive = reader.GetExpressive();
					return await expressive.GetModelsForResultSetAsync(callback);
				}
			}
		}

		private SqlConnection GetDb()
		{
			var connectionString = ConfigurationManager.ConnectionStrings["SampleDB"].ConnectionString;
			return new SqlConnection(connectionString);
		}

		private SqlCommand GetCommand(string sql, IEnumerable<SqlParameter> parameters)
		{
			parameters = parameters ?? Enumerable.Empty<SqlParameter>();

			var cmd = new SqlCommand(sql, GetDb());
			foreach (var p in parameters)
				cmd.Parameters.Add(p);

			return cmd;
		}
	}
}
