// StatConfig.cs
using UnityEngine;

[CreateAssetMenu(fileName = "StatConfig", menuName = "Game/StatConfig")]
public class StatConfig : ScriptableObject
{
    [Header("Scaling Ranges")]
    [Tooltip("Highest possible rank")]
    public int maxRank = 100;

    [Tooltip("Highest possible level")]
    public int maxLevel = 50;

    [Header("Boundary Values")]
    [Tooltip("Stat at rank=1, level=1")]
    public float statAtRank1Level1 = 10f;

    [Tooltip("Stat at rank=1, level=maxLevel")]
    public float statAtRank1MaxLevel = 114f;

    [Tooltip("Stat at rank=maxRank, level=1")]
    public float statAtMaxRankLevel1 = 15f;

    [Tooltip("Stat at rank=maxRank, level=maxLevel")]
    public float statAtMaxRankMaxLevel = 150f;
}
