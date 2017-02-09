using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using Absentia.Domain;
using Absentia.Domain.CertificateLoaders;
using Absentia.Model;
using Castle.Components.DictionaryAdapter;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Absentia.Ioc
{
    public class ComponentInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IRepository>().ImplementedBy<Repository>());

            container.Register(
                Component.For<ISubscriptionService>()
                    .ImplementedBy<SubscriptionService>()
                    .DependsOn(Dependency.OnValue("subscriptionLength", new TimeSpan(0,70,0,0))));
            
            container.Register(Component.For<INotificationService>().ImplementedBy<NotificationService>());
            container.Register(Component.For<ITokenService>().ImplementedBy<TokenService>());

            container.Register(
                Component.For<ITokenisedHttpClientFactory>()
                    .ImplementedBy<TokenisedHttpClientFactory>()
                    .LifestyleSingleton());

            container.Register(
                Component.For<IAppSetting>()
                    .UsingFactoryMethod(
                        () => new DictionaryAdapterFactory().GetAdapter<IAppSetting>(AppSettings()))
                    .LifestyleSingleton());

            //detect azure runtime....
            if (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME")))
            {
                //if in Azure get cert from WebApp
                container.Register(
                    Component.For<ICertificateLoader>()
                        .ImplementedBy<AzureWebappCertificateLoader>()
                        .LifestyleSingleton());
            }
            else
            {
                //else load locally
                container.Register(
                    Component.For<ICertificateLoader>()
                        .ImplementedBy<LocalDevelopmentCertificateLoader>()
                        .LifestyleSingleton());
            }
        }

        private static NameValueCollection AppSettings()
        {
            var nvm = new NameValueCollection();
            foreach (string key in ConfigurationManager.AppSettings)
            {
                nvm.Add(key, ConfigurationManager.AppSettings[key]);
            }
            return nvm;
        }
    }


}
