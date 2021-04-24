using Apache.Ignite.Core.Cache.Configuration;
using System;

namespace Models
{
    public class Organization
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="address">Address.</param>
        /// <param name="type">Type.</param>
        /// <param name="lastUpdated">Last update time.</param>
        public Organization(string name, Address address, OrganizationType type, DateTime lastUpdated)
        {
            Name = name;
            Address = address;
            Type = type;
            LastUpdated = lastUpdated;
        }

        /// <summary>
        /// Name.
        /// </summary>
        [QuerySqlField(IsIndexed = true)]
        public string Name { get; set; }

        /// <summary>
        /// Address.
        /// </summary>
        public Address Address { get; set; }

        /// <summary>
        /// Type.
        /// </summary>
        public OrganizationType Type { get; set; }

        /// <summary>
        /// Last update time.
        /// </summary>
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format("{0} [name={1}, address={2}, type={3}, lastUpdated={4}]", typeof(Organization).Name,
                Name, Address, Type, LastUpdated);
        }
    }
}
