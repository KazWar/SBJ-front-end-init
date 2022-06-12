using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using RMS.SBJ.CodeTypeTables;
using RMS.SBJ.CodeTypeTables.Dtos;
using RMS.SBJ.Company;
using RMS.SBJ.Company.Dtos;
using RMS.Test.Base;
using Shouldly;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace RMS.Tests.SBJ.Companies
{
    public class CompaniesAppService_Tests : AppTestBase
    {
        private readonly ICompaniesAppService _CompaniesAppService;

        public CompaniesAppService_Tests()
        {
            _CompaniesAppService = Resolve<ICompaniesAppService>();
        }

        [MultiTenantFact]
        public async Task Should_Get_All_Companies()
        {
            var output = await _CompaniesAppService.GetAll(new GetAllCompaniesInput { });

            output.ShouldNotBeNull();
            output.ShouldBeOfType<PagedResultDto<GetCompanyForViewDto>>();

            output.Items.Count.ShouldBe(1);
            output.Items[0].AddressPostalCode.ShouldBe("5651 GH");
            output.Items[0].Company.Id.ShouldBe(1);
            output.Items[0].Company.Name.ShouldBe("Test Company");
        }

        [MultiTenantFact]
        public async Task Should_Get_Company_By_Id()
        {
            var output = await _CompaniesAppService.GetCompanyForView(1);

            output.ShouldNotBeNull();
            output.Company.Id.ShouldBe(1);
            output.Company.AddressId.ShouldBe(1);
            output.Company.Name.ShouldBe("Test Company");
            output.Company.EmailAddress.ShouldBe("youremailaddress@domain.com");
            output.Company.PhoneNumber.ShouldBe("+31209999999");
            output.Company.BICCashBack.ShouldBe("BBBBLLPPFFF");
            output.Company.IBANCashBack.ShouldBe("NL00RABO1122334455");
        }

        [MultiTenantFact]
        public async Task Should_Throw_Exception_On_Get_Company_By_Id()
        {
            await _CompaniesAppService.GetCompanyForView(3).ShouldThrowAsync<EntityNotFoundException>();
        }
    }
}
