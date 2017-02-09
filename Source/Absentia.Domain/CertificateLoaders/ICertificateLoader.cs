using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Absentia.Domain.CertificateLoaders
{
    public interface ICertificateLoader
    {
        
        ClientAssertionCertificate CreateClientAssertionCertificate();
    }
}