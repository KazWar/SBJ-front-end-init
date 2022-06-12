using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace RMS.Web.Website.Whirlpool.Handlers
{
    public class IsAuthenticatedRequirement : AuthorizationHandler<IsAuthenticatedRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsAuthenticatedRequirement requirement)
        {
            context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
