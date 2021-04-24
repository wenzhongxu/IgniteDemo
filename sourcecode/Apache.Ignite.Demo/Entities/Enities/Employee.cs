using Apache.Ignite.Core.Cache.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    public class Employee
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="salary">Salary.</param>
        /// <param name="address">Address.</param>
        /// <param name="departments">Departments.</param>
        /// <param name="organizationId">The organization identifier.</param>
        public Employee(string name, long salary, Address address, ICollection<string> departments,
            int organizationId = 0)
        {
            Name = name;
            Salary = salary;
            Address = address;
            Departments = departments;
            OrganizationId = organizationId;
        }

        /// <summary>
        /// Name.
        /// </summary>
        [QuerySqlField]
        public string Name { get; set; }

        /// <summary>
        /// Organization id.
        /// </summary>
        [QuerySqlField(IsIndexed = true)]
        public int OrganizationId { get; set; }

        /// <summary>
        /// Salary.
        /// </summary>
        [QuerySqlField]
        public long Salary { get; set; }

        /// <summary>
        /// Address.
        /// </summary>
        [QuerySqlField]
        public Address Address { get; set; }

        /// <summary>
        /// Departments.
        /// </summary>
        public ICollection<string> Departments { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} [name={1}, salary={2}, address={3}, departments={4}]", typeof(Employee).Name,
                Name, Salary, Address, CollectionToString(Departments));
        }

        /// <summary>
        /// Get string representation of collection.
        /// </summary>
        /// <returns></returns>
        private static string CollectionToString<T>(ICollection<T> col)
        {
            if (col == null)
                return "null";

            var elements = col.Any()
                ? col.Select(x => x.ToString()).Aggregate((x, y) => x + ", " + y)
                : string.Empty;

            return string.Format("[{0}]", elements);
        }
    }
}
