using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Absentia.Domain.CertificateLoaders
{
    public class LocalDevelopmentCertificateLoader : ICertificateLoader
    {

        private const string CertPassword = "HARDCODED_LOCAL_PFX_PASSWORD";

        private IAppSetting _appSetting;

        public LocalDevelopmentCertificateLoader(IAppSetting appSetting)
        {
            _appSetting = appSetting;
        }
        public ClientAssertionCertificate CreateClientAssertionCertificate()
        {
            string certfile = "absentia.pfx";

            //relative path for cert.....

            var certPath = Path.GetFullPath( Path.Combine(
                Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                @"C\localDev\Certificates", certfile));

            if (!File.Exists(certPath))
            {
                throw new ApplicationException("could not locate certificate for Outlook REST API access - make sure you have the pfx to run locally");
            }

            X509Certificate2 cert = new X509Certificate2(certPath, CertPassword, X509KeyStorageFlags.DefaultKeySet);
            ClientAssertionCertificate cac = new ClientAssertionCertificate(_appSetting.AbsentiaAadApplicationId, cert);
            return cac;

        }
    }
}