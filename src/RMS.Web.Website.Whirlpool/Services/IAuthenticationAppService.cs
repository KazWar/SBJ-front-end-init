using Abp.Application.Services;
using RMS.Web.Models.TokenAuth;
using System.Threading.Tasks;

namespace RMS.Web.Website.Whirlpool.Services
{
    public interface IAuthenticationAppService : IApplicationService
    {
        Task<AuthenticateResultModel> Authenticate(AuthenticateModel model);
    }
}
