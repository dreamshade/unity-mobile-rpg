// CharacterStats.cs
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct StatRank
{
    public StatType type;
    [Min(1)] public int rank;
}

[Serializable]
public struct StatPreview
{
    public StatType type;
    [SerializeField] public float value; // shows in Inspector; edits will be overwritten on validate
}

public class CharacterStats : MonoBehaviour
{
    [Header("Global Scaling Config")]
    [Tooltip("Drag in the StatConfig asset here")]
    public StatConfig statConfig;

    [Header("Character Level (applies to all stats)")]
    [Min(1)] public int CHAR_level = 1;

    [Header("Ranks (editable list)")]
    [SerializeField] private List<StatRank> ranks = new List<StatRank>();

    [Header("Preview (computed, read-only by design)")]
    [SerializeField] private List<StatPreview> previews = new List<StatPreview>();

    // ---------- RUNTIME CACHE ----------
    private int[] rankByType;

    // Convenience properties:
    public float STR => GetStat(StatType.STR);
    public float DEF => GetStat(StatType.DEF);
    public float VIT => GetStat(StatType.VIT);
    public float PTY => GetStat(StatType.PTY);
    public float INT => GetStat(StatType.INT);
    public float AGI => GetStat(StatType.AGI);

    // ---------- PUBLIC API ----------
    public float GetStat(StatType type)
    {
        EnsureCache();
        if (statConfig == null)   // guard
            return 0f;

        int rank = rankByType[(int)type];
        return StatCalculator.CalculateStat(rank, CHAR_level, statConfig);
    }

    public int GetRank(StatType type)
    {
        EnsureCache();
        return rankByType[(int)type];
    }

    public void SetRank(StatType type, int value)
    {
        int maxRank = statConfig != null ? Mathf.Max(1, statConfig.maxRank) : 100;
        value = Mathf.Clamp(value, 1, maxRank);

        int idx = ranks.FindIndex(s => s.type == type);
        if (idx >= 0)
        {
            var sr = ranks[idx];
            sr.rank = value;
            ranks[idx] = sr;
        }
        else
        {
            ranks.Add(new StatRank { type = type, rank = value });
        }

        RebuildCache();
        UpdatePreviews();
    }

    // ---------- UNITY LIFECYCLE ----------
    private void Awake()
    {
        RebuildCache();
        if (statConfig != null)   // guard
            UpdatePreviews();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Keep safe & tidy in the editor
        if (statConfig != null)
            CHAR_level = Mathf.Clamp(CHAR_level, 1, Mathf.Max(1, statConfig.maxLevel));
        else
            CHAR_level = Mathf.Max(1, CHAR_level);

        EnsureAllStatsPresent();
        ClampAllRanksToConfig();
        SortListByEnumOrder();

        RebuildCache();
        UpdatePreviews();
    }
#endif

    // ---------- INTERNAL HELPERS ----------
    private void EnsureCache()
    {
        int n = EnumCount<StatType>();
        if (rankByType == null || rankByType.Length != n)
            RebuildCache();
    }

    private void RebuildCache()
    {
        int n = EnumCount<StatType>();
        if (rankByType == null || rankByType.Length != n)
            rankByType = new int[n];

        for (int i = 0; i < n; i++) rankByType[i] = 1;

        foreach (var sr in ranks)
        {
            int i = (int)sr.type;
            if (i >= 0 && i < n)
                rankByType[i] = Mathf.Max(1, sr.rank);
        }
    }

    private void UpdatePreviews()
    {
        if (statConfig == null) return;  // guard

        EnsurePreviewListPresent();
        for (int i = 0; i < previews.Count; i++)
        {
            var p = previews[i];
            p.value = GetStat(p.type);
            previews[i] = p;
        }
    }

    private void EnsureAllStatsPresent()
    {
        int n = EnumCount<StatType>();
        for (int i = 0; i < n; i++)
        {
            var t = (StatType)i;
            if (!ranks.Exists(s => s.type == t))
                ranks.Add(new StatRank { type = t, rank = 1 });
        }
    }

    private void EnsurePreviewListPresent()
    {
        int n = EnumCount<StatType>();

        // ensure one entry for each StatType
        for (int i = 0; i < n; i++)
        {
            var t = (StatType)i;
            if (!previews.Exists(p => p.type == t))
                previews.Add(new StatPreview { type = t, value = 0f });
        }

        // sort to match enum order
        previews.Sort((a, b) => ((int)a.type).CompareTo((int)b.type));
    }

    private void ClampAllRanksToConfig()
    {
        int maxRank = statConfig != null ? Mathf.Max(1, statConfig.maxRank) : 100;
        for (int i = 0; i < ranks.Count; i++)
        {
            var sr = ranks[i];
            sr.rank = Mathf.Clamp(sr.rank, 1, maxRank);
            ranks[i] = sr;
        }
    }

    private void SortListByEnumOrder()
    {
        ranks.Sort((a, b) => ((int)a.type).CompareTo((int)b.type));
    }

    private static int EnumCount<T>() where T : Enum => Enum.GetValues(typeof(T)).Length;

    [ContextMenu("Recalculate Previews")]
    public void RecalculatePreviewsContext()
    {
        RebuildCache();
        UpdatePreviews();
    }
}
