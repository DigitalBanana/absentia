using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Routing;
using Absentia.Ioc;
using Absentia.Model;
using Absentia.Web.Controllers;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Absentia.Web
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configuration.Services.Replace(
                typeof(IHttpControllerActivator),
                new WindsorCompositionRoot(this.container));
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        private readonly IWindsorContainer container;

        public WebApiApplication()
        {
            this.container =
                new WindsorContainer().Install(new ComponentInstaller());
            container.Register(Classes.FromThisAssembly().BasedOn<IHttpController>().LifestyleTransient());

        }

        public override void Dispose()
        {
            this.container.Dispose();
            base.Dispose();
        }
    }
}
