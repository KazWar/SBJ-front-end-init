using System;
using System.Linq;
using System.Text;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Castle.Core.Logging;
using Abp;
using Abp.Configuration;
using Abp.Authorization;
using Abp.Extensions;
using Abp.MultiTenancy;
using Abp.Runtime.Caching;
using Abp.Zero.Configuration;
using Abp.Authorization.Users;
using Abp.Runtime.Security;
using RMS.Identity;
using RMS.Configuration;
using RMS.Security.Recaptcha;
using RMS.Authorization;
using RMS.Authorization.Users;
using RMS.MultiTenancy;
using RMS.Web.Models.TokenAuth;
using RMS.Web.Authentication.TwoFactor;
using RMS.Web.Authentication.JwtBearer;
using RMS.Authentication.TwoFactor.Google;
using RMS.Web.Website.Whirlpool.Configuration;

namespace RMS.Web.Website.Whirlpool.Services
{
    public class AuthenticationAppService : RMSAppServiceBase, IAuthenticationAppService
    {
        private const string UserIdentifierClaimType = "http://aspnetzero.com/claims/useridentifier";

        private readonly LogInManager _logInManager;
        private readonly ISettingManager _settingManager;
        private readonly UserManager _userManager;
        private readonly ICacheManager _cacheManager;
        private readonly IJwtSecurityStampHandler _securityStampHandler;
        private readonly TokenAuthConfiguration _configuration;
        private readonly AbpLoginResultTypeHelper _abpLoginResultTypeHelper;
        private readonly ITenantCache _tenantCache;
        private readonly GoogleAuthenticatorProvider _googleAuthenticatorProvider;
        private readonly IdentityOptions _identityOptions;
        private readonly IOptions<JwtBearerOptions> _jwtOptions;
        private readonly TenantConfiguration _tenantConfiguration;
        private readonly IConfiguration _appConfiguration;

        private readonly ITenantAppService _tenantAppService;

        public AuthenticationAppService(
            LogInManager logInManager,
            ISettingManager settingManager,
            UserManager userManager,
            ICacheManager cacheManager,
            IJwtSecurityStampHandler securityStampHandler,
            TokenAuthConfiguration configuration,
            AbpLoginResultTypeHelper abpLoginResultTypeHelper,
            ITenantCache tenantCache,
            GoogleAuthenticatorProvider googleAuthenticatorProvider,
            IOptions<IdentityOptions> identityOptions,
            IOptions<JwtBearerOptions> jwtOptions,
            IOptions<TenantConfiguration> tenantConfiguration,
            IConfiguration appConfiguration,
            ITenantAppService tenantAppService)
        {
            _logInManager = logInManager;
            _settingManager = settingManager;
            _cacheManager = cacheManager;
            _userManager = userManager;
            _securityStampHandler = securityStampHandler;
            _abpLoginResultTypeHelper = abpLoginResultTypeHelper;
            _tenantCache = tenantCache;
            _googleAuthenticatorProvider = googleAuthenticatorProvider;
            _identityOptions = identityOptions.Value;
            _jwtOptions = jwtOptions;
            _configuration = configuration;
            _tenantConfiguration = tenantConfiguration.Value;
            _appConfiguration = appConfiguration;

            _tenantAppService = tenantAppService;
        }

        public new ILogger Logger => NullLogger.Instance;

        public IRecaptchaValidator RecaptchaValidator { get; set; }

