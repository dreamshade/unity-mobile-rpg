// Assets/Scripts/SaveSystem/Data/RecruitData.cs
using System;
using System.Collections.Generic;

namespace Dreamshade.SaveSystem.Data
{
    [Serializable]
    public class RecruitData
    {
        // Identity & progression
        public string name;   // e.g., "Aerin"
        public string job;    // e.g., "Warrior" (RecruitClass as string)
        public int level;     // >= 1

        // Stat ranks saved by name -> rank for forward compatibility
        [Serializable]
        public class StatRank
        {
            public string stat; // e.g., "STR", "DEF" ...
            public int rank;    // >= 1
        }
        public List<StatRank> ranks = new List<StatRank>();

        // Equipment slots + currently equipped item by ID
        [Serializable]
        public class EquipmentSlotData
        {
            public string slot;           // e.g., "Weapon", "Armor"
            public string equippedItemId; // null/empty if nothing equipped
        }
        public List<EquipmentSlotData> equipment = new List<EquipmentSlotData>();
    }
}
