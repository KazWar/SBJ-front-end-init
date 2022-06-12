using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Abp.Runtime.Validation;
using RMS.Web.Controllers;
using RMS.Web.Mvc.Services;
using RMS.Web.Website.Whirlpool.Filters;
using JsonResult = Microsoft.AspNetCore.Mvc.JsonResult;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using ValidateAntiForgeryTokenAttribute = Microsoft.AspNetCore.Mvc.ValidateAntiForgeryTokenAttribute;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using Abp.Domain.Uow;
using Microsoft.Extensions.Options;
using RMS.Web.Website.Whirlpool.Configuration;
using RestSharp;
using Newtonsoft.Json;
using System.Net.Http;
using RMS.SBJ.Forms.Dtos;
using RMS.SBJ.Registrations.Dtos;
using RMS.Web.Areas.App.Models.UniqueCodes;
using System.Text;
using System.Threading;

namespace RMS.Web.Website.Whirlpool.Controllers
{
    //[Authorize(Policy = "IsAuthenticated")]
    //[TypeFilter(typeof(IsAuthenticatedFilter))]
    //[ServiceFilter(typeof(IsAuthenticatedFilter))]
    public class ApiController : Controller
    {
        private readonly TenantConfiguration _tenantConfiguration;
        private readonly AuthenticationConfiguration _authenticationConfiguration;
        private readonly PostCodeApi _authenticationPostCodeApi;
        private readonly CancellationTokenSource cancellationTokenSource;
        // private readonly IFrontEndAppService _frontEndService;

        public ApiController(
            IOptions<TenantConfiguration> tenantConfiguration,
            IOptions<AuthenticationConfiguration> authenticationConfiguration,
            IOptions<PostCodeApi> authenticationPostCodeApi
            /*IFrontEndAppService frontEndService*/)
        {
            //_frontEndService = frontEndService;
            _tenantConfiguration = tenantConfiguration.Value;
            _authenticationConfiguration = authenticationConfiguration.Value;
            _authenticationPostCodeApi = authenticationPostCodeApi.Value;
            cancellationTokenSource = new CancellationTokenSource();

            // Set the current unit of work's tenant to the configured one from appsettings.json
            /*using (var uow = _unitOfWorkManager.Begin())
            { 
                CurrentUnitOfWork.SetFilterParameter(AbpDataFilters.MayHaveTenant, AbpDataFilters.Parameters.TenantId, _tenantConfiguration.Id);
                uow.Complete();
            }*/
        }

        [Route("/Api/GetToken")]
        [HttpGet]
        public async Task<JsonResult> GetToken()
        {
            var client = new RestClient("https://app-sbj-rms2.azurewebsites.net/api");
            var request = new RestRequest("TokenAuth/Authenticate", Method.POST)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("Abp.TenantId", _tenantConfiguration.Id.ToString())
                .AddJsonBody(new
                {
                    userNameOrEmailAddress = _authenticationConfiguration.UserNameOrEmailAddress,
                    password = _authenticationConfiguration.Password
                });

            var response = await client.ExecuteAsync(request, cancellationTokenSource.Token);

            return Json(response.Content);
        }
        public async Task<string> getBearerToken()
        {
            var token = await GetToken();
            var unpackContent = JsonConvert.DeserializeObject<AuthModel>(token.Value.ToString());

            return unpackContent.result.accessToken;
        }

        [Route("/Api/GetLocales")]
        [HttpGet]
        public async Task<JsonResult> GetLocales()
        {
            var bearer = await getBearerToken();

            //var client = new RestClient("https://app-sbj-rms2-test.azurewebsites.net/api/services/app");
            //var client = new RestClient("https://localhost:44302/api/services/app");
            var client = new RestClient("https://app-sbj-rms2.azurewebsites.net/api/services/app");
            var request = new RestRequest("Campaigns/GetCampaignLocales", Method.GET)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("Abp.TenantId", _tenantConfiguration.Id.ToString())
                .AddHeader("Authorization", "Bearer " + bearer);
            var response = await client.ExecuteAsync(request, cancellationTokenSource.Token);

            return Json(response.Content);
        }

        [Route("/Api/GetCampaignOverview")]
        [HttpGet]
        public async Task<JsonResult> GetCampaignOverview(string currentLocale)
        {
            var bearer = await getBearerToken();

            //var client = new RestClient("https://app-sbj-rms2-test.azurewebsites.net/api/services/app");
            //var client = new RestClient("https://localhost:44302/api/services/api/services/app");
            var client = new RestClient("https://app-sbj-rms2.azurewebsites.net/api/services/app");
            var request = new RestRequest("/Campaigns/GetCampaignOverview?currentLocale=" + currentLocale, Method.GET)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("Abp.TenantId", _tenantConfiguration.Id.ToString())
                .AddHeader("Authorization", "Bearer " + bearer);
            var response = await client.ExecuteAsync(request, cancellationTokenSource.Token);

            return Json(response.Content);
        }