        public async Task<AuthenticateResultModel> Authenticate(AuthenticateModel model)
        {
            /*if (UseCaptchaOnLogin())
            {
                await ValidateReCaptcha(model.CaptchaResponse);
            }*/
            var tenants = _tenantAppService.GetTenants(new MultiTenancy.Dto.GetTenantsInput { Filter = "" });

            var loginResult = await GetLoginResultAsync(
                model.UserNameOrEmailAddress,
                model.Password,
                GetTenancyNameOrNull()
            );

            

            var returnUrl = model.ReturnUrl;

            if (model.SingleSignIn.HasValue && model.SingleSignIn.Value && loginResult.Result == AbpLoginResultType.Success)
            {
                loginResult.User.SetSignInToken();
                returnUrl = AddSingleSignInParametersToReturnUrl(model.ReturnUrl, loginResult.User.SignInToken, loginResult.User.Id, loginResult.User.TenantId);
            }

            //Password reset
            if (loginResult.User.ShouldChangePasswordOnNextLogin)
            {
                loginResult.User.SetNewPasswordResetCode();
                return new AuthenticateResultModel
                {
                    ShouldResetPassword = true,
                    PasswordResetCode = loginResult.User.PasswordResetCode,
                    UserId = loginResult.User.Id,
                    ReturnUrl = returnUrl
                };
            }

            if (loginResult.Tenant != null)
            { 
                //Two factor auth
                await _userManager.InitializeOptionsAsync(loginResult.Tenant.Id);
            }

            string twoFactorRememberClientToken = null;
            if (await IsTwoFactorAuthRequiredAsync(loginResult, model))
            {
                if (model.TwoFactorVerificationCode.IsNullOrEmpty())
                {
                    //Add a cache item which will be checked in SendTwoFactorAuthCode to prevent sending unwanted two factor code to users.
                    _cacheManager
                        .GetTwoFactorCodeCache()
                        .Set(
                            loginResult.User.ToUserIdentifier().ToString(),
                            new TwoFactorCodeCacheItem()
                        );

                    return new AuthenticateResultModel
                    {
                        RequiresTwoFactorVerification = true,
                        UserId = loginResult.User.Id,
                        TwoFactorAuthProviders = await _userManager.GetValidTwoFactorProvidersAsync(loginResult.User),
                        ReturnUrl = returnUrl
                    };
                }

                twoFactorRememberClientToken = await TwoFactorAuthenticateAsync(loginResult.User, model);
            }

            // One Concurrent Login 
            if (AllowOneConcurrentLoginPerUser())
            {
                await _userManager.UpdateSecurityStampAsync(loginResult.User);
                await _securityStampHandler.SetSecurityStampCacheItem(loginResult.User.TenantId, loginResult.User.Id, loginResult.User.SecurityStamp);
                loginResult.Identity.ReplaceClaim(new Claim(AppConsts.SecurityStampKey, loginResult.User.SecurityStamp));
            }

            var accessToken = CreateAccessToken(await CreateJwtClaims(loginResult.Identity, loginResult.User));
            var refreshToken = CreateRefreshToken(await CreateJwtClaims(loginResult.Identity, loginResult.User, tokenType: TokenType.RefreshToken));

            return new AuthenticateResultModel
            {
                AccessToken = accessToken,
                ExpireInSeconds = (int)_configuration.AccessTokenExpiration.TotalSeconds,
                RefreshToken = refreshToken,
                RefreshTokenExpireInSeconds = (int)_configuration.RefreshTokenExpiration.TotalSeconds,
                EncryptedAccessToken = GetEncryptedAccessToken(accessToken),
                TwoFactorRememberClientToken = twoFactorRememberClientToken,
                UserId = loginResult.User.Id,
                ReturnUrl = returnUrl
            };
        }

        private bool UseCaptchaOnLogin()
        {
            return _settingManager.GetSettingValue<bool>(AppSettings.UserManagement.UseCaptchaOnLogin);
        }

        private async Task<AbpLoginResult<Tenant, User>> GetLoginResultAsync(string usernameOrEmailAddress, string password, string tenancyName)
        {
            var loginResult = await _logInManager.LoginAsync(usernameOrEmailAddress, password, tenancyName);

            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    return loginResult;
                default:
                    throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(loginResult.Result, usernameOrEmailAddress, tenancyName);
            }
        }

        private string GetTenancyNameOrNull()
        {
            if (AbpSession.TenantId.HasValue)
            {
                return _tenantCache.GetOrNull(AbpSession.TenantId.Value)?.TenancyName;
            }

            return _tenantConfiguration.Name;
        }

        private static string AddSingleSignInParametersToReturnUrl(string returnUrl, string signInToken, long userId, int? tenantId)
        {
            returnUrl += (returnUrl.Contains("?") ? "&" : "?") +
                         "accessToken=" + signInToken +
                         "&userId=" + Convert.ToBase64String(Encoding.UTF8.GetBytes(userId.ToString()));
            if (tenantId.HasValue)
            {
                returnUrl += "&tenantId=" + Convert.ToBase64String(Encoding.UTF8.GetBytes(tenantId.Value.ToString()));
            }

            return returnUrl;
        }

        private async Task<bool> IsTwoFactorAuthRequiredAsync(AbpLoginResult<Tenant, User> loginResult, AuthenticateModel authenticateModel)
        {
            if (!await _settingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.TwoFactorLogin.IsEnabled))
            {
                return false;
            }

            if (!loginResult.User.IsTwoFactorEnabled)
            {
                return false;
            }

            if ((await _userManager.GetValidTwoFactorProvidersAsync(loginResult.User)).Count <= 0)
            {
                return false;
            }

            if (await TwoFactorClientRememberedAsync(loginResult.User.ToUserIdentifier(), authenticateModel))
            {
                return false;
            }

