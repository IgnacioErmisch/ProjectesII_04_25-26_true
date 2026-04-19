using UnityEngine;

public class SmallCloneStats
{
    public float SizeMultiplier; 
    public float SpeedMultiplier; 
    public float JumpMultiplier; 
    public int InitialEnergyCost; 
    public int EnergyDrainPerSecond; 

    public SmallCloneStats()
    {
        SizeMultiplier = 0.6f;
        SpeedMultiplier = 1.4f;
        JumpMultiplier = 1.6f;
        InitialEnergyCost = 10;
        EnergyDrainPerSecond = 5;
    }
}
