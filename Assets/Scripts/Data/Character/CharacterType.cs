using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

[CreateAssetMenu(fileName = "Character Type", menuName = "New Character Type")]
public class CharacterType : ScriptableObject
{
    public enum Type
    {
        PlayerTop,
        PlayerLeft,
        PlayerRight,
        PlayerBottom,
        Enemy
    }

    public enum Race
    {
        Enemy,
        Imp,
        Fae
    }

    public int enemyIndex;

    public string TypeDisplayName;
    public Type TypeofCharacter;
    public Race RaceofCharacter;
    public Sprite characterSprite;

    public int speed;

    [Header("Health")]
    public int HealthAmount;

    [Header("CombatActions")]
    public List<CombatAction> typeCombatActions;
}
