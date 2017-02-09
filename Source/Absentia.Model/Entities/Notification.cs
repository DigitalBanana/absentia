using System;
using Newtonsoft.Json;

namespace Absentia.Model.Entities
{
    public class Notification
    {
        public Notification()
        {
            NotificationRecievedTimeUtc = DateTime.UtcNow;
        }

        [JsonIgnore]
        public int NotificationId { get; set; }
        [JsonIgnore]
        public string ProcessingMessage { get; set; }
        [JsonIgnore]
        public bool? ProcessingResult { get; set; }
        [JsonIgnore]
        public DateTime NotificationRecievedTimeUtc { get; set; }
        public DateTime? NotificationProcessedTimeUtc { get; set; }

        // The type of change.
        [JsonProperty(PropertyName = "changeType")]
        public string ChangeType { get; set; }

        // The client state used to verify that the notification is from Microsoft Graph. Compare the value received with the notification to the value you sent with the subscription request.
        [JsonProperty(PropertyName = "clientState")]
        public string ClientState { get; set; }

        // The endpoint of the resource that changed. For example, a message uses the format ../Users/{user-id}/Messages/{message-id}
        [JsonProperty(PropertyName = "resource")]
        public string Resource { get; set; }

        // The UTC date and time when the webhooks subscription expires.
        [JsonProperty(PropertyName = "subscriptionExpirationDateTime")]
        public DateTime SubscriptionExpirationDateTime { get; set; }

        // The unique identifier for the webhooks subscription.
        [JsonProperty(PropertyName = "subscriptionId")]
        public string SubscriptionId { get; set; }

        // Properties of the changed resource.
        [JsonProperty(PropertyName = "resourceData")]
        public ResourceData ResourceData { get; set; }
    }
}