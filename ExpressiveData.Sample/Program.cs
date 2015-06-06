using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExpressiveData.Sample.Models;
using ExpressiveData.Sample.Models.Repositories;

namespace ExpressiveData.Sample
{
	class Program
	{
		static void Main()
		{
			var repo = new CustomerRepository();
			var customers = repo.GetAllCustomers();
			PrintCustomers(customers, "Non-Async");

			// if we could be async then this would use await, 
			// but the entry point can't be async
			customers = repo.GetAllCustomersAsync().Result;
			PrintCustomers(customers, "Async");
		}

		static void PrintCustomers(IEnumerable<Customer> customers, string header)
		{
			Console.WriteLine();
			Console.WriteLine("===== {0} =====", header);
			foreach (var customer in customers)
				PrintCustomer(customer);
		}

		static void PrintCustomer(Customer customer)
		{
			Console.WriteLine("{1,3:000} {0,-50}", customer.Name, customer.Id);
			Console.WriteLine("{0,-50}", customer.Address);
			Console.WriteLine("{0}, {1}  {2}", customer.City, customer.State, customer.ZipCode);
			Console.WriteLine();
		}
	}
}
