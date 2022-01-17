using Newtonsoft.Json;

namespace VRCTower
{
    public class DataPacket
    {
        [JsonProperty("apiKey")] private string _apiKey;

        [JsonProperty("authCookie")] private string _authCookie;

        [JsonProperty("userId")] private string _userId;

        public DataPacket(string apiKey, string authCookie, string userId)
        {
            _apiKey = apiKey;
            _authCookie = authCookie;
            _userId = userId;
        }
    }
}