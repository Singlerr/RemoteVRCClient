using Newtonsoft.Json;

namespace VRCTower
{
    public class ActionPacket : DataPacket
    {
        [JsonProperty("action")] private string _action;

        [JsonProperty("params")] private string[] _args;

        public ActionPacket(string id, string password, string userId, string action = "",
            string[] args = null) : base(
            id, password, userId)
        {
            _action = action;
            _args = args;
        }
    }
}