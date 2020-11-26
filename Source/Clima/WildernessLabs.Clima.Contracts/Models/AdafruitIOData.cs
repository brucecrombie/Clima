using System;
using System.Text.Json.Serialization;

namespace Clima.Contracts.Models
{
    public class AdafruitIOData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("value")]
        public string Value { get; set; }
       
        [JsonPropertyName("feed_id")]
        public int FeedId { get; set; }
        
        [JsonPropertyName("feed_key")]
        public string FeedKey { get; set; }
        
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        public AdafruitIOData()
        {
        }
    }
}
