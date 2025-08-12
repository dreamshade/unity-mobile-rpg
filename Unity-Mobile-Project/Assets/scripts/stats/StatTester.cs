// StatTester.cs
using UnityEngine;

public class StatTester : MonoBehaviour
{
    [Tooltip("Drag in your StatConfig asset here")]
    public StatConfig config;

    [Header("Quick Debug Inputs")]
    public int testRank  = 1;
    public int testLevel = 1;

    void Start()
    {
        if (config == null)
        {
            Debug.LogWarning("[StatTester] No StatConfig assigned; skipping tests.");
            return;
        }

        Debug.Log($"[StatTester] stat(1,1)        = {StatCalculator.CalculateStat(1, 1,  config)}   (should be {config.statAtRank1Level1})");
        Debug.Log($"[StatTester] stat(max,1)      = {StatCalculator.CalculateStat(config.maxRank, 1,  config)}   (should be {config.statAtMaxRankLevel1})");
        Debug.Log($"[StatTester] stat(1,max)      = {StatCalculator.CalculateStat(1, config.maxLevel,  config)}   (should be {config.statAtRank1MaxLevel})");
        Debug.Log($"[StatTester] stat(max,max)    = {StatCalculator.CalculateStat(config.maxRank, config.maxLevel, config)}   (should be {config.statAtMaxRankMaxLevel})");
        Debug.Log($"[StatTester] stat({testRank},{testLevel}) = {StatCalculator.CalculateStat(testRank, testLevel, config)}");
    }
}
