using RMS.EntityFrameworkCore;

namespace RMS.Test.Base.TestData
{
    public class TestDataBuilder
    {
        private readonly RMSDbContext _context;
        private readonly int _tenantId;

        public TestDataBuilder(RMSDbContext context, int tenantId)
        {
            _context = context;
            _tenantId = tenantId;
        }

        public void Create()
        {
            new TestOrganizationUnitsBuilder(_context, _tenantId).Create();
            new TestSubscriptionPaymentBuilder(_context, _tenantId).Create();
            new TestEditionsBuilder(_context).Create();

            new TestCountriesBuilder(_context, _tenantId).Create();
            new TestAddressesBuilder(_context, _tenantId).Create();
            new TestCompaniesBuilder(_context, _tenantId).Create();

            new TestCampaignsBuilder(_context, _tenantId).Create();
            new TestUniqueCodesBuilder(_context).Create();

            _context.SaveChanges();
        }
    }
}
