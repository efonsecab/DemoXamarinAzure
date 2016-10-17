using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomerOffers.SharedEntities
{
    public class NotificationHubMessage<T>
    {
        [JsonProperty(PropertyName ="data")]
        public NotificationHubMessageData<T> Data { get; set; } = null;
    }

    public class NotificationHubMessageData<T>
    {
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; } = string.Empty;
        [JsonProperty(PropertyName = "entitydata")]
        public T EntiyData { get; set; } = default(T);
    }
}
