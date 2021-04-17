using Apache.Ignite.Core.Binary;
using Apache.Ignite.Core.Cache.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Address : IBinarizable
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="street">Street.</param>
        /// <param name="zip">ZIP code.</param>
        public Address(string street, int zip)
        {
            Street = street;
            Zip = zip;
        }

        /// <summary>
        /// Street.
        /// </summary>
        [QueryTextField]
        public string Street { get; set; }

        /// <summary>
        /// ZIP code.
        /// </summary>
        [QuerySqlField(IsIndexed = true)]
        public int Zip { get; set; }

        /// <summary>
        /// Writes this object to the given writer.
        /// </summary>
        /// <param name="writer">Writer.</param>
        public void WriteBinary(IBinaryWriter writer)
        {
            writer.WriteString("street", Street);
            writer.WriteInt("zip", Zip);
        }

        /// <summary>
        /// Reads this object from the given reader.
        /// </summary>
        /// <param name="reader">Reader.</param>
        public void ReadBinary(IBinaryReader reader)
        {
            Street = reader.ReadString("street");
            Zip = reader.ReadInt("zip");
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} [street={1}, zip={2}]", typeof(Address).Name, Street, Zip);
        }
    }
}