            return true;
        }

        private async Task<string> TwoFactorAuthenticateAsync(User user, AuthenticateModel authenticateModel)
        {
            var twoFactorCodeCache = _cacheManager.GetTwoFactorCodeCache();
            var userIdentifier = user.ToUserIdentifier().ToString();
            var cachedCode = await twoFactorCodeCache.GetOrDefaultAsync(userIdentifier);
            var provider = _cacheManager.GetCache("ProviderCache").Get("Provider", cache => cache).ToString();

            if (provider == GoogleAuthenticatorProvider.Name)
            {
                if (!await _googleAuthenticatorProvider.ValidateAsync("TwoFactor", authenticateModel.TwoFactorVerificationCode, _userManager, user))
                {
                    /*throw; new UserFriendlyException(L("InvalidSecurityCode"));*/
                    return null;
                }
            }
            else if (cachedCode?.Code == null || cachedCode.Code != authenticateModel.TwoFactorVerificationCode)
            {
                //throw new UserFriendlyException(L("InvalidSecurityCode"));
                return null;
            }

            //Delete from the cache since it was a single usage code
            await twoFactorCodeCache.RemoveAsync(userIdentifier);

            if (authenticateModel.RememberClient)
            {
                if (await _settingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.TwoFactorLogin.IsRememberBrowserEnabled))
                {
                    return CreateAccessToken(new[]
                        {
                            new Claim(UserIdentifierClaimType, user.ToUserIdentifier().ToString())
                        },
                        TimeSpan.FromDays(365)
                    );
                }
            }

            return null;
        }

        private bool AllowOneConcurrentLoginPerUser()
        {
            return _settingManager.GetSettingValue<bool>(AppSettings.UserManagement.AllowOneConcurrentLoginPerUser);
        }

        private string CreateAccessToken(IEnumerable<Claim> claims, TimeSpan? expiration = null)
        {
            return CreateToken(claims, expiration ?? _configuration.AccessTokenExpiration);
        }

        private string CreateRefreshToken(IEnumerable<Claim> claims)
        {
            return CreateToken(claims, AppConsts.RefreshTokenExpiration);
        }

        private async Task<IEnumerable<Claim>> CreateJwtClaims(ClaimsIdentity identity, User user, TimeSpan? expiration = null, TokenType tokenType = TokenType.AccessToken)
        {
            var tokenValidityKey = Guid.NewGuid().ToString();
            var claims = identity.Claims.ToList();
            var nameIdClaim = claims.First(c => c.Type == _identityOptions.ClaimsIdentity.UserIdClaimType);

            if (_identityOptions.ClaimsIdentity.UserIdClaimType != JwtRegisteredClaimNames.Sub)
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Sub, nameIdClaim.Value));
            }

            claims.AddRange(new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim(AppConsts.TokenValidityKey, tokenValidityKey),
                new Claim(AppConsts.UserIdentifier, user.ToUserIdentifier().ToUserIdentifierString()),
                new Claim(AppConsts.TokenType, tokenType.To<int>().ToString())
             });

            if (!expiration.HasValue)
            {
                expiration = tokenType == TokenType.AccessToken
                    ? _configuration.AccessTokenExpiration
                    : _configuration.RefreshTokenExpiration;
            }

            _cacheManager
                .GetCache(AppConsts.TokenValidityKey)
                .Set(tokenValidityKey, "", absoluteExpireTime: expiration);

            await _userManager.AddTokenValidityKeyAsync(
                user,
                tokenValidityKey,
                DateTime.UtcNow.Add(expiration.Value)
            );

            return claims;
        }

        private static string GetEncryptedAccessToken(string accessToken)
        {
            return SimpleStringCipher.Instance.Encrypt(accessToken, AppConsts.DefaultPassPhrase);
        }

        private async Task<bool> TwoFactorClientRememberedAsync(UserIdentifier userIdentifier, AuthenticateModel authenticateModel)
        {
            if (!await _settingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.TwoFactorLogin.IsRememberBrowserEnabled))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(authenticateModel.TwoFactorRememberClientToken))
            {
                return false;
            }

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidAudience = _configuration.Audience,
                    ValidIssuer = _configuration.Issuer,
                    IssuerSigningKey = _configuration.SecurityKey
                };

                foreach (var validator in _jwtOptions.Value.SecurityTokenValidators)
                {
                    if (validator.CanReadToken(authenticateModel.TwoFactorRememberClientToken))
                    {
                        try
                        {
                            var principal = validator.ValidateToken(authenticateModel.TwoFactorRememberClientToken, validationParameters, out _);
                            var useridentifierClaim = principal.FindFirst(c => c.Type == UserIdentifierClaimType);
                            if (useridentifierClaim == null)
                            {
                                return false;
                            }

                            return useridentifierClaim.Value == userIdentifier.ToString();
                        }
                        catch (Exception ex)
                        {
                            Logger.Debug(ex.ToString(), ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.ToString(), ex);
            }

            return false;
        }

        private string CreateToken(IEnumerable<Claim> claims, TimeSpan? expiration = null)
        {
            var now = DateTime.UtcNow;

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration.Issuer,
                audience: _configuration.Audience,
                claims: claims,
                notBefore: now,
                signingCredentials: _configuration.SigningCredentials,
                expires: expiration == null ?
                    (DateTime?)null :
                    now.Add(expiration.Value)
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }


/*        private async Task ValidateReCaptcha(string captchaResponse)
        {
            var requestUserAgent = Request.Headers["User-Agent"].ToString();
            if (!requestUserAgent.IsNullOrWhiteSpace() && WebConsts.ReCaptchaIgnoreWhiteList.Contains(requestUserAgent.Trim()))
            {
                return;
            }

            await RecaptchaValidator.ValidateAsync(captchaResponse);
        }*/
    }
}
