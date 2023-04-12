using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Combat Action", menuName = "New Player Combat Action")]
public class PlayerCombatAction : CombatAction
{
    public enum TargetType
    {
        Self,
        Players,
        Single,
        Columns,
        Rows,
        Area,
        All
    }

    
    public TargetType TargetingType;

    [Header("Is this ability random targeted?")]
    public bool RandomTargets;

    //Random?
    [Header("If random targeted, Number of Random Targets")]
    public int RandomTargetsNumber;

    //Multiple selections allowed for 1 ability?
    [Header("Are multiple selections allowed?")]
    public bool MultiSelect;

    [Header("If multiSelect allowed, how many selections")]
    public int NumberToSelect;

    //If target type Area
    [Header("If target type is Area, Targets by Width")]
    public int AreaWidthToTarget;

    [Header("If target type is Area, Targets by Height")]
    public int AreaHeightToTarget;

    //If target type Rows
    [Header("If target type is Rows, Number of Rows to Target")]
    public int RowsToTarget;

    //If target type Columns
    [Header("If target type is Columns, Number of Rows to Target")]
    public int ColumnsToTarget;
}
