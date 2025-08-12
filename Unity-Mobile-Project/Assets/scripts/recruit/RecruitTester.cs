// RecruitTester.cs
using System;
using UnityEngine;

public class RecruitTester : MonoBehaviour
{
    public RecruitConfig recruitConfig;
    public StatConfig statConfig;

    [Range(1, 10)] public int generateCount = 3;

    // cache the enum list once
    private static readonly StatType[] AllTypes = (StatType[])Enum.GetValues(typeof(StatType));

    [ContextMenu("Generate Recruits")]
    public void GenerateRecruits()
    {
        if (recruitConfig == null || statConfig == null)
        {
            Debug.LogError("[RecruitTester] Assign RecruitConfig and StatConfig.");
            return;
        }

        for (int i = 0; i < generateCount; i++)
        {
            var go = new GameObject($"Recruit_{i + 1}");
            var cs = RecruitGenerator.GenerateCharacter(go, recruitConfig, statConfig);
            if (cs == null)
            {
                Debug.LogError("[RecruitTester] Failed to generate CharacterStats.");
                continue;
            }

            // Build a readable line: TOTAL + per-stat ranks and computed values
            int totalRank = 0;
            string ranksStr = "";
            string valsStr  = "";

            foreach (var t in AllTypes)
            {
                int r = cs.GetRank(t);
                float v = cs.GetStat(t);
                totalRank += r;

                ranksStr += $"{t}={r}, ";
                valsStr  += $"{t}:{v:0.##}, ";
            }

            // trim trailing comma+space
            if (ranksStr.Length > 2) ranksStr = ranksStr.Substring(0, ranksStr.Length - 2);
            if (valsStr.Length  > 2) valsStr  = valsStr.Substring(0,  valsStr.Length  - 2);

            Debug.Log($"[{go.name}] LVL={cs.CHAR_level} | TOTAL_RANK={totalRank} | Ranks({ranksStr}) | Values({valsStr})");
        }
    }
}
