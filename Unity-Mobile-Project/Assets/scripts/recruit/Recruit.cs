// Assets/scripts/recruit/Recruit.cs
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Dreamshade.Characters;
using Dreamshade.Items;

// ScriptableObject for recruit data that can be created as assets
// Create via: Assets > Create > Game Data > Recruit Template

[CreateAssetMenu(fileName = "NewRecruit", menuName = "Recruits/Recruit Template", order = 1)]
[Serializable]
public sealed class Recruit : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string recruitName = "New Recruit";
    [SerializeField] private RecruitClass job;

    [Header("Progression")]
    [SerializeField, Min(1)] private int level = 1;

    // Store ranks in a dictionary-like structure that auto-populates from enum
    [Header("Stats")]
    [SerializeField] private List<StatRankEntry> statRanks = new List<StatRankEntry>();

    [Serializable]
    public class StatRankEntry
    {
        public StatType statType;
        [Min(1)] public int rank = 1;

        public StatRankEntry(StatType type, int rankValue = 1)
        {
            statType = type;
            rank = Mathf.Max(1, rankValue);
        }
    }

    [Serializable]
    public struct EquipmentSlotEntry
    {
        public EquipmentSlot slot;
        public EquipmentDef equippedItem;
    }

    [Header("Equipment")]
    [SerializeField] private List<EquipmentSlotEntry> equipmentSlots = new List<EquipmentSlotEntry>();

    // Properties for external access
    public string RecruitName => recruitName;
    public RecruitClass Job => job;
    public int Level => level;
    public IReadOnlyList<EquipmentSlotEntry> EquipmentSlots => equipmentSlots;

    // Initialize stat ranks list with all enum values if needed
    private void EnsureStatRanksInitialized()
    {
        var allStatTypes = Enum.GetValues(typeof(StatType)).Cast<StatType>().ToList();
        
        // Remove any duplicates or invalid entries
        var validRanks = new List<StatRankEntry>();
        foreach (var statType in allStatTypes)
        {
            var existing = statRanks.FirstOrDefault(r => r.statType == statType);
            if (existing != null)
            {
                validRanks.Add(existing);
            }
            else
            {
                validRanks.Add(new StatRankEntry(statType, 1));
            }
        }
        
        statRanks = validRanks;
    }

    // Public methods for accessing/modifying stats
    public int GetRank(StatType type)
    {
        EnsureStatRanksInitialized();
        var entry = statRanks.FirstOrDefault(r => r.statType == type);
        return entry != null ? entry.rank : 1;
    }
    
    public void SetRank(StatType type, int value)
    {
        EnsureStatRanksInitialized();
        value = Mathf.Max(1, value);
        
        var entry = statRanks.FirstOrDefault(r => r.statType == type);
        if (entry != null)
        {
            entry.rank = value;
        }
        else
        {
            statRanks.Add(new StatRankEntry(type, value));
        }
        
        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }

    // Initialize this recruit (used for runtime creation, not for preset assets)
    public void Initialize(string name, RecruitClass recruitClass, int recruitLevel, int[] ranks, IEnumerable<EquipmentSlot> slots)
    {
        recruitName = string.IsNullOrWhiteSpace(name) ? "Recruit" : name.Trim();
        job = recruitClass;
        level = Mathf.Max(1, recruitLevel);
        
        // Initialize ranks from array using enum order
        if (ranks != null)
        {
            var statTypes = Enum.GetValues(typeof(StatType)).Cast<StatType>().ToArray();
            statRanks.Clear();
            
            for (int i = 0; i < statTypes.Length && i < ranks.Length; i++)
            {
                statRanks.Add(new StatRankEntry(statTypes[i], ranks[i]));
            }
        }

        equipmentSlots.Clear();
        if (slots != null)
        {
            foreach (var s in slots)
                equipmentSlots.Add(new EquipmentSlotEntry { slot = s, equippedItem = null });
        }

        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }

    // Apply this recruit's data onto a CharacterStats component
    public CharacterStats ApplyTo(GameObject host, StatConfig statCfg)
    {
        if (host == null) return null;
        var stats = host.GetComponent<CharacterStats>();
        if (stats == null) stats = host.AddComponent<CharacterStats>();

        stats.statConfig = statCfg;
        stats.CHAR_level = level;

        // Iterate through all stat types instead of manually listing them
        foreach (StatType statType in Enum.GetValues(typeof(StatType)))
        {
            stats.SetRank(statType, GetRank(statType));
        }

        stats.RecalculatePreviewsContext();
        return stats;
    }

    // Create a runtime instance (copy) of this recruit template
    public Recruit CreateInstance()
    {
        var instance = CreateInstance<Recruit>();
        instance.recruitName = recruitName;
        instance.job = job;
        instance.level = level;
        instance.statRanks = new List<StatRankEntry>(statRanks);
        instance.equipmentSlots = new List<EquipmentSlotEntry>(equipmentSlots);
        return instance;
    }

    // Convert ranks to array in enum order (for compatibility with existing code)
    public int[] GetRanksAsArray()
    {
        var statTypes = Enum.GetValues(typeof(StatType)).Cast<StatType>().ToArray();
        var ranks = new int[statTypes.Length];
        
        for (int i = 0; i < statTypes.Length; i++)
        {
            ranks[i] = GetRank(statTypes[i]);
        }
        
        return ranks;
    }

    // Static factory method for creating recruits at runtime with procedural generation
    public static Recruit CreateRuntimeRecruit(
        string recruitName,
        RecruitClass job,
        int level,
        RecruitConfig config,
        IEnumerable<EquipmentSlot> equipmentSlots)
    {
        if (config == null)
        {
            Debug.LogError("[Recruit.CreateRuntimeRecruit] config is null. Please pass a valid RecruitConfig.");
            return null;
        }

        // Roll random stats
        int[] rolledRanks = RecruitGenerator.GenerateRanks(config);
        if (rolledRanks == null)
        {
            Debug.LogError("[Recruit.CreateRuntimeRecruit] Failed to roll ranks.");
            return null;
        }

        // Create a new ScriptableObject instance at runtime
        var recruit = CreateInstance<Recruit>();
        recruit.Initialize(recruitName, job, level, rolledRanks, equipmentSlots);
        return recruit;
    }

    // Equip an item to a specific slot
    public bool EquipItem(EquipmentSlot slot, EquipmentDef item)
    {
        for (int i = 0; i < equipmentSlots.Count; i++)
        {
            if (equipmentSlots[i].slot == slot)
            {
                var entry = equipmentSlots[i];
                entry.equippedItem = item;
                equipmentSlots[i] = entry;
                #if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
                #endif
                return true;
            }
        }
        return false;
    }

    // Get equipped item in a specific slot
    public EquipmentDef GetEquippedItem(EquipmentSlot slot)
    {
        foreach (var entry in equipmentSlots)
        {
            if (entry.slot == slot)
                return entry.equippedItem;
        }
        return null;
    }

    // Clear all equipment
    public void ClearAllEquipment()
    {
        for (int i = 0; i < equipmentSlots.Count; i++)
        {
            var entry = equipmentSlots[i];
            entry.equippedItem = null;
            equipmentSlots[i] = entry;
        }
        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }

    // Validation in editor
    private void OnValidate()
    {
        level = Mathf.Max(1, level);
        if (string.IsNullOrWhiteSpace(recruitName))
            recruitName = "Recruit";
        
        EnsureStatRanksInitialized();
    }
}