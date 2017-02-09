using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Results;
using Absentia.Model;
using Absentia.Model.Entities;
using Absentia.Web.DTO;
using Swashbuckle.Swagger.Annotations;

namespace Absentia.Web.Controllers
{
    public class NotifyController : ApiController
    {
        private IRepository _repository;

        public NotifyController(IRepository repository)
        {
            _repository = repository;
        }

        public IHttpActionResult Post([FromBody] Notifications notificationsPayload,
            [FromUri] string validationToken = "")
        {
            if (!string.IsNullOrEmpty(validationToken))
            {
                // Validate the new subscription by sending the token back to Microsoft Graph.
                // This response is required for each subscription.
                return new PlaintTextResult(HttpStatusCode.OK, validationToken);

            }

            // Parse the received notifications.
            try
            {
                foreach (var notification in notificationsPayload.Value)
                {
                    _repository.AddNotifcation(notification);
                }
            }
            catch (Exception ex)
            {
                var s = ex.Message;
                // TODO: Handle the exception.
                // Still return a 202 so the service doesn't resend the notification.
            }
            return new StatusCodeResult(HttpStatusCode.Accepted, Request);
        }
    }

}
