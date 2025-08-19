// RecruitGenerator.cs
using UnityEngine;
using System;
using System.Linq;
using Dreamshade.Characters;
using Dreamshade.Items;

public static class RecruitGenerator
{
    // Dynamically get all stats from the enum - no more manual listing!
    public static StatType[] AllStats => Enum.GetValues(typeof(StatType)).Cast<StatType>().ToArray();

    public static CharacterStats GenerateCharacter(GameObject host, RecruitConfig cfg, StatConfig statCfg)
    {
        if (cfg == null || statCfg == null)
        {
            Debug.LogError("[RecruitGenerator] Missing cfg/statCfg.");
            return null;
        }

        // Ensure we have a CharacterStats component to fill
        var stats = host.GetComponent<CharacterStats>();
        if (stats == null) stats = host.AddComponent<CharacterStats>();
        stats.statConfig = statCfg;

        // 1) choose total points with a skew toward low totals
        int total = RollTotalPoints(cfg);

        // 2) allocate across all stats with anti-dominance + rare spikes
        int[] ranks = AllocateWithAntiDominance(total, cfg);

        // 3) apply to CharacterStats (levels start at cfg.startingLevel)
        stats.CHAR_level = cfg.startingLevel;
        
        // Iterate through all stat types dynamically
        var statTypes = AllStats;
        for (int i = 0; i < statTypes.Length && i < ranks.Length; i++)
        {
            stats.SetRank(statTypes[i], ranks[i]);
        }
        
        stats.RecalculatePreviewsContext();

        return stats;
    }

    static int RollTotalPoints(RecruitConfig cfg)
    {
        // Sample a [0,1) value, skew it, then map to [min,max]
        // Skew > 1 biases toward lower values (u^k shrinks large u).
        float u = UnityEngine.Random.value;
        float skewed = Mathf.Pow(u, cfg.totalPointsSkew);
        float t = Mathf.Clamp01(skewed);
        return Mathf.RoundToInt(Mathf.Lerp(cfg.minTotalRankPoints, cfg.maxTotalRankPoints, t));
    }

    static int[] AllocateWithAntiDominance(int totalPoints, RecruitConfig cfg)
    {
        int n = AllStats.Length; // Dynamically get the number of stats
        int[] vals = new int[n]; // start at 0 each

        int spikeRemaining = 0;

        for (int p = 0; p < totalPoints; p++)
        {
            // maybe begin a spike
            if (spikeRemaining <= 0 && UnityEngine.Random.value < cfg.spikeStartChance)
            {
                spikeRemaining = UnityEngine.Random.Range(cfg.spikeLengthRange.x, cfg.spikeLengthRange.y + 1);
            }

            int pick;
            if (spikeRemaining > 0)
            {
                // favor the already-high stats: weight = (vals[i]+1)^spikeAlpha
                pick = WeightedPick(vals, v => Mathf.Pow(v + 1f, cfg.spikeAlpha));
                spikeRemaining--;
            }
            else
            {
                // anti-dominance: weight inversely related to how far above the min (or mean) a stat is
                int baseline = cfg.compareToCurrentMin ? Min(vals) : Mathf.RoundToInt(Mean(vals));
                pick = WeightedPick(vals, v =>
                {
                    float delta = (v - baseline);
                    // when v == baseline, weight = 1; higher v lowers the weight
                    return 1f / Mathf.Pow(1f + Mathf.Max(0f, delta), cfg.antiDominanceAlpha);
                });
            }

            vals[pick]++;
        }

        return vals;
    }

    static int WeightedPick(int[] vals, Func<int, float> weightFunc)
    {
        // compute weights
        float sum = 0f;
        float[] w = new float[vals.Length];
        for (int i = 0; i < vals.Length; i++)
        {
            w[i] = Mathf.Max(0.0001f, weightFunc(vals[i])); // avoid zero
            sum += w[i];
        }

        // roulette wheel
        float r = UnityEngine.Random.value * sum;
        float acc = 0f;
        for (int i = 0; i < vals.Length; i++)
        {
            acc += w[i];
            if (r <= acc) return i;
        }
        return vals.Length - 1; // fallback
    }

    static int Min(int[] a)
    {
        int m = int.MaxValue;
        for (int i = 0; i < a.Length; i++) if (a[i] < m) m = a[i];
        return m;
    }

    static float Mean(int[] a)
    {
        int s = 0;
        for (int i = 0; i < a.Length; i++) s += a[i];
        return (float)s / a.Length;
    }

    // Easy wrapper for beginners: roll a fresh ranks array from a RecruitConfig.
    public static int[] GenerateRanks(RecruitConfig cfg)
    {
        if (cfg == null)
        {
            Debug.LogError("[RecruitGenerator] GenerateRanks called with null cfg.");
            return null;
        }

        // 1) Decide how many total points we get to spend
        int total = RollTotalPoints(cfg);

        // 2) Distribute those points across all stats dynamically
        //    The order follows the enum order in StatType
        int[] ranks = AllocateWithAntiDominance(total, cfg);

        return ranks;
    }
}