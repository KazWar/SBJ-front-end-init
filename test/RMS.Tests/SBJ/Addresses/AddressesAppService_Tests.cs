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

namespace RMS.Tests.SBJ.Addresses
{
    public class AddressesAppService_Tests : AppTestBase
    {
        private readonly IAddressesAppService _addressesAppService;

        public AddressesAppService_Tests()
        {
            _addressesAppService = Resolve<IAddressesAppService>();
        }

        [MultiTenantFact]
        public async Task Should_Get_All_Addresses()
        {
            var output = await _addressesAppService.GetAll(new GetAllAddressesInput { });

            output.ShouldNotBeNull();
            output.ShouldBeOfType<PagedResultDto<GetAddressForViewDto>>();

            output.Items.Count.ShouldBe(1);
            output.Items[0].CountryCountryCode.ShouldBe("NL");
            output.Items[0].Address.Id.ShouldBe(1);
            output.Items[0].Address.City.ShouldBe("Eindhoven");
        }

        [MultiTenantFact]
        public async Task Should_Get_Address_By_Id()
        {
            var output = await _addressesAppService.GetAddressForView(1);

            output.ShouldNotBeNull();
            output.Address.Id.ShouldBe(1);
            output.Address.CountryId.ShouldBe(1);
            output.Address.AddressLine1.ShouldBe("De Schakel 17");
            output.Address.AddressLine2.ShouldBeEmpty();
            output.Address.PostalCode.ShouldBe("5651 GH");
            output.Address.City.ShouldBe("Eindhoven");
        }

        [MultiTenantFact]
        public async Task Should_Throw_Exception_On_Get_Address_By_Id()
        {
            await _addressesAppService.GetAddressForView(3).ShouldThrowAsync<EntityNotFoundException>();
        }
    }
}
