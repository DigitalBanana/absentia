using Absentia.Domain.CertificateLoaders;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Absentia.Domain
{
    public interface ITokenService
    {
        AuthenticationResult GetCredentialsAccessToken(string resourceId);

        AuthenticationResult GetCertificateAccessToken(string resourceId);
    }


    public class TokenService : ITokenService
    {
        private IAppSetting _appSetting;
        private ICertificateLoader _certificateLoader;

        public TokenService(IAppSetting appSetting, ICertificateLoader certificateLoader)
        {
            _certificateLoader = certificateLoader;
            _appSetting = appSetting;
        }



        public AuthenticationResult GetCredentialsAccessToken(string resourceId)
        {

            ClientCredential credential = new ClientCredential(_appSetting.AbsentiaAadApplicationId,
                _appSetting.AbsentiaAadApplicationKey);

            string authority = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                _appSetting.AadTokenEndpoint, _appSetting.TenantId);

            AuthenticationContext authContext = new AuthenticationContext(authority, false);

            AuthenticationResult authResult = authContext.AcquireTokenAsync(
                resourceId,
                credential).Result;
            return authResult;
        }

        public AuthenticationResult GetCertificateAccessToken(string resourceId)
        {

            string authority = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                _appSetting.AadTokenEndpoint, _appSetting.TenantId);

            AuthenticationContext authContext = new AuthenticationContext(authority, false);

            var cac = _certificateLoader.CreateClientAssertionCertificate();

            var authenticationResult = authContext.AcquireTokenAsync(resourceId, cac).Result;
            return authenticationResult;
        }
    }
    

} 