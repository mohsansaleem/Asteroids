using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PG.Asteroids.Models.DataModels
{
    [Serializable]
    public class UserData
    {
        [JsonProperty("TopScores")]
        public List<int> TopScores;

        public DateTime LastSaved;
    }
}