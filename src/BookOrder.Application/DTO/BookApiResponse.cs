using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace BookOrder.Application.DTO
{
    public class BookApiResponse
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("authors")]
        public List<AuthorPayload> AuthorPayloads { get; set; } = new List<AuthorPayload>();
        [JsonPropertyName("covers")]
        public List<int> Covers { get; set; } = new List<int>();
        [JsonIgnore]
        public List<AuthorInfo> Authors { get; set; } = new List<AuthorInfo>();
        
        public class AuthorPayload
        {
            [JsonPropertyName("author")]
            public KeyPayload AuthorKey { get; set; }
        }
        public class KeyPayload
        {
            [JsonPropertyName("key")]
            public string Key { get; set; }
        }
        public class AuthorInfo
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }
        }
    }
}
