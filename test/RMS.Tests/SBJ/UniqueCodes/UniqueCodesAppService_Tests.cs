using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using RMS.SBJ.UniqueCodes;
using RMS.SBJ.UniqueCodes.Dtos;
using RMS.Test.Base;
using Shouldly;
using Xunit;

namespace RMS.Tests.SBJ.UniqueCodes
{
    public class UniqueCodesAppService_Tests : AppTestBase
    {
        // Seeded UniqueCodes are: [A123456, false], [A098765, false], [B123456, true]

        private readonly IUniqueCodesAppService _uniqueCodesAppService;
        private readonly IUniqueCodeByCampaignsAppService _uniqueCodeByCampaignsAppService;

        public UniqueCodesAppService_Tests()
        {
            _uniqueCodesAppService = Resolve<IUniqueCodesAppService>();
            _uniqueCodeByCampaignsAppService = Resolve<IUniqueCodeByCampaignsAppService>();
        }

        #region GetAll

        #endregion

        #region GetUniqueCodeForEdit
        [MultiTenantFact]
        public async Task Should_Get_Unused_UniqueCode()
        {
            var output = await _uniqueCodesAppService.GetUniqueCodeForEdit(new EntityDto<string>{ Id = "A123456" });

            output.ShouldNotBeNull();

            output.UniqueCode.Id.ShouldBe("A123456");
            output.UniqueCode.Used.ShouldBeFalse();
        }

        [MultiTenantFact]
        public async Task Should_Get_Used_UniqueCode()
        {
            var output = await _uniqueCodesAppService.GetUniqueCodeForEdit(new EntityDto<string> { Id = "B123456" });

            output.ShouldNotBeNull();

            output.UniqueCode.Id.ShouldBe("B123456");
            output.UniqueCode.Used.ShouldBeTrue();
        }

        [MultiTenantFact]
        public async Task Should_Not_Get_Invalid_UniqueCode()
        {
            var output = await _uniqueCodesAppService.GetUniqueCodeForEdit(new EntityDto<string> { Id = "C111111" });

            output.ShouldNotBeNull();

            output.UniqueCode.ShouldBeNull();
        }
        #endregion

        #region Create
        //Won't work this way since it'll create its own id (GUID), and context doesn't have a find first or last for UniqueCode

        //[MultiTenantFact]
        //public async Task Should_Create_UniqueCode()
        //{
        //    await _uniqueCodesAppService.CreateOrEdit(
        //        new CreateOrEditUniqueCodeDto
        //        {
        //            Used = false
        //        });

        //    await UsingDbContext(async context =>
        //    {
        //        var createdUniqueCode = await context.UniqueCodes.FindAsync();
        //        createdUniqueCode.ShouldNotBeNull();

        //        createdUniqueCode.Used.ShouldBeFalse();
        //    });
        //}
        #endregion

        #region Update
        [MultiTenantFact]
        public async Task Should_Update_UniqueCode()
        {
            string uniqueCode = "B123456";

            await _uniqueCodesAppService.CreateOrEdit(
                new CreateOrEditUniqueCodeDto
                {
                    Id = uniqueCode,
                    Used = false
                });

            await UsingDbContext(async context =>
            {
                var createdUniqueCode = await context.UniqueCodes.FindAsync(uniqueCode);
                createdUniqueCode.ShouldNotBeNull();

                createdUniqueCode.Used.ShouldBeFalse();
            });
        }

        [MultiTenantFact]
        public async Task Should_Not_Update_Invalid_UniqueCode()
        {
            string uniqueCode = "C111111";

            await _uniqueCodesAppService.CreateOrEdit(
                new CreateOrEditUniqueCodeDto
                {
                    Id = uniqueCode,
                    Used = false
                });

            await UsingDbContext(async context =>
            {
                var createdUniqueCode = await context.UniqueCodes.FindAsync(uniqueCode);
                createdUniqueCode.ShouldBeNull();
            });
        }
        #endregion

        #region Delete
        [MultiTenantFact]
        public async Task Should_Delete_UniqueCode()
        {
            string uniqueCode = "A098765";

            await UsingDbContext(async context =>
            {
                var existingUniqueCode = await context.UniqueCodes.FindAsync(uniqueCode);
                existingUniqueCode.ShouldNotBeNull();
            });

            await _uniqueCodesAppService.Delete(
                new CreateOrEditUniqueCodeDto
                {
                    Id = uniqueCode,
                    Used = false
                });

            await UsingDbContext(async context =>
            {
                var deletedUniqueCode = await context.UniqueCodes.FindAsync(uniqueCode);
                deletedUniqueCode.ShouldBeNull();
            });
        }

        [MultiTenantFact]
        public async Task Should_Not_Delete_Invalid_UniqueCode()
        {
            string uniqueCode = "C111111";

            await _uniqueCodesAppService.CreateOrEdit(
                new CreateOrEditUniqueCodeDto
                {
                    Id = uniqueCode,
                    Used = false
                });

            await UsingDbContext(async context =>
            {
                var deletedUniqueCode = await context.UniqueCodes.FindAsync(uniqueCode);
                deletedUniqueCode.ShouldBeNull();
            });
        }
        #endregion

        #region IsCodeValid
        [MultiTenantFact]
        public async Task Should_Be_Valid_UniqueCode()
        {
            var output = await _uniqueCodesAppService.IsCodeValid("A123456");
            output.ShouldBeTrue();
        }

        [MultiTenantFact]
        public async Task Should_Not_Be_Valid_UniqueCodes()
        {
            //Code doesn't exist in DB
            var outputNotInDb = await _uniqueCodesAppService.IsCodeValid("C111111");
            outputNotInDb.ShouldBeFalse();

            //Code exists but is used
            var outputUsed = await _uniqueCodesAppService.IsCodeValid("B123456");
            outputUsed.ShouldBeFalse();
        }
        #endregion

        #region IsCodeValidByCampaign
        [MultiTenantFact]
        public async Task Should_Be_Valid_UniqueCodeByCampaigns()
        {
            var outputCampaignCode10100 = await _uniqueCodesAppService.IsCodeValidByCampaign("Z112233", "10100");
            outputCampaignCode10100.ShouldBeTrue();

            var outputCampaignCode10300 = await _uniqueCodesAppService.IsCodeValidByCampaign("Y112233", "10300");
            outputCampaignCode10300.ShouldBeTrue();
        }

        [MultiTenantFact]
        public async Task Should_Not_Be_Valid_UniqueCodeByCampaigns()
        {
            //Code doesn't exist in Database for CampaignCode 10100
            var outputNotInDatabaseCampaignCode10100 = await _uniqueCodesAppService.IsCodeValidByCampaign("Y112233", "10100");
            outputNotInDatabaseCampaignCode10100.ShouldBeFalse();

            //Code is set as used in CampaignCode 11500, and set as not used in the general UniqueCodes table
            var outputNotInDatabaseCampaignCode11500 = await _uniqueCodesAppService.IsCodeValidByCampaign("A123456", "11500");
            outputNotInDatabaseCampaignCode11500.ShouldBeFalse();

            //Code exists for CampaignCode 10300 only but is used
            var outputUsedCampaignCode10300 = await _uniqueCodesAppService.IsCodeValidByCampaign("Y445566", "10300");
            outputUsedCampaignCode10300.ShouldBeFalse();
        }

        [MultiTenantFact]
        public async Task Should_Not_Be_Valid_NonExisting_UniqueCodeByCampaign_For_Any_CampaignCode()
        {
            //Code doesn't exist in Database for any CampaignCode
            var uniqueCodeByCampaignsPaged = await _uniqueCodeByCampaignsAppService.GetAll(new GetAllUniqueCodeByCampaignsInput { Filter = "0ABCDEFG" });
            uniqueCodeByCampaignsPaged.TotalCount.ShouldBe(0);

            var outputNotInDbForAnyCampaignCode10100 = await _uniqueCodesAppService.IsCodeValidByCampaign("0ABCDEFG", "10100");
            outputNotInDbForAnyCampaignCode10100.ShouldBeFalse();

            var outputNotInDbForAnyCampaignCode10200 = await _uniqueCodesAppService.IsCodeValidByCampaign("0ABCDEFG", "10200");
            outputNotInDbForAnyCampaignCode10200.ShouldBeFalse();

            var outputNotInDbForAnyCampaignCode10300 = await _uniqueCodesAppService.IsCodeValidByCampaign("0ABCDEFG", "10300");
            outputNotInDbForAnyCampaignCode10300.ShouldBeFalse();

            var outputNotInDbForAnyCampaignCode10400 = await _uniqueCodesAppService.IsCodeValidByCampaign("0ABCDEFG", "10400");
            outputNotInDbForAnyCampaignCode10400.ShouldBeFalse();

            var outputNotInDbForAnyCampaignCode10500 = await _uniqueCodesAppService.IsCodeValidByCampaign("0ABCDEFG", "10500");
            outputNotInDbForAnyCampaignCode10500.ShouldBeFalse();

            var outputNotInDbForAnyCampaignCode10600 = await _uniqueCodesAppService.IsCodeValidByCampaign("0ABCDEFG", "10600");
            outputNotInDbForAnyCampaignCode10600.ShouldBeFalse();

            var outputNotInDbForAnyCampaignCode10700 = await _uniqueCodesAppService.IsCodeValidByCampaign("0ABCDEFG", "10700");
            outputNotInDbForAnyCampaignCode10700.ShouldBeFalse();

            var outputNotInDbForAnyCampaignCode10800 = await _uniqueCodesAppService.IsCodeValidByCampaign("0ABCDEFG", "10800");
            outputNotInDbForAnyCampaignCode10800.ShouldBeFalse();

            var outputNotInDbForAnyCampaignCode10900 = await _uniqueCodesAppService.IsCodeValidByCampaign("0ABCDEFG", "10900");
            outputNotInDbForAnyCampaignCode10900.ShouldBeFalse();

            var outputNotInDbForAnyCampaignCode11000 = await _uniqueCodesAppService.IsCodeValidByCampaign("0ABCDEFG", "11000");
            outputNotInDbForAnyCampaignCode11000.ShouldBeFalse();

            var outputNotInDbForAnyCampaignCode11100 = await _uniqueCodesAppService.IsCodeValidByCampaign("0ABCDEFG", "11100");
            outputNotInDbForAnyCampaignCode11100.ShouldBeFalse();

            var outputNotInDbForAnyCampaignCode11200 = await _uniqueCodesAppService.IsCodeValidByCampaign("0ABCDEFG", "11200");
            outputNotInDbForAnyCampaignCode11200.ShouldBeFalse();

            var outputNotInDbForAnyCampaignCode11300 = await _uniqueCodesAppService.IsCodeValidByCampaign("0ABCDEFG", "11300");
            outputNotInDbForAnyCampaignCode11300.ShouldBeFalse();

            var outputNotInDbForAnyCampaignCode11400 = await _uniqueCodesAppService.IsCodeValidByCampaign("0ABCDEFG", "11400");
            outputNotInDbForAnyCampaignCode11400.ShouldBeFalse();

            var outputNotInDbForAnyCampaignCode11500 = await _uniqueCodesAppService.IsCodeValidByCampaign("0ABCDEFG", "11500");
            outputNotInDbForAnyCampaignCode11500.ShouldBeFalse();
        }
        #endregion

        #region SetCodeUsed
        [MultiTenantFact]
        public async Task Should_Change_Unused_UniqueCode_To_Used()
        {
            var outputValidBeforeUsed = await _uniqueCodesAppService.IsCodeValid("A098765");
            outputValidBeforeUsed.ShouldBeTrue();

            var outputFromSetUsed = await _uniqueCodesAppService.SetCodeUsed("A098765");
            outputFromSetUsed.ShouldBeTrue();

            var outputValidAfterUsed = await _uniqueCodesAppService.IsCodeValid("A098765");
            outputValidAfterUsed.ShouldBeFalse();
        }

        [MultiTenantFact]
        public async Task Should_Keep_Used_UniqueCode_As_Used()
        {
            var outputValidBeforeUsed = await _uniqueCodesAppService.IsCodeValid("B123456");
            outputValidBeforeUsed.ShouldBeFalse();

            var outputFromSetUsed = await _uniqueCodesAppService.SetCodeUsed("B123456");
            outputFromSetUsed.ShouldBeTrue();

            var outputValidAfterUsed = await _uniqueCodesAppService.IsCodeValid("B123456");
            outputValidAfterUsed.ShouldBeFalse();
        }

        [MultiTenantFact]
        public async Task Should_Not_Error_Nor_Create_NonExisting_UniqueCode_When_Set_As_Used()
        {
            var outputNotExistBeforeUsed = await _uniqueCodesAppService.GetUniqueCodeForEdit(new EntityDto<string> { Id = "C111111" });
            outputNotExistBeforeUsed.UniqueCode.ShouldBeNull();

            var outputFromSetUsed = await _uniqueCodesAppService.SetCodeUsed("C111111");
            outputFromSetUsed.ShouldBeFalse();

            var outputNotExistAfterUsed = await _uniqueCodesAppService.GetUniqueCodeForEdit(new EntityDto<string> { Id = "C111111" });
            outputNotExistAfterUsed.UniqueCode.ShouldBeNull();
        }
        #endregion

        #region SetCodeUsedByCampaign
        [MultiTenantFact]
        public async Task Should_Change_Unused_UniqueCodeByCampaign_To_Used()
        {
            var outputValidBeforeUsedCampaignCode10300 = await _uniqueCodesAppService.IsCodeValidByCampaign("Y112233", "10300");
            outputValidBeforeUsedCampaignCode10300.ShouldBeTrue();

            var outputFromSetUsed = await _uniqueCodesAppService.SetCodeUsedByCampaign("Y112233", "10300");
            outputFromSetUsed.ShouldBeTrue();

            var outputValidAfterUsedCampaignCode10300 = await _uniqueCodesAppService.IsCodeValidByCampaign("Y112233", "10300");
            outputValidAfterUsedCampaignCode10300.ShouldBeFalse();
        }

        [MultiTenantFact]
        public async Task Should_Keep_Used_UniqueCodeByCampaign_As_Used()
        {
            var outputValidBeforeUsedCampaignCode10100 = await _uniqueCodesAppService.IsCodeValidByCampaign("Z778899", "10100");
            outputValidBeforeUsedCampaignCode10100.ShouldBeFalse();

            var outputFromSetUsed = await _uniqueCodesAppService.SetCodeUsedByCampaign("Z778899", "10100");
            outputFromSetUsed.ShouldBeTrue();

            var outputValidAfterUsedCampaignCode10100 = await _uniqueCodesAppService.IsCodeValidByCampaign("Z778899", "10100");
            outputValidAfterUsedCampaignCode10100.ShouldBeFalse();
        }

        [MultiTenantFact]
        public async Task Should_Not_Error_Nor_Create_NonExisting_UniqueCodeByCampaign_When_Set_As_Used()
        {
            var uniqueCodeByCampaignsPagedBeforeSetUseds = await _uniqueCodeByCampaignsAppService.GetAll(new GetAllUniqueCodeByCampaignsInput { Filter = "0ABCDEFG" });
            uniqueCodeByCampaignsPagedBeforeSetUseds.TotalCount.ShouldBe(0);

            var outputNotInDbForAnyCampaignCode10100 = await _uniqueCodesAppService.SetCodeUsedByCampaign("0ABCDEFG", "10100");
            outputNotInDbForAnyCampaignCode10100.ShouldBeFalse();

            var outputNotInDbForAnyCampaignCode10200 = await _uniqueCodesAppService.SetCodeUsedByCampaign("0ABCDEFG", "10200");
            outputNotInDbForAnyCampaignCode10200.ShouldBeFalse();

            var outputNotInDbForAnyCampaignCode10300 = await _uniqueCodesAppService.SetCodeUsedByCampaign("0ABCDEFG", "10300");
            outputNotInDbForAnyCampaignCode10300.ShouldBeFalse();

            var outputNotInDbForAnyCampaignCode10400 = await _uniqueCodesAppService.SetCodeUsedByCampaign("0ABCDEFG", "10400");
            outputNotInDbForAnyCampaignCode10400.ShouldBeFalse();

            var outputNotInDbForAnyCampaignCode10500 = await _uniqueCodesAppService.SetCodeUsedByCampaign("0ABCDEFG", "10500");
            outputNotInDbForAnyCampaignCode10500.ShouldBeFalse();

            var outputNotInDbForAnyCampaignCode10600 = await _uniqueCodesAppService.SetCodeUsedByCampaign("0ABCDEFG", "10600");
            outputNotInDbForAnyCampaignCode10600.ShouldBeFalse();

            var outputNotInDbForAnyCampaignCode10700 = await _uniqueCodesAppService.SetCodeUsedByCampaign("0ABCDEFG", "10700");
            outputNotInDbForAnyCampaignCode10700.ShouldBeFalse();

            var outputNotInDbForAnyCampaignCode10800 = await _uniqueCodesAppService.SetCodeUsedByCampaign("0ABCDEFG", "10800");
            outputNotInDbForAnyCampaignCode10800.ShouldBeFalse();

            var outputNotInDbForAnyCampaignCode10900 = await _uniqueCodesAppService.SetCodeUsedByCampaign("0ABCDEFG", "10900");
            outputNotInDbForAnyCampaignCode10900.ShouldBeFalse();

            var outputNotInDbForAnyCampaignCode11000 = await _uniqueCodesAppService.SetCodeUsedByCampaign("0ABCDEFG", "11000");
            outputNotInDbForAnyCampaignCode11000.ShouldBeFalse();

            var outputNotInDbForAnyCampaignCode11100 = await _uniqueCodesAppService.SetCodeUsedByCampaign("0ABCDEFG", "11100");
            outputNotInDbForAnyCampaignCode11100.ShouldBeFalse();

            var outputNotInDbForAnyCampaignCode11200 = await _uniqueCodesAppService.SetCodeUsedByCampaign("0ABCDEFG", "11200");
            outputNotInDbForAnyCampaignCode11200.ShouldBeFalse();

            var outputNotInDbForAnyCampaignCode11300 = await _uniqueCodesAppService.SetCodeUsedByCampaign("0ABCDEFG", "11300");
            outputNotInDbForAnyCampaignCode11300.ShouldBeFalse();

            var outputNotInDbForAnyCampaignCode11400 = await _uniqueCodesAppService.SetCodeUsedByCampaign("0ABCDEFG", "11400");
            outputNotInDbForAnyCampaignCode11400.ShouldBeFalse();

            var outputNotInDbForAnyCampaignCode11500 = await _uniqueCodesAppService.SetCodeUsedByCampaign("0ABCDEFG", "11500");
            outputNotInDbForAnyCampaignCode11500.ShouldBeFalse();

            var uniqueCodeByCampaignsPagedAfterSetUseds = await _uniqueCodeByCampaignsAppService.GetAll(new GetAllUniqueCodeByCampaignsInput { Filter = "0ABCDEFG" });
            uniqueCodeByCampaignsPagedAfterSetUseds.TotalCount.ShouldBe(0);
        }
        #endregion
    }
}
