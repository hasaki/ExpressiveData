﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ExpressiveData.Sample.Models.Repositories
{
	class CustomerRepository : SampleDataRepository
	{
		public Customer GetCustomer(int id)
		{
			const string sql = "SELECT * FROM Customers WHERE Id = @id";
			var parameters = new[]
			{
				new SqlParameter("@id", id)
			};

			Customer customer = null;
			Action<IExpressiveReader<Customer>> callback = r =>
			{
				r.Model = customer = new Customer();

				r.Read(m => m.Id);
				r.Read(m => m.Name);
				r.Read(m => m.Address);
				r.Read(m => m.City);
				r.Read(m => m.State);
				r.Read(m => m.ZipCode);
				r.Read(m => m.Region);
			};

			ExecuteQuery(sql, parameters, callback);

			return customer;
		}

		public async Task<Customer> GetCustomerAsync(int id)
		{
			const string sql = "SELECT * FROM Customers WHERE Id = @id";
			var parameters = new[]
			{
				new SqlParameter("@id", id)
			};

			Customer customer = null;
			Func<IExpressiveReaderAsync<Customer>, Task> callback = async r =>
			{
				r.Model = customer = new Customer();

				await r.ReadAsync(m => m.Id);
				await r.ReadAsync(m => m.Name);
				await r.ReadAsync(m => m.Address);
				await r.ReadAsync(m => m.City);
				await r.ReadAsync(m => m.State);
				await r.ReadAsync(m => m.ZipCode);
				await r.ReadAsync(m => m.Region);
			};

			await ExecuteQueryAsync(sql, parameters, callback);

			return customer;
		}

		public IEnumerable<Customer> GetAllCustomers()
		{
			const string sql = "SELECT * FROM Customers";

			var customers = new List<Customer>();
			ExecuteQuery<Customer>(sql, null, r =>
			{
				r.Model = new Customer();

				r.Read(m => m.Id);
				r.Read(m => m.Name);
				r.Read(m => m.Address);
				r.Read(m => m.City);
				r.Read(m => m.State);
				r.Read(m => m.ZipCode);
				r.Read(m => m.Region);

				customers.Add(r.Model);
			});

			return customers;
		}

		public async Task<IEnumerable<Customer>>  GetAllCustomersAsync()
		{
			const string sql = "SELECT * FROM Customers";

			return await ExecuteQueryAsync<Customer>(sql, null, async r =>
			{
				r.Model = new Customer();

				await r.ReadAsync(m => m.Id);
				await r.ReadAsync(m => m.Name);
				await r.ReadAsync(m => m.Address);
				await r.ReadAsync(m => m.City);
				await r.ReadAsync(m => m.State);
				await r.ReadAsync(m => m.ZipCode);
				await r.ReadAsync(m => m.Region);

				return r.Model;
			});
		}
	}
}
