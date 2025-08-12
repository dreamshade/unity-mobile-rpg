// RecruitConfig.cs
using UnityEngine;

[CreateAssetMenu(fileName = "RecruitConfig", menuName = "Game/Recruit Config")]
public class RecruitConfig : ScriptableObject
{
    [Header("Totals")]
    [Tooltip("Min and max total rank points to distribute across all 6 stats.")]
    public int minTotalRankPoints = 100;
    public int maxTotalRankPoints = 400;

    [Tooltip("Skews the total toward lower values. >1 = fewer high totals. 1 = uniform.")]
    public float totalPointsSkew = 2.0f;

    [Header("Anti-Dominance")]
    [Tooltip("How strongly we avoid piling points on already-high stats. 0 = none, 1..2 = moderate/strong.")]
    public float antiDominanceAlpha = 1.25f;

    [Tooltip("We measure 'how high' a stat is relative to the CURRENT MIN value among all stats.")]
    public bool compareToCurrentMin = true;

    [Header("Rare Spikes (to allow super-stats, but rarely)")]
    [Tooltip("Chance per allocation to start a brief 'spike' that favors already-high stats.")]
    public float spikeStartChance = 0.01f;   // 1%
    [Tooltip("How many points a spike lasts, inclusive.")]
    public Vector2Int spikeLengthRange = new Vector2Int(2, 6);
    [Tooltip("When spiking, weight ~ (value+1)^spikeAlpha (>=1 makes highs get higher).")]
    public float spikeAlpha = 2.0f;

    [Header("Levels")]
    [Tooltip("Initial level for fresh recruits.")]
    public int startingLevel = 1;
}
