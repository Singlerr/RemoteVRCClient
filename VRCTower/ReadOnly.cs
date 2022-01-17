using Newtonsoft.Json;

namespace VRCTower
{
    public class ReadOnly
    {
        [JsonProperty("message")] public string Message;

        [JsonProperty("statusCode")] public int StatusCode;

        public ReadOnly(string message, int statusCode)
        {
            Message = message;
            StatusCode = statusCode;
        }
    }
}