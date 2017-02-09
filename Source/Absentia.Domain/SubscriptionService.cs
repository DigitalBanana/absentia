using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Absentia.Domain.DTO;
using Absentia.Model.Entities;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;

namespace Absentia.Domain
{
    public interface ISubscriptionService
    {
        Subscription AddSubscription(string username);
        DateTime RenewSubscription(string subscriptionId);
        void DeleteSubscription(string subscriptionId);
    }

    public class SubscriptionService : ISubscriptionService
    {
        private IAppSetting _appSetting;
        private TimeSpan _subscriptionLength;
        private ITokenisedHttpClientFactory _tokenisedHttpClientFactory;

        public SubscriptionService(IAppSetting appSetting, TimeSpan subscriptionLength, ITokenisedHttpClientFactory tokenisedHttpClientFactory)
        {
            _tokenisedHttpClientFactory = tokenisedHttpClientFactory;
            _appSetting = appSetting;
            _subscriptionLength = subscriptionLength;
        }

        public Subscription AddSubscription(string username)
        {

            //graph client is used to access graph API (create subscriptions, unpack notifications)
            var graphClient = _tokenisedHttpClientFactory.GetGraphResourceClient();

            //create subscription payload
            var subscriptionPayload = new Subscription
            {
                Resource = string.Format($"Users('{username}')/events?$filter=contains(subject,'Leave')"),
                ChangeType = "updated",
                NotificationUrl = _appSetting.AbsentiaNotificationUrl,
                ClientState = Guid.NewGuid().ToString(),
                ExpirationDateTime = DateTime.UtcNow + _subscriptionLength
            };
            string contentString = JsonConvert.SerializeObject(subscriptionPayload,
                new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore});


            //assemble into subscriptions request
            string subscriptionsEndpoint = string.Format($"{_appSetting.MicrosoftGraphResource}/v1.0/subscriptions/");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, subscriptionsEndpoint);
            request.Content = new StringContent(contentString, System.Text.Encoding.UTF8, "application/json");

            // Send the request and parse the response.
            HttpResponseMessage response = graphClient.SendAsync(request).Result;
            if (response.IsSuccessStatusCode)
            {
                // Parse the JSON response.
                string stringResult = response.Content.ReadAsStringAsync().Result;
                var createdSubscription = JsonConvert.DeserializeObject<Subscription>(stringResult);
                if (createdSubscription != null)
                {
                    return createdSubscription;
                }
                throw new ApplicationException(string.Format($"could not deserialize response: {stringResult}"));
            }

            //log error
            var callResult = response.Content.ReadAsStringAsync().Result;
            var failureReason =
                string.Format($"AddSubscription. HTTP status code:{response.StatusCode}. Error:{callResult}");
            throw new ApplicationException(failureReason);

        }

        public void DeleteSubscription(string subscriptionId)
        {
            var client = _tokenisedHttpClientFactory.GetGraphResourceClient();
            string resourceUri = string.Format($"{_appSetting.MicrosoftGraphResource}/v1.0/subscriptions/{subscriptionId}");
            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("DELETE"), resourceUri);
            HttpResponseMessage response = client.SendAsync(request).Result;
            if (!response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                var failureReason = string.Format($"DeleteSubscription. HTTP status code:{response.StatusCode}. Error:{result}");
                throw new ApplicationException(failureReason);
            }
        }

        public DateTime RenewSubscription(string subscriptionId)
        {
            var client = _tokenisedHttpClientFactory.GetGraphResourceClient();
            string resourceUri = string.Format($"{_appSetting.MicrosoftGraphResource}/v1.0/subscriptions/{subscriptionId}");
            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), resourceUri);
            var subscription = new 
            {
                expirationDateTime = DateTime.UtcNow.Add(_subscriptionLength)
            };
            string contentString = JsonConvert.SerializeObject(subscription, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            request.Content = new StringContent(contentString, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.SendAsync(request).Result;
            if (!response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                var failureReason = string.Format($"RenewSubscription. HTTP status code:{response.StatusCode}. Error:{result}");
                throw new ApplicationException(failureReason);
            }
            return subscription.expirationDateTime;
        }
    }
}
