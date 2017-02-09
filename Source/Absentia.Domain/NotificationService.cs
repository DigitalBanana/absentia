using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Absentia.Domain.DTO;
using Absentia.Model;
using Absentia.Model.Entities;
using Newtonsoft.Json;

namespace Absentia.Domain
{
    public interface INotificationService
    {
        void ProcessNotifications();
    }

    public class NotificationService : INotificationService
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly IAppSetting _setting;
        private readonly IRepository _repository;
        private readonly ITokenisedHttpClientFactory _tokenisedHttpClientFactory;

        public NotificationService(IRepository repository, ITokenisedHttpClientFactory tokenisedHttpClientFactory, ISubscriptionService subscriptionService, IAppSetting setting )
        {
            _tokenisedHttpClientFactory = tokenisedHttpClientFactory;
            _repository = repository;
            _setting = setting;
            _subscriptionService = subscriptionService;
        }

        public void ProcessNotifications()
        {
            var notifications = _repository.GetPendingNotifications().ToList();
            if (!notifications.Any())
                return;

            //graph client is used to access graph API (create subscriptions, unpack notifications)
            var graphClient = _tokenisedHttpClientFactory.GetGraphResourceClient();
            //outlook client is used to access graph API (update calendars)
            var outlookClient = _tokenisedHttpClientFactory.GetOutlookResourceClient();

            foreach (var notification in notifications)
            {
                



                try
                {

                    var subscription = _repository.GetSubscription(notification.SubscriptionId);
                    if (subscription == null)
                    {
                        //if the subscription doesnt exist in the database then stop further notifications?
                        _subscriptionService.DeleteSubscription(notification.SubscriptionId);
                        _repository.SetNotificationProcessingResult(notification.NotificationId, true, "Deleted Orphan Subscription");
                        continue;
                    }

                    string serviceRootUrl = string.Format($"{_setting.MicrosoftGraphResource}/v1.0/");
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, serviceRootUrl + notification.Resource);

                    // Send the 'GET' request to retreive the event 
                    var response = graphClient.SendAsync(request).Result;

                    // Get the messages from the JSON response.
                    if (response.IsSuccessStatusCode)
                    {
                        string stringResult = response.Content.ReadAsStringAsync().Result;

                        Event e = JsonConvert.DeserializeObject<Event>(stringResult);

                        //Appogee events have leave type in the body but "Approved Leave" in the subject
                        if (e.BodyPreview.Contains("Leave Type: Working From Home") &&
                            e.Subject.Contains(": Approved Leave"))
                        {

                            var username = _repository.GetUsernameFromSubscription(notification.SubscriptionId);
                            if (string.IsNullOrEmpty(username))
                            {
                                throw new ApplicationException(string.Format($"could not locate subscription '{notification.SubscriptionId}' for notification '{notification.NotificationId}'"));
                            }

                            //for Graph PATCH requests supply only those properties to be updated
                            var update = new
                            {
                                Subject = e.Subject.Replace(": Approved Leave", ": Approved WFH"),
                                ShowAs = "4"
                            };

                            string patchUrl =
                                string.Format($"{_setting.OutlookResource}/api/v1.0/Users('{username}')/events/{e.Id}");


                            HttpRequestMessage updateRequest = new HttpRequestMessage(new HttpMethod("PATCH"), patchUrl);

                            string contentString = JsonConvert.SerializeObject(update,
                                new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore});
                            updateRequest.Content = new StringContent(contentString, System.Text.Encoding.UTF8,
                                "application/json");


                            HttpResponseMessage updateResponse = outlookClient.SendAsync(updateRequest).Result;
                            if (!updateResponse.IsSuccessStatusCode)
                            {
                                throw new ApplicationException(string.Format($"could not update entity: {e.Id}"));
                            }
                            _repository.SetNotificationProcessingResult(notification.NotificationId, true, "processed successfully");
                        }
                        else
                        {
                            _repository.SetNotificationProcessingResult(notification.NotificationId, true, string.Format($"no record to process, did not match predicate: Subject {e.Subject}, Body {e.BodyPreview}"));
                        }

                    }
                    else
                    {
                        throw new ApplicationException(String.Format($"failed processing. status code {response.StatusCode} .message {response.ReasonPhrase}"));
                    }

                }
                catch
                    (Exception ex)
                {
                    _repository.SetNotificationProcessingResult(notification.NotificationId,false, ex.Message);
                }
            }
        }
    }
}