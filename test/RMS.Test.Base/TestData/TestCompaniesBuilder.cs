using RMS.EntityFrameworkCore;
using RMS.SBJ.Company;
using System;

namespace RMS.Test.Base.TestData
{
    public class TestCompaniesBuilder
    {
        private readonly RMSDbContext _context;
        private readonly int _tenantId;

        public TestCompaniesBuilder(RMSDbContext context ,int tenantId)
        {
            _context = context;
            _tenantId = tenantId;
        }

        public void Create()
        {
            CreateCompanies();
        }

        private void CreateCompanies()
        {
            CreateCompany(1L, 1L, "Test Company", "youremailaddress@domain.com", "+31209999999", "BBBBLLPPFFF", "NL00RABO1122334455");
        }

        private void CreateCompany(long id, long addressId, string name, string emailAddress, string phoneNumber, string bicCashBack, string ibanCashBack)
        {
            var company = new Company
            {
                Id = id,
                TenantId = _tenantId,
                AddressId = addressId,
                CreationTime = DateTime.Now,
                IsDeleted = false,
                Name = name,
                EmailAddress = emailAddress,
                PhoneNumber = phoneNumber,
                BicCashBack = bicCashBack,
                IbanCashBack = ibanCashBack
            };

            _context.Companies.Add(company);
        }
    }
}
