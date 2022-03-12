using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScriptsManager : MonoBehaviour
{
    public PlayerMovement PlayerMovement => GetComponent<PlayerMovement>();
    public PlayerItemManager PlayerItemManager => GetComponent<PlayerItemManager>();
    public PlayerTurret PlayerTurret => GetComponent<PlayerTurret>();
    public PlayerHealthManager PlayerHealthManager => GetComponent<PlayerHealthManager>();
    public HitMechanics PlayerHitMechanics => GetComponent<HitMechanics>();

    private Dictionary<EStatName, float> PlayerStats { get; set; } = new Dictionary<EStatName, float>();

    public bool DisablePlayer { get; set; } = false;

    private void Awake()
    {
        foreach(EStatName value in System.Enum.GetValues(typeof(EStatName)))
        {
            if (value == EStatName.none)
                return;
            PlayerStats.Add(value, 0);
        }
    }
    public float GetStat(EStatName statName)
    {
        foreach (KeyValuePair<EStatName, float> pair in PlayerStats)
        {
            if (pair.Key == statName)
            {
                return pair.Value;
            }
        }
        return -1;
    }
    public void SetStat(EStatName statName, float value)
    {
        foreach(KeyValuePair<EStatName, float> pair in PlayerStats)
        {
            if(pair.Key == statName)
            {
                PlayerStats[pair.Key] = value;
                return;
            }
        }
    }
}
