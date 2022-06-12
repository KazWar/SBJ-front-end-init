using Abp.Domain.Entities;
using RMS.SBJ.CodeTypeTables;
using RMS.SBJ.CodeTypeTables.Dtos;
using RMS.Test.Base;
using Shouldly;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace RMS.Tests.SBJ.Countries
{
    public class CountriesAppService_Tests : AppTestBase
    {
        private readonly ICountriesAppService _countriesAppService;

        public CountriesAppService_Tests()
        {
            _countriesAppService = Resolve<ICountriesAppService>();
        }

        [MultiTenantFact]
        public async Task Should_Get_All_Countries()
        {
            var output = await _countriesAppService.GetAll();

            output.ShouldNotBeNull();
            output.ShouldBeOfType<List<GetCountryForViewDto>>();

            List<GetCountryForViewDto> countries = (List<GetCountryForViewDto>)output;
            countries.Count.ShouldBe(2);
            countries[0].Country.CountryCode.ShouldBe("NL");
            countries[1].Country.Description.ShouldBe("Belgium");
        }

        [MultiTenantFact]
        public async Task Should_Get_Country_By_Id()
        {
            var output = await _countriesAppService.GetCountryForView(1);

            output.ShouldNotBeNull();
            output.Country.Id.ShouldBe(1);
            output.Country.CountryCode.ShouldBe("NL");
            output.Country.Description.ShouldBe("Netherlands");
        }

        [MultiTenantFact]
        public async Task Should_Throw_Exception_When_Getting_By_Id()
        {
            await _countriesAppService.GetCountryForView(3).ShouldThrowAsync<EntityNotFoundException>();
        }
    }
}
