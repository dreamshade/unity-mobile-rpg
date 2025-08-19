// RecruitTester.cs
using System;
using System.Linq;
using UnityEngine;

public class RecruitTester : MonoBehaviour
{
    public RecruitConfig recruitConfig;
    public StatConfig statConfig;

    [Range(1, 10)] public int generateCount = 3;

    [ContextMenu("Generate Recruits")]
    public void GenerateRecruits()
    {
        if (recruitConfig == null || statConfig == null)
        {
            Debug.LogError("[RecruitTester] Assign RecruitConfig and StatConfig.");
            return;
        }

        // Get all stat types dynamically from the enum
        var allStatTypes = Enum.GetValues(typeof(StatType)).Cast<StatType>().ToArray();

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

            // Iterate through all stat types dynamically
            foreach (var statType in allStatTypes)
            {
                int r = cs.GetRank(statType);
                float v = cs.GetStat(statType);
                totalRank += r;

                ranksStr += $"{statType}={r}, ";
                valsStr  += $"{statType}:{v:0.##}, ";
            }

            // trim trailing comma+space
            if (ranksStr.Length > 2) ranksStr = ranksStr.Substring(0, ranksStr.Length - 2);
            if (valsStr.Length  > 2) valsStr  = valsStr.Substring(0,  valsStr.Length  - 2);

            Debug.Log($"[{go.name}] LVL={cs.CHAR_level} | TOTAL_RANK={totalRank} | Ranks({ranksStr}) | Values({valsStr})");
        }
    }
}