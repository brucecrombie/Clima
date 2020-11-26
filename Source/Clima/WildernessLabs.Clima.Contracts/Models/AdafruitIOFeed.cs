using System;
using System.Text.Json.Serialization;

namespace Clima.Contracts.Models
{
    /// <summary>
    /// Adafruit IO HTTP API
    /// Feeds - All Feeds - returns an array of feeds
    /// "https://io.adafruit.com/api/v2/{UserName}/feeds/
    /// Json deserializaiotn model
    /// 
    /// </summary>
    public class AdafruitIOFeed
    {

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
        
        [JsonPropertyName("key")]
        public string Key { get; set; }

        public AdafruitIOFeed()
        {
        }
    }
}






