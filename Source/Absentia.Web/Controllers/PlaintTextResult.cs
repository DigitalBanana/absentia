using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Hosting;
using Castle.Windsor;

namespace Absentia.Web.Controllers
{
    public class PlaintTextResult : IHttpActionResult
    {
        private readonly HttpStatusCode _statusCode;
        private readonly string _data;

        public PlaintTextResult(HttpStatusCode statusCode, string data)
        {
            _statusCode = statusCode;
            _data = data;
        }

        public HttpResponseMessage CreateResponse(HttpStatusCode statusCode, string data)
        {
            HttpRequestMessage request = new HttpRequestMessage();
            request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            var x = new StringContent(data, Encoding.UTF8, "plain/text");
            HttpResponseMessage response = request.CreateResponse();
            response.Content = x;
            return response;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(CreateResponse(_statusCode, _data));
        }
    }

    public class WindsorCompositionRoot : IHttpControllerActivator
    {
        private readonly IWindsorContainer container;

        public WindsorCompositionRoot(IWindsorContainer container)
        {
            this.container = container;
        }

        public IHttpController Create(
            HttpRequestMessage request,
            HttpControllerDescriptor controllerDescriptor,
            Type controllerType)
        {
            var controller =
                (IHttpController)this.container.Resolve(controllerType);

            request.RegisterForDispose(
                new Release(
                    () => this.container.Release(controller)));

            return controller;
        }

        private class Release : IDisposable
        {
            private readonly Action release;

            public Release(Action release)
            {
                this.release = release;
            }

            public void Dispose()
            {
                this.release();
            }
        }
    }
}