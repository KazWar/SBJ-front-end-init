using RMS.SBJ.UniqueCodes;
using RMS.EntityFrameworkCore;

namespace RMS.Test.Base.TestData
{
    public class TestUniqueCodesBuilder
    {
        private readonly RMSDbContext _context;

        public TestUniqueCodesBuilder(RMSDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateUniqueCodes();
            CreateUniqueCodeByCampaigns();
        }

        private void CreateUniqueCodes()
        {
            CreateUniqueCode("A123456", false);
            CreateUniqueCode("A098765", false);
            CreateUniqueCode("B123456", true);
        }

        private void CreateUniqueCode(string code, bool used)
        {
            var uniqueCode = new UniqueCode
            {
                Id = code,
                Used = used
            };

            _context.UniqueCodes.Add(uniqueCode);
        }

        private void CreateUniqueCodeByCampaigns()
        {
            CreateUniqueCodeByCampaign("Z112233", false, 1);
            CreateUniqueCodeByCampaign("Z778899", true, 1);
            CreateUniqueCodeByCampaign("Y112233", false, 3);
            CreateUniqueCodeByCampaign("Y445566", true, 3);
            CreateUniqueCodeByCampaign("A123456", true, 15);
        }

        private async void CreateUniqueCodeByCampaign(string code, bool used, long campaignId)
        {
            var uniqueCodeByCampaign = new UniqueCodeByCampaign
            {
                Id = code,
                Used = used,
                CampaignId = campaignId,
                CampaignFk = await _context.Campaigns.FindAsync(campaignId)
            };

            _context.UniqueCodeByCampaigns.Add(uniqueCodeByCampaign);
        }
    }
}
