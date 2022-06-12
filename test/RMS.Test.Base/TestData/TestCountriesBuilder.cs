using RMS.EntityFrameworkCore;
using RMS.SBJ.CodeTypeTables;
using RMS.SBJ.Company;
using System;

namespace RMS.Test.Base.TestData
{
    public class TestCountriesBuilder
    {
        private readonly RMSDbContext _context;
        private readonly int _tenantId;

        public TestCountriesBuilder(RMSDbContext context ,int tenantId)
        {
            _context = context;
            _tenantId = tenantId;
        }

        public void Create()
        {
            CreateCountries();
        }

        private void CreateCountries()
        {
            CreateCountry(1, "NL", "Netherlands");
            CreateCountry(2, "BE", "Belgium");
        }

        private void CreateCountry(long id, string countryCode, string description)
        {
            var country = new Country
            {
                Id = id,
                TenantId = _tenantId,
                CreationTime = DateTime.Now,
                CountryCode = countryCode,
                Description = description
            };

            _context.Countries.Add(country);
        }
    }
}
