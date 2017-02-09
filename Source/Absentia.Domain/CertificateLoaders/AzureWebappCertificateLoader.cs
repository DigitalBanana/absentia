using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Absentia.Domain.CertificateLoaders
{
    public class AzureWebappCertificateLoader : ICertificateLoader
    {

        private IAppSetting _appSetting;

        public AzureWebappCertificateLoader(IAppSetting appSetting)
        {
            _appSetting = appSetting;
        }
        public ClientAssertionCertificate CreateClientAssertionCertificate()
        {
            ClientAssertionCertificate cac = null;
            X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            certStore.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection certCollection = certStore.Certificates.Find(
                                       X509FindType.FindByThumbprint,
                                         _appSetting.CertificateThumbprint,
                                       false);
            // Get the first cert with the thumbprint
            if (certCollection.Count > 0)
            {
                X509Certificate2 cert = certCollection[0];
                // Use certificate
                cac = new ClientAssertionCertificate(_appSetting.AbsentiaAadApplicationId, cert);
            }
            certStore.Close();
            return cac;
        }
    }
}