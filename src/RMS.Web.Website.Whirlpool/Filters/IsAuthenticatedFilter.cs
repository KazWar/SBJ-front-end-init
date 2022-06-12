using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using RMS.Web.Models.TokenAuth;
using RMS.Web.Website.Whirlpool.Services;
using Microsoft.Extensions.Options;
using RMS.Web.Website.Whirlpool.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace RMS.Web.Website.Whirlpool.Filters
{
    public class IsAuthenticatedFilter : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        private readonly AuthenticationConfiguration _authenticationConfiguration;
        private readonly IAuthenticationAppService _authenticationAppService;

        public IsAuthenticatedFilter(
            IOptions<AuthenticationConfiguration> authenticationConfiguration,
            IAuthenticationAppService authenticationAppService)
        {
            _authenticationConfiguration = authenticationConfiguration.Value;
            _authenticationAppService = authenticationAppService;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var authenticated = await _authenticationAppService.Authenticate(new AuthenticateModel
            {
                UserNameOrEmailAddress = _authenticationConfiguration?.UserNameOrEmailAddress,
                Password = _authenticationConfiguration?.Password
            });

            if (string.IsNullOrWhiteSpace(authenticated.AccessToken))
            {
                context.Result = new UnauthorizedResult();
            }
        }

    }
}
