// StatCalculator.cs
using UnityEngine;

public static class StatCalculator
{
    /// <summary>
    /// Returns the interpolated stat for a given rank & level.
    /// </summary>
    public static float CalculateStat(int rank, int level, StatConfig cfg)
    {
        // clamp inputs
        rank  = Mathf.Clamp(rank,  1, cfg.maxRank);
        level = Mathf.Clamp(level, 1, cfg.maxLevel);

        // tL = 0 at level=1, 1 at level=maxLevel
        float tL = (level - 1f) / (cfg.maxLevel - 1f);

        // for any level, what's the stat at rank=1?
        float minStat = Mathf.Lerp(cfg.statAtRank1Level1,
                                   cfg.statAtRank1MaxLevel,
                                   tL);
        // for any level, what's the stat at maxRank?
        float maxStat = Mathf.Lerp(cfg.statAtMaxRankLevel1,
                                   cfg.statAtMaxRankMaxLevel,
                                   tL);

        // tR = 0 at rank=1, 1 at rank=maxRank
        float tR = (rank - 1f) / (cfg.maxRank - 1f);

        // final stat
        return Mathf.Lerp(minStat, maxStat, tR);
    }
}
