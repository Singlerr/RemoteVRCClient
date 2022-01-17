using Newtonsoft.Json;

namespace VRCTower
{
    public class ActionPacket : DataPacket
    {
        [JsonProperty("action")] private string _action;

        [JsonProperty("params")] private string[] _args;

        public ActionPacket(string apiKey, string authCookie, string userId, string action = "",
            string[] args = null) : base(
            apiKey, authCookie, userId)
        {
            _action = action;
            _args = args;
        }
    }
}