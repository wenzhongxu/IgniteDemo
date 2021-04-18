﻿using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Client;
using Apache.Ignite.Core.Client.Cache;
using Models;
using System;
using System.Linq;
using Apache.Ignite.Linq;
using Apache.Ignite.Core.Cache.Query;
using Apache.Ignite.Core.Cache.Affinity;

namespace LinqThin
{
    public class Program
    {
        private const string OrganizationCacheName = "dotnet_linq_organization";

        private const string EmployeeCacheName = "dotnet_linq_employee";

        private const string EmployeeCacheNameColocated = "dotnet_linq_employee_colocated";

        //ignite client配置
        private static readonly IgniteClientConfiguration igniteClientConfiguration = new IgniteClientConfiguration
        {
            Endpoints = new[] { "127.0.0.1" }
        };

        public static void Main(string[] args)
        {
            using var ignite = Ignition.StartClient(igniteClientConfiguration);

            var employeeCache = ignite.GetOrCreateCache<int, Employee>(
                new CacheClientConfiguration(
                    EmployeeCacheName,
                    new QueryEntity(typeof(int), typeof(Employee))));
            PopulateCache(employeeCache);
            QueryExample(employeeCache);
            CompiledQueryExample(employeeCache);
            FieldsQueryExample(employeeCache);

            var organizationCache = ignite.GetOrCreateCache<int, Organization>(
                   new CacheClientConfiguration(
                       OrganizationCacheName,
                       new QueryEntity(typeof(int), typeof(Organization))));
            PopulateCache(organizationCache);
            DistributedJoinQueryExample(employeeCache, organizationCache);

            var employeeCacheColocated = ignite.GetOrCreateCache<AffinityKey, Employee>(
                    new CacheClientConfiguration(
                        EmployeeCacheNameColocated,
                        new QueryEntity(typeof(AffinityKey), typeof(Employee))));
            PopulateCache(employeeCacheColocated);
            JoinQueryExample(employeeCacheColocated, organizationCache);
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

        private static void PopulateCache(ICacheClient<int, Organization> cache)
        {
            cache.Put(1, new Organization(
                "Apache",
                new Address("1065 East Hillsdale Blvd, Foster City, CA", 94404),
                OrganizationType.Private,
                DateTime.Now));

            cache.Put(2, new Organization(
                "Microsoft",
                new Address("1096 Eddy Street, San Francisco, CA", 94109),
                OrganizationType.Private,
                DateTime.Now));
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

        private static void QueryExample(ICacheClient<int, Employee> cache)
        {
            const int zip = 94109;

            IQueryable<ICacheEntry<int, Employee>> qry =
                cache.AsCacheQueryable().Where(emp => emp.Value.Address.Zip == zip);

            Console.WriteLine();
            Console.WriteLine($">>> Employees with zipcode {zip}:");

            foreach (ICacheEntry<int, Employee> entry in qry)
                Console.WriteLine(">>>    " + entry.Value);

            Console.WriteLine($">>> Generated SQL: {qry.ToCacheQueryable().GetFieldsQuery().Sql}");
        }

        private static void CompiledQueryExample(ICacheClient<int, Employee> cache)
        {
            const int zip = 94109;

            var cache0 = cache.AsCacheQueryable();

            // Compile cache query to eliminate LINQ overhead on multiple runs.
            Func<int, IQueryCursor<ICacheEntry<int, Employee>>> qry =
                CompiledQuery.Compile((int z) => cache0.Where(emp => emp.Value.Address.Zip == z));

            Console.WriteLine();
            Console.WriteLine(">>> Employees with zipcode {0} using compiled query:", zip);

            foreach (ICacheEntry<int, Employee> entry in qry(zip))
                Console.WriteLine(">>>    " + entry.Value);
        }

        private static void FieldsQueryExample(ICacheClient<int, Employee> cache)
        {
            var qry = cache.AsCacheQueryable().Select(entry => new { entry.Value.Name, entry.Value.Salary });

            Console.WriteLine();
            Console.WriteLine(">>> Employee names and their salaries:");

            foreach (var row in qry)
                Console.WriteLine($">>>     [Name={row.Name}, salary={row.Salary}{']'}");

            Console.WriteLine($">>> Generated SQL: {qry.ToCacheQueryable().GetFieldsQuery().Sql}");
        }

        private static void DistributedJoinQueryExample(
            ICacheClient<int, Employee> employeeCache,
            ICacheClient<int, Organization> organizationCache)
        {
            const string orgName = "Apache";

            var queryOptions = new QueryOptions
            {
                EnableDistributedJoins = true,
                Timeout = new TimeSpan(0, 1, 0)
            };

            IQueryable<ICacheEntry<int, Employee>> employees = employeeCache.AsCacheQueryable(queryOptions);
            IQueryable<ICacheEntry<int, Organization>> organizations = organizationCache.AsCacheQueryable(queryOptions);

            IQueryable<ICacheEntry<int, Employee>> qry =
                from employee in employees
                from organization in organizations
                where employee.Value.OrganizationId == organization.Key && organization.Value.Name == orgName
                select employee;


            Console.WriteLine();
            Console.WriteLine($">>> Employees working for {orgName} using distributed joins:");

            foreach (ICacheEntry<int, Employee> entry in qry)
                Console.WriteLine(">>>     " + entry.Value);

            Console.WriteLine($">>> Generated SQL: {qry.ToCacheQueryable().GetFieldsQuery().Sql}");
        }

        private static void JoinQueryExample(
            ICacheClient<AffinityKey, Employee> employeeCache,
            ICacheClient<int, Organization> organizationCache)
        {
            const string orgName = "Apache";

            IQueryable<ICacheEntry<AffinityKey, Employee>> employees = employeeCache.AsCacheQueryable();
            IQueryable<ICacheEntry<int, Organization>> organizations = organizationCache.AsCacheQueryable();

            IQueryable<ICacheEntry<AffinityKey, Employee>> qry =
                from employee in employees
                from organization in organizations
                where employee.Value.OrganizationId == organization.Key && organization.Value.Name == orgName
                select employee;


            Console.WriteLine();
            Console.WriteLine($">>> Employees working for {orgName}:");

            foreach (ICacheEntry<AffinityKey, Employee> entry in qry)
                Console.WriteLine(">>>     " + entry.Value);

            Console.WriteLine($">>> Generated SQL: {qry.ToCacheQueryable().GetFieldsQuery().Sql}");
        }

    }
}