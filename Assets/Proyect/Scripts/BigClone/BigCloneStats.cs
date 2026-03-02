using UnityEngine;

public class BigCloneStats
{
    public float SizeMultiplier;
    public float SpeedMultiplier;
    public float JumpMultiplier;
    public int InitialEnergyCost;
    public int EnergyDrainPerSecond;

    public BigCloneStats()
    {
        SizeMultiplier = 1.75f;
        SpeedMultiplier = 0.7f;
        JumpMultiplier = 0.7f;
        InitialEnergyCost = 0;
        EnergyDrainPerSecond = 10;
    }
}