        [Route("/Api/GetCampaignForForm")]
        [HttpGet]
        public async Task<JsonResult> GetCampaignForForm(string currentLocale, string currentCampaignCode)
        {
            var bearer = await getBearerToken();

            //var client = new RestClient("https://localhost:44302/api/services/app");
            //var client = new RestClient("https://app-sbj-rms2-test.azurewebsites.net/api/services/app");
            var client = new RestClient("https://app-sbj-rms2.azurewebsites.net/api/services/app");
            var request = new RestRequest("/Campaigns/GetCampaignForForm?currentLocale=" + currentLocale + "&currentCampaignCode=" + currentCampaignCode, Method.GET)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("Abp.TenantId", _tenantConfiguration.Id.ToString())
                .AddHeader("Authorization", "Bearer " + bearer);
            var response = await client.ExecuteAsync(request, cancellationTokenSource.Token);

            return Json(response.Content);
        }

        public async Task<JsonResult> GetFormAndProductHandling(string currentLocale, string currentCampaignId)
        {
            var bearer = await getBearerToken();

            //var client = new RestClient("https://app-sbj-rms2-test.azurewebsites.net/api/services/app");
            var client = new RestClient("https://app-sbj-rms2.azurewebsites.net/api/services/app");
            var request = new RestRequest("/FormLocales/GetFormAndProductHandeling?currentCampaignId=" + currentCampaignId + "&currentLocale=" + currentLocale, Method.GET)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("Abp.TenantId", _tenantConfiguration.Id.ToString())
                .AddHeader("Authorization", "Bearer " + bearer);
            var response = await client.ExecuteAsync(request, cancellationTokenSource.Token);

            return Json(response.Content);
        }

        [Route("/Api/SendForm")]
        [HttpPost]
        public async Task<HttpResponseMessage> SendForm([FromBody] FormRegistrationHandlingDto vueJsToRmsModel)
        {
            var bearer = await getBearerToken();

            var registrationMapper = new FormRegistrationHandlingDto
            {
                Data = vueJsToRmsModel.Data
            };

            //var client = new RestClient("https://app-sbj-rms2-test.azurewebsites.net/app");
            var client = new RestClient("https://app-sbj-rms2.azurewebsites.net/app");
            //var client = new RestClient("https://localhost:44302/app");
            var request = new RestRequest("Registrations/SendFormData", Method.POST)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("Abp.TenantId", _tenantConfiguration.Id.ToString())
                .AddHeader("Authorization", "Bearer " + bearer);
            request.AddJsonBody(registrationMapper);
            var response = await client.ExecuteAsync(request, cancellationTokenSource.Token);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            }

            return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
        }

        [Route("/Api/GetEditForm")]
        [HttpPost]
        public async Task<JsonResult> GetEditForm([FromBody] GetFormLayoutAndDataInput input)
        {
            var bearer = await getBearerToken();

            var registrationMapper = new GetFormLayoutAndDataInput
            {
                RegistrationId = input.RegistrationId,
                Password = input.Password,
                Locale = input.Locale
            };

            //var client = new RestClient("https://app-sbj-rms2-test.azurewebsites.net/api/services/app");
            //var client = new RestClient("https://localhost:44302/app");
            var client = new RestClient("https://app-sbj-rms2.azurewebsites.net/app");
            var request = new RestRequest("Registrations/GetEditForRegistration", Method.POST)
               .AddHeader("Content-Type", "application/json")
               .AddHeader("Abp.TenantId", _tenantConfiguration.Id.ToString())
               .AddHeader("Authorization", "Bearer " + bearer);
            request.AddJsonBody(registrationMapper);
            var response = await client.ExecuteAsync(request, cancellationTokenSource.Token);

            return Json(response.Content);
        }

        [Route("/Api/UpdateForm")]
        [HttpPut]
        public async Task<HttpResponseMessage> UpdateForm([FromBody] UpdateRegistrationDataDto vueJsToRmsModel)
        {
            var bearer = await getBearerToken();

            var registrationMapper = new UpdateRegistrationDataDto
            {
                Data = vueJsToRmsModel.Data
            };

            //var client = new RestClient("https://app-sbj-rms2-test.azurewebsites.net/app");
            var client = new RestClient("https://app-sbj-rms2.azurewebsites.net/app");
            //var client = new RestClient("https://localhost:44302/app");
            var request = new RestRequest("Registrations/UpdateFormData", Method.PUT)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("Abp.TenantId", _tenantConfiguration.Id.ToString())
                .AddHeader("Authorization", "Bearer " + bearer);
            request.AddJsonBody(registrationMapper);
            var response = await client.ExecuteAsync(request, cancellationTokenSource.Token);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            }

            return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
        }

        [Route("/Api/UniqueCode")]
        public async Task<ActionResult> UniqueCode(string code)
        {
            var bearer = await getBearerToken();

            //var client = new RestClient("https://localhost:44302/app");
            var client = new RestClient("https://app-sbj-rms2.azurewebsites.net/app");
            //var client = new RestClient("https://app-sbj-rms2-test.azurewebsites.net/app");
            var request = new RestRequest("UniqueCodes/IsCodeValid?code=" + code, Method.GET)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("Abp.TenantId", _tenantConfiguration.Id.ToString())
                .AddHeader("Authorization", "Bearer " + bearer);
            var response = await client.ExecuteAsync(request, cancellationTokenSource.Token);

            var deserializeCalledContent = JsonConvert.DeserializeObject<ApiCalls>(response.Content);
            if (deserializeCalledContent.Result.statusCode == "200")
            {
                return StatusCode(200);
            }

            return StatusCode(500);
        }

        [Route("/Api/SetUniqueCode")]
        public async Task<ActionResult> SetUniqueCode([FromBody] UniqueCodeViewModel model)
        {
            var bearer = await getBearerToken();

            var registrationMapper = new UniqueCodeViewModel
            {
                Code = model.Code
            };

            //var client = new RestClient("https://localhost:44302/app");
            var client = new RestClient("https://app-sbj-rms2.azurewebsites.net/app");
            //var client = new RestClient("https://app-sbj-rms2-test.azurewebsites.net/app");
            var request = new RestRequest("UniqueCodes/SetCodeUsed", Method.POST)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("Abp.TenantId", _tenantConfiguration.Id.ToString())
                .AddHeader("Authorization", "Bearer " + bearer);
            request.AddJsonBody(registrationMapper);
            var response = await client.ExecuteAsync(request, cancellationTokenSource.Token);

            var deserializeCalledContent = JsonConvert.DeserializeObject<ApiCalls>(response.Content);
            if (deserializeCalledContent.Result.statusCode == "200")
            {
                return StatusCode(200);
            }

            return StatusCode(500);
        }

        [Route("/Api/SetAddress")]
        public async Task<JsonResult> SetAddress(string postalcode, string housenumber)
        {
            var key = _authenticationPostCodeApi.key.ToString();
            var secret = _authenticationPostCodeApi.secret.ToString();
            var url = "https://api.postcode.eu/nl/v1/addresses/postcode/" + postalcode + "/" + housenumber;
            var client = new RestClient(url);

            var request = new RestRequest(Method.GET)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("mode", "no-cors")
                .AddHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(key + ":" + secret))
                );
            var response = await client.ExecuteAsync(request, cancellationTokenSource.Token);

            //var deserializeCalledContent = JsonConvert.DeserializeObject<AddressApi>(response.Content);
            if (response.StatusCode.ToString() == "OK")
            {
                return Json(response.Content);
            }

            return Json(response.StatusCode);
        }

        [Route("/Api/UniqueCodeByCampaign")]
        [HttpGet]
        public async Task<ActionResult> UniqueCodeByCampaign(string code, string? campaignCode)
        {
            var bearer = await getBearerToken();

            //var client = new RestClient("https://localhost:44302/app");
            var client = new RestClient("https://app-sbj-rms2.azurewebsites.net/app");
            //var client = new RestClient("https://app-sbj-rms2-test.azurewebsites.net/app");
            var request = new RestRequest("UniqueCodes/IsCodeValidByCampaign?code=" + code + "&campaignCode=" + campaignCode, Method.GET)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("Abp.TenantId", _tenantConfiguration.Id.ToString())
                .AddHeader("Authorization", "Bearer " + bearer);
            var response = await client.ExecuteAsync(request, cancellationTokenSource.Token);

            var deserializeCalledContent = JsonConvert.DeserializeObject<ApiCalls>(response.Content);
            if (deserializeCalledContent.Result.statusCode == "200")
            {
                return StatusCode(200);
            }

            return StatusCode(500);
        }

        [Route("/Api/SetUniqueCodeByCampaign")]
        [HttpPost]
        public async Task<ActionResult> SetUniqueCodeByCampaign([FromBody] UniqueCodeViewModel model)
        {
            var bearer = await getBearerToken();

            var registrationMapper = new UniqueCodeViewModel
            {
                Code = model.Code,
                CampaignCode = model.CampaignCode
            };

            //var client = new RestClient("https://localhost:44302/app");
            var client = new RestClient("https://app-sbj-rms2.azurewebsites.net/app");
            //var client = new RestClient("https://app-sbj-rms2-test.azurewebsites.net/app");
            var request = new RestRequest("UniqueCodes/SetCodeUsedByCampaign", Method.POST)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("Abp.TenantId", _tenantConfiguration.Id.ToString())
                .AddHeader("Authorization", "Bearer " + bearer);
            request.AddJsonBody(registrationMapper);
            var response = await client.ExecuteAsync(request, cancellationTokenSource.Token);

            var deserializeCalledContent = JsonConvert.DeserializeObject<ApiCalls>(response.Content);
            if (deserializeCalledContent.Result.statusCode == "200")
            {
                return StatusCode(200);
            }

            return StatusCode(500);
        }

    }

}