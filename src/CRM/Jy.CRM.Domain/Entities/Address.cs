using Jy.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jy.CRM.Domain.Entities
{
    public class Address
         : ValueObject
    {
        public  ICollection<SecKillOrder> SecKillOrders { get; set; }

        public  ICollection<UserAddress> AddressUsers { get; set; }
        public String Street { get;  set; }

        public String City { get;  set; }

        public String Province { get;  set; }

        public String Country { get;  set; }

        public String ZipCode { get;  set; }

        public Address(string street, string city, string province, string country, string zipcode)
        {
            Street = street;
            City = city;
            Province = province;
            Country = country;
            ZipCode = zipcode;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Street;
            yield return City;
            yield return Province;
            yield return Country;
            yield return ZipCode;
        }
    }
}
