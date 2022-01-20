using Newtonsoft.Json;

namespace VRCTower
{
    public class DataPacket
    {
        [JsonProperty("id")] private string _id;

        [JsonProperty("password")] private string _password;

        [JsonProperty("userId")] private string _userId;

        public DataPacket(string id, string password, string userId)
        {
            _id = id;
            _password = password;
            _userId = userId;
        }
    }
}