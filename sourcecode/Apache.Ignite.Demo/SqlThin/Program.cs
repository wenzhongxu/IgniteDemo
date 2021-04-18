using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Cache.Query;
using Apache.Ignite.Core.Client;
using Apache.Ignite.Core.Client.Cache;
using Entities;
using System;

namespace SqlThin
{
    class Program
    {
        //ignite client配置
        private static readonly IgniteClientConfiguration igniteClientConfiguration = new IgniteClientConfiguration
        {
            Endpoints = new[] { "127.0.0.1" }
        };

        private const string EmployeeCacheName = "dotnet_cache_query_employee";

        public static void Main(string[] args)
        {

            using (var ignite = Ignition.StartClient(igniteClientConfiguration))
            {
                var employeeCache = ignite.GetOrCreateCache<int, Employee>(
                    new CacheClientConfiguration(EmployeeCacheName, new QueryEntity(typeof(int), typeof(Employee))));

                PopulateCache(employeeCache);

                SqlQueryExample(employeeCache);

                Console.ReadKey();
            }
        }


        private static void PopulateCache(ICacheClient<int, Employee> cache)
        {
            cache.Put(1, new Employee(
                "James Wilson",
                12500,
                new Address("1096 Eddy Street, San Francisco, CA", 94109),
                new[] { "Human Resources", "Customer Service" },
                1));

            cache.Put(2, new Employee(
                "Daniel Adams",
                11000,
                new Address("184 Fidler Drive, San Antonio, TX", 78130),
                new[] { "Development", "QA" },
                1));

            cache.Put(3, new Employee(
                "Cristian Moss",
                12500,
                new Address("667 Jerry Dove Drive, Florence, SC", 29501),
                new[] { "Logistics" },
                1));

            cache.Put(4, new Employee(
                "Allison Mathis",
                25300,
                new Address("2702 Freedom Lane, San Francisco, CA", 94109),
                new[] { "Development" },
                2));

            cache.Put(5, new Employee(
                "Breana Robbin",
                6500,
                new Address("3960 Sundown Lane, Austin, TX", 78130),
                new[] { "Sales" },
                2));

            cache.Put(6, new Employee(
                "Philip Horsley",
                19800,
                new Address("2803 Elsie Drive, Sioux Falls, SD", 57104),
                new[] { "Sales" },
                2));

            cache.Put(7, new Employee(
                "Brian Peters",
                10600,
                new Address("1407 Pearlman Avenue, Boston, MA", 12110),
                new[] { "Development", "QA" },
                2));
        }

        private static void SqlQueryExample(ICacheClient<int, Employee> cache)
        {
            const int zip = 94109;

            var qry = cache.Query(new SqlFieldsQuery("select name, salary from Employee where zip = ?", zip));

            Console.WriteLine();
            Console.WriteLine(">>> Employees with zipcode {0} (SQL):", zip);

            foreach (var row in qry)
            {
                Console.WriteLine(">>>     [Name=" + row[0] + ", salary=" + row[1] + ']');
            }
        }
    }
}
