// Assets/scripts/recruit/Recruit.cs
using System;
using System.Collections.Generic;
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

    // Ranks per StatType - using a custom serializable class for better Inspector display
    [Header("Stats")]
    [SerializeField] private StatRanks statRanks = new StatRanks();

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

    // Serializable wrapper for stat ranks with better Inspector display
    [Serializable]
    public class StatRanks
    {
        [Header("Base Stats")]
        [SerializeField, Min(1)] private int strength = 1;
        [SerializeField, Min(1)] private int defense = 1;
        [SerializeField, Min(1)] private int vitality = 1;
        [SerializeField, Min(1)] private int piety = 1;
        [SerializeField, Min(1)] private int intelligence = 1;
        [SerializeField, Min(1)] private int agility = 1;

        public int GetRank(StatType type)
        {
            return type switch
            {
                StatType.STR => strength,
                StatType.DEF => defense,
                StatType.VIT => vitality,
                StatType.PTY => piety,
                StatType.INT => intelligence,
                StatType.AGI => agility,
                _ => 1
            };
        }

        public void SetRank(StatType type, int value)
        {
            value = Mathf.Max(1, value);
            switch (type)
            {
                case StatType.STR: strength = value; break;
                case StatType.DEF: defense = value; break;
                case StatType.VIT: vitality = value; break;
                case StatType.PTY: piety = value; break;
                case StatType.INT: intelligence = value; break;
                case StatType.AGI: agility = value; break;
            }
        }

        // Initialize from array (for procedural generation)
        public void InitializeFromArray(int[] ranks)
        {
            if (ranks == null || ranks.Length < 6) return;
            strength = Mathf.Max(1, ranks[0]);
            defense = Mathf.Max(1, ranks[1]);
            vitality = Mathf.Max(1, ranks[2]);
            piety = Mathf.Max(1, ranks[3]);
            intelligence = Mathf.Max(1, ranks[4]);
            agility = Mathf.Max(1, ranks[5]);
        }

        // Export to array
        public int[] ToArray()
        {
            return new int[] { strength, defense, vitality, piety, intelligence, agility };
        }
    }

    // Public methods for accessing/modifying stats
    public int GetRank(StatType type) => statRanks.GetRank(type);
    
    public void SetRank(StatType type, int value)
    {
        statRanks.SetRank(type, value);
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
        
        if (ranks != null && ranks.Length >= 6)
        {
            statRanks.InitializeFromArray(ranks);
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

        stats.SetRank(StatType.STR, GetRank(StatType.STR));
        stats.SetRank(StatType.DEF, GetRank(StatType.DEF));
        stats.SetRank(StatType.VIT, GetRank(StatType.VIT));
        stats.SetRank(StatType.PTY, GetRank(StatType.PTY));
        stats.SetRank(StatType.INT, GetRank(StatType.INT));
        stats.SetRank(StatType.AGI, GetRank(StatType.AGI));

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
        instance.statRanks = new StatRanks();
        instance.statRanks.InitializeFromArray(statRanks.ToArray());
        instance.equipmentSlots = new List<EquipmentSlotEntry>(equipmentSlots);
        return instance;
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
    }
}