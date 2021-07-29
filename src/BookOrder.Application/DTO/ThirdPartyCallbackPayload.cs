using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace BookOrder.Application.DTO
{
    public class ThirdPartyCallbackPayload
    {
        [JsonPropertyName("third_party_callback_command")]
        public CommandType? Command { get; set; }
        [JsonPropertyName("callback_key")]
        public string BookKey { get; set; }
        public enum CommandType
        {
            CancelOrder,
            DoOtherStuff
        }
    }
}
