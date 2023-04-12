using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Horde Encounter", menuName = "New Horde Encounter")]
public class Encounter : ScriptableObject
{
    public enum OrientationType
    {
        Random,
        Manual
    }

    public OrientationType Orientation;
    public CharacterType[] CharacterTypes;

    [Header("Number of Rows in the Grid (max 9)")]
    public int Rows;

    [Header("Number of Columns in the Grid (max 5)")]
    public int Columns;

    [Header("Number of Enemy 1")]
    public int Enemy1Number;

    [Header("Number of Enemy 2")]
    public int Enemy2Number;

    [Header("Number of Enemy 3")]
    public int Enemy3Number;

    [Header("Number of Enemy 4")]
    public int Enemy4Number;

    [Header("Number of Enemy 5")]
    public int Enemy5Number;
}
