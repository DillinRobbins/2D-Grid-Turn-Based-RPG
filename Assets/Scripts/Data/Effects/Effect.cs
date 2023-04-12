using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "new Effect")]
public class Effect : ScriptableObject
{
    public enum Type
    {
        SpeedDown,
        AttackDown,
        ManaRegen,
        SpellUp
    }

    public bool isBuff;

    public Type effectType;
    public bool hasTurnLimit;
    public int durationOfTurns;
    
    public bool isStackable;
    public bool applyOnSameTurn;
    public bool applyOnDestroy = false;

    public Sprite icon;

    [Header("Prefabs")]
    public GameObject activePrefab;
    public GameObject tickPrefab;

    public int speedMod;

    public float spellMod;
    public float damageMod;

    public float manaRegen;
}
