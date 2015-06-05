using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.DataAnnotations;

namespace WindowsFormsApplication1
{

    public enum PhoneType
    {
        Home,
        Work,
        Mobile,
    }

    public enum AddressType
    {
        Home,
        Work,
        Other,
    }


    public class Address
    {
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string ZipCode { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }

    public class Customer
    {
        public Customer()
        {
            this.PhoneNumbers = new Dictionary<PhoneType, string>();
            this.Addresses = new Dictionary<AddressType, Address>();
        }

         [PrimaryKey]  
        public Guid Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Index(Unique = true)] // Creates Unique Index
        public string Email { get; set; }

        public Dictionary<PhoneType, string> PhoneNumbers { get; set; }  //Blobbed
        public Dictionary<AddressType, Address> Addresses { get; set; }  //Blobbed
        public DateTime CreatedAt { get; set; }
    }
}
