using System;
using System.Collections.Generic;
namespace Dreamshade.SaveSystem.Data
{
    [Serializable]
        public class SaveData
    {
        public string playerId; // The id we loaded/saved under
        public int version = 1; // For future migrations
        public List<RecruitData> roster = new List<RecruitData>();
    }
}