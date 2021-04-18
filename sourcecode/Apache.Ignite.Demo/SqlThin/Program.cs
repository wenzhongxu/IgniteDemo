using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache.Affinity;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Cache.Query;
using Apache.Ignite.Core.Client;
using Apache.Ignite.Core.Client.Cache;
using Models;
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

        private const string OrganizationCacheName = "dotnet_cache_query_organization";

        private const string EmployeeCacheNameColocated = "dotnet_cache_query_employee_colocated";

        public static void Main(string[] args)
        {
            //启动一个thin client
            using var ignite = Ignition.StartClient(igniteClientConfiguration);

            //简单sql查询
            var employeeCache = ignite.GetOrCreateCache<int, Employee>(
                new CacheClientConfiguration(EmployeeCacheName, new QueryEntity(typeof(int), typeof(Employee))));
            PopulateCache(employeeCache);
            SqlQueryExample(employeeCache);

            //分布式查询
            var organizationCache = ignite.GetOrCreateCache<int, Organization>(
                    new CacheClientConfiguration(OrganizationCacheName, new QueryEntity(typeof(int), typeof(Organization))));
            PopulateCache(organizationCache);
            SqlDistributedJoinQueryExample(employeeCache);

            //关联查询
            var employeeCacheColocated = ignite.GetOrCreateCache<AffinityKey, Employee>(
                    new CacheClientConfiguration(EmployeeCacheNameColocated,
                        new QueryEntity(typeof(AffinityKey), typeof(Employee))));
            PopulateCache(employeeCacheColocated); 
            SqlJoinQueryExample(employeeCacheColocated);

            Console.ReadKey();
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

        private static void PopulateCache(ICacheClient<int, Organization> cache)
        {
            cache.Put(1, new Organization(
                "Apache",
                new Address("1065 East Hillsdale Blvd, Foster City, CA", 94404),
                OrganizationType.Private,
                DateTime.Now));

            cache.Put(2, new Organization("Microsoft",
                new Address("1096 Eddy Street, San Francisco, CA", 94109),
                OrganizationType.Private,
                DateTime.Now));
        }

        private static void SqlDistributedJoinQueryExample(ICacheClient<int, Employee> cache)
        {
            const string orgName = "Apache";

            var qry = cache.Query(new SqlFieldsQuery(
                "select Employee.name from Employee, \"dotnet_cache_query_organization\".Organization " +
                "where Employee.organizationId = Organization._key and Organization.name = ?", orgName)
            {
                EnableDistributedJoins = true,
                Timeout = new TimeSpan(0, 1, 0)
            });

            Console.WriteLine();
            Console.WriteLine(">>> Employees working for " + orgName + " (distributed joins enabled):");

            foreach (var entry in qry)
                Console.WriteLine(">>>     " + entry[0]);
        }

        private static void PopulateCache(ICacheClient<AffinityKey, Employee> cache)
        {
            cache.Put(new AffinityKey(1, 1), new Employee(
                "James Wilson",
                12500,
                new Address("1096 Eddy Street, San Francisco, CA", 94109),
                new[] { "Human Resources", "Customer Service" },
                1));

            cache.Put(new AffinityKey(2, 1), new Employee(
                "Daniel Adams",
                11000,
                new Address("184 Fidler Drive, San Antonio, TX", 78130),
                new[] { "Development", "QA" },
                1));

            cache.Put(new AffinityKey(3, 1), new Employee(
                "Cristian Moss",
                12500,
                new Address("667 Jerry Dove Drive, Florence, SC", 29501),
                new[] { "Logistics" },
                1));

            cache.Put(new AffinityKey(4, 2), new Employee(
                "Allison Mathis",
                25300,
                new Address("2702 Freedom Lane, San Francisco, CA", 94109),
                new[] { "Development" },
                2));

            cache.Put(new AffinityKey(5, 2), new Employee(
                "Breana Robbin",
                6500,
                new Address("3960 Sundown Lane, Austin, TX", 78130),
                new[] { "Sales" },
                2));

            cache.Put(new AffinityKey(6, 2), new Employee(
                "Philip Horsley",
                19800,
                new Address("2803 Elsie Drive, Sioux Falls, SD", 57104),
                new[] { "Sales" },
                2));

            cache.Put(new AffinityKey(7, 2), new Employee(
                "Brian Peters",
                10600,
                new Address("1407 Pearlman Avenue, Boston, MA", 12110),
                new[] { "Development", "QA" },
                2));
        }

        private static void SqlJoinQueryExample(ICacheClient<AffinityKey, Employee> cache)
        {
            const string orgName = "Apache";

            var qry = cache.Query(new SqlFieldsQuery(
                "select Employee.name from Employee, \"dotnet_cache_query_organization\".Organization " +
                "where Employee.organizationId = Organization._key and Organization.name = ?", orgName));

            Console.WriteLine();
            Console.WriteLine(">>> Employees working for " + orgName + ":");

            foreach (var entry in qry)
            {
                Console.WriteLine(">>>     " + entry[0]);
            }
        }
    }
}
