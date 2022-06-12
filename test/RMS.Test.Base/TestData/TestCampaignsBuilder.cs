using RMS.EntityFrameworkCore;
using RMS.SBJ.CampaignProcesses;
using RMS.SBJ.CodeTypeTables;
using System;

namespace RMS.Test.Base.TestData
{
    public class TestCampaignsBuilder
    {
        private readonly RMSDbContext _context;
        private readonly int _tenantId;

        public TestCampaignsBuilder(RMSDbContext context, int tenantId)
        {
            _context = context;
            _tenantId = tenantId;
        }

        public void Create()
        {
            CreateCampaigns();
            CreateCampaignTypes();
            CreateCampaignCampaignTypes();
        }

        private void CreateCampaigns()
        {
            //Cash Refund campaigns
            CreateCampaign(1, "TesterEndedCR", "", DateTime.Today.AddMonths(-2), DateTime.Today.AddMonths(-1), (_tenantId * 10000 + 100), "EndedCampaignCR");
            CreateCampaign(2, "TesterEndingCR", "", DateTime.Today.AddMonths(-1), DateTime.Today, (_tenantId * 10000 + 200), "EndingCampaignCR");
            CreateCampaign(3, "TesterMidRunCR", "", DateTime.Today.AddMonths(-1), DateTime.Today.AddMonths(1), (_tenantId * 10000 + 300), "MidRunCampaignCR");
            CreateCampaign(4, "TesterStartingCR", "", DateTime.Today, DateTime.Today.AddMonths(1), (_tenantId * 10000 + 400), "StartingCampaignCR");
            CreateCampaign(5, "TesterComingCR", "", DateTime.Today.AddMonths(1), DateTime.Today.AddMonths(2), (_tenantId * 10000 + 500), "ComingCampaignCR");

            //Premium campaigns
            CreateCampaign(6, "TesterEndedPM", "", DateTime.Today.AddMonths(-2), DateTime.Today.AddMonths(-1), (_tenantId * 10000 + 600), "EndedCampaignPM");
            CreateCampaign(7, "TesterEndingPM", "", DateTime.Today.AddMonths(-1), DateTime.Today, (_tenantId * 10000 + 700), "EndingCampaignPM");
            CreateCampaign(8, "TesterMidRunPM", "", DateTime.Today.AddMonths(-1), DateTime.Today.AddMonths(1), (_tenantId * 10000 + 800), "MidRunCampaignPM");
            CreateCampaign(9, "TesterStartingPM", "", DateTime.Today, DateTime.Today.AddMonths(1), (_tenantId * 10000 + 900), "StartingCampaignPM");
            CreateCampaign(10, "TesterComingPM", "", DateTime.Today.AddMonths(1), DateTime.Today.AddMonths(2), (_tenantId * 10000 + 1000), "ComingCampaignPM");

            //Activation Code campaigns
            CreateCampaign(11, "TesterEndedAC", "", DateTime.Today.AddMonths(-2), DateTime.Today.AddMonths(-1), (_tenantId * 10000 + 1100), "EndedCampaignAC");
            CreateCampaign(12, "TesterEndingAC", "", DateTime.Today.AddMonths(-1), DateTime.Today, (_tenantId * 10000 + 1200), "EndingCampaignAC");
            CreateCampaign(13, "TesterMidRunAC", "", DateTime.Today.AddMonths(-1), DateTime.Today.AddMonths(1), (_tenantId * 10000 + 1300), "MidRunCampaignAC");
            CreateCampaign(14, "TesterStartingAC", "", DateTime.Today, DateTime.Today.AddMonths(1), (_tenantId * 10000 + 1400), "StartingCampaignAC");
            CreateCampaign(15, "TesterComingAC", "", DateTime.Today.AddMonths(1), DateTime.Today.AddMonths(2), (_tenantId * 10000 + 1500), "ComingCampaignAC");
        }

        private void CreateCampaign(long id, string name, string description, DateTime startDate, DateTime endDate, int campaignCode, string externalCode)
        {
            var campaign = new Campaign
            {
                Id = id,
                TenantId = _tenantId,
                CreationTime = DateTime.Now,
                CreatorUserId = 1,
                IsDeleted = false,
                Name = name,
                Description = description,
                StartDate = startDate,
                EndDate = endDate,
                CampaignCode = campaignCode,
                ExternalCode = externalCode
            };

            _context.Campaigns.Add(campaign);
        }

        private void CreateCampaignTypes()
        {
            CreateCampaignType(1, "CR", "Cash Refund");
            CreateCampaignType(2, "PM", "Premium");
            CreateCampaignType(3, "AC", "Activation Code");
        }

        private void CreateCampaignType(long id, string code, string name)
        {
            var campaignType = new CampaignType
            {
                Id = id,
                TenantId = _tenantId,
                CreationTime = DateTime.Now,
                CreatorUserId = 1,
                IsDeleted = false,
                IsActive = true,
                Code = code,
                Name = name
            };

            _context.CampaignTypes.Add(campaignType);
        }

        private void CreateCampaignCampaignTypes()
        {
            CreateCampaignCampaignType(1, "Cash Refund", 1, 1);
            CreateCampaignCampaignType(2, "Cash Refund", 2, 1);
            CreateCampaignCampaignType(3, "Cash Refund", 3, 1);
            CreateCampaignCampaignType(4, "Cash Refund", 4, 1);
            CreateCampaignCampaignType(5, "Cash Refund", 5, 1);

            CreateCampaignCampaignType(6, "Premium", 6, 2);
            CreateCampaignCampaignType(7, "Premium", 7, 2);
            CreateCampaignCampaignType(8, "Premium", 8, 2);
            CreateCampaignCampaignType(9, "Premium", 9, 2);
            CreateCampaignCampaignType(10, "Premium", 10, 2);

            CreateCampaignCampaignType(11, "Activation Code", 11, 3);
            CreateCampaignCampaignType(12, "Activation Code", 12, 3);
            CreateCampaignCampaignType(13, "Activation Code", 13, 3);
            CreateCampaignCampaignType(14, "Activation Code", 14, 3);
            CreateCampaignCampaignType(15, "Activation Code", 15, 3);
        }

        private async void CreateCampaignCampaignType(long id, string description, long campaignId, long campaignTypeId)
        {
            var campaignCampaignType = new CampaignCampaignType
            {
                Id = id,
                TenantId = _tenantId,
                CreationTime = DateTime.Now,
                CreatorUserId = 1,
                IsDeleted = false,
                Description = description,
                CampaignId = campaignId,
                CampaignTypeId = campaignTypeId,
                CampaignFk = await _context.Campaigns.FindAsync(campaignId),
                CampaignTypeFk = await _context.CampaignTypes.FindAsync(campaignTypeId)
            };

            _context.CampaignCampaignTypes.Add(campaignCampaignType);
        }
    }
}
