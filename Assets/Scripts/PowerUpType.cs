using UnityEngine;

public enum PowerUpType
{
    Shield,        // Invincibility
    SlowMotion,    // Slows down time
    DoublePoints,  // 2x score multiplier
    SpeedBoost     // Faster movement
}

[System.Serializable]
public class PowerUpData
{
    public PowerUpType type;
    public string powerUpName;
    public Sprite icon;
    public Color color = Color.white;
    public float duration = 5f;
    public float spawnChance = 1f; // Weight for random spawning

    [Header("Visual")]
    public GameObject particleEffect;

    [Header("Audio")]
    public string collectSoundName = "powerup";
    public string activateSoundName = "powerup_activate";
    public string expireSoundName = "powerup_expire";
}