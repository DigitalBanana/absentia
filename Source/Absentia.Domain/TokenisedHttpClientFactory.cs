using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Absentia.Domain
{
    public interface ITokenisedHttpClientFactory
    {
        HttpClient GetGraphResourceClient();
        HttpClient GetOutlookResourceClient();
    }
    public class TokenisedHttpClientFactory : ITokenisedHttpClientFactory
    {
        private ITokenService _tokenService;
        private IAppSetting _appSetting;

        public TokenisedHttpClientFactory(ITokenService tokenService, IAppSetting appSetting)
        {
            _appSetting = appSetting;
            _tokenService = tokenService;
        }

        private HttpClient GetResourseClient(string accessToken)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        public HttpClient GetGraphResourceClient()
        {
            AuthenticationResult authResult = _tokenService.GetCredentialsAccessToken(_appSetting.MicrosoftGraphResource);
            return GetResourseClient(authResult.AccessToken);
        }

        public HttpClient GetOutlookResourceClient()
        {
            AuthenticationResult authResult = _tokenService.GetCertificateAccessToken(_appSetting.OutlookResource);

            HttpClient client = new HttpClient();
            return GetResourseClient(authResult.AccessToken);
        }

    }
}