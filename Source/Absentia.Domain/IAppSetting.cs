namespace Absentia.Domain
{
    public interface IAppSetting
    {
        string TenantId { get; set; }
        string AadTokenEndpoint { get; set; }
        string MicrosoftGraphResource { get; set; }
        string OutlookResource { get; set; }
        string AbsentiaAadApplicationId { get; set; }
        string AbsentiaAadApplicationKey { get; set; }
        string AbsentiaNotificationUrl { get; set; }
        string CertificateThumbprint { get; set; }

    }
}