using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CombatAction : ScriptableObject
{
    public enum Type
    {
        Attack,
        Heal,
        Spell,
        Defense,
        Totem
    }

    public enum Element
    {
        None,
        Radiant,
        Arcane,
        Calamity,
        Fire,
        Ice,
        Electric
    }

    public string DisplayName;
    public Sprite icon;
    public Type ActionType;
    public GameObject projectilePrefab;
    public Effect effect;
    public GameObject totem;

    public float CombatActionChance;

    public int Damage;
    public float DefenseAmount;
    public int HealAmount;
    public int ManaCost, SpellDamage;
    public int DamageReduction;
    public int SpeedModifier;
}
