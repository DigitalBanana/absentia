using Newtonsoft.Json;

namespace Absentia.Domain.DTO
{
    public class Event
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "subject")]
        public string Subject { get; set; }

        [JsonProperty(PropertyName = "bodyPreview")]
        public string BodyPreview { get; set; }

    }
}