using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Absentia.Domain;
using Absentia.Ioc;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Absentia.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var container =
                new WindsorContainer().Install(new ComponentInstaller());

            container.Register(
                Component.For<SubscriptionManager>()
                    .DependsOn(Dependency.OnValue("subscriptionRenewalTimeLeftTrigger", new TimeSpan(0,10,0,0)))
                    );


            var manager = container.Resolve<SubscriptionManager>();
            manager.Process();

        }
    }
}
