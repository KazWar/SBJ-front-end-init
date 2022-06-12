using RMS.EntityFrameworkCore;
using RMS.SBJ.Company;
using System;

namespace RMS.Test.Base.TestData
{
    public class TestAddressesBuilder
    {
        private readonly RMSDbContext _context;
        private readonly int _tenantId;

        public TestAddressesBuilder(RMSDbContext context ,int tenantId)
        {
            _context = context;
            _tenantId = tenantId;
        }

        public void Create()
        {
            CreateAddresses();
        }

        private void CreateAddresses()
        {
            CreateAddress(1, 1, "De Schakel 17", "", "5651 GH", "Eindhoven");
        }

        private void CreateAddress(long id, long countryId, string addressLine1, string addressLine2, string postalCode, string city)
        {
            var address = new Address
            {
                Id = id,
                TenantId = _tenantId,
                CountryId = countryId,
                CreationTime = DateTime.Now,
                IsDeleted = false,
                AddressLine1 = addressLine1,
                AddressLine2 = addressLine2,
                PostalCode = postalCode,
                City = city
            };

            _context.Addresses.Add(address);
        }
    }
}
