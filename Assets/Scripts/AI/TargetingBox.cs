using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TargetingBox : MonoBehaviour
{
    private Vector2 selectedCoord = new(-1, 0);
    private Vector2 startCoord = new(-1, 0);
    private PlayerCombatAction ca;
    [SerializeField] private EnemyGrid EnemyGrid;

    public static event UnityAction<Vector2> OnNewCoordinateSelected;

    private void OnEnable()
    {
        UICombatAction.OnCombatActionSelected += SetCombatAction;
        EnemyGrid.DehighlightAllCells += ResetCoord;
    }

    private void OnDisable()
    {
        UICombatAction.OnCombatActionSelected -= SetCombatAction;
        EnemyGrid.DehighlightAllCells -= ResetCoord;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other != null && ca != null && other.CompareTag("TargetPoint"))
        {
            var newCoord = other.GetComponent<TargetPoint>().GetPointCoordinate();
            if (selectedCoord == startCoord)
            {
                //Debug.Log("New Collision!");
                selectedCoord = newCoord;
                OnNewCoordinateSelected?.Invoke(selectedCoord);
            }
            if (selectedCoord != newCoord)
            {
                switch (ca.TargetingType)
                {
                    case PlayerCombatAction.TargetType.Single:
                        TargetSingle(newCoord);
                        break;

                    case PlayerCombatAction.TargetType.Rows:
                        TargetRows(newCoord);
                        break;

                    case PlayerCombatAction.TargetType.Columns:
                        TargetColumns(newCoord);
                        break;

                    case PlayerCombatAction.TargetType.Area:
                        TargetArea(newCoord);
                        break;
                }
            }
        }
    }

    //Determine if coordinates match requirements of targeting specifications
    //if they do invoke the onCoordinateSelected event and pass in the new coordinate
    void TargetSingle(Vector2 newCoord)
    {
        //Check if both points are even
        bool xEven = OddOrEven(newCoord.x);
        bool yEven = OddOrEven(newCoord.y);

        if(xEven && yEven)
        {
            Debug.Log("New Collision!");
            OnNewCoordinateSelected?.Invoke(newCoord);
            selectedCoord = newCoord;
        }
    }

    private void TargetRows(Vector2 newCoord)
    {
        int maxRows = ca.RowsToTarget;
        if(EnemyGrid.EnemyEncounter.Rows < maxRows) maxRows = EnemyGrid.EnemyEncounter.Rows;

        bool rowsEven = CombatActionTargetingOddOrEven(maxRows);
        bool xEven = OddOrEven(newCoord.x);
        bool yEven = OddOrEven(newCoord.y);

        if (rowsEven && !yEven)
        {
            if(newCoord.y != selectedCoord.y)
            {
                //Debug.Log("New Collision!");
                OnNewCoordinateSelected?.Invoke(newCoord);
                selectedCoord = newCoord;
            }
        }
        else if(yEven)
        {
            if (newCoord.y != selectedCoord.y)
            {
                //Debug.Log("New Collision!");
                OnNewCoordinateSelected?.Invoke(newCoord);
                selectedCoord = newCoord;
            }
        }
        
    }

    private void TargetColumns(Vector2 newCoord)
    {
        int maxColumns = ca.ColumnsToTarget;
        if (EnemyGrid.EnemyEncounter.Columns < maxColumns) maxColumns = EnemyGrid.EnemyEncounter.Columns;

        bool columnsEven = CombatActionTargetingOddOrEven(maxColumns);
        bool xEven = OddOrEven(newCoord.x);
        bool yEven = OddOrEven(newCoord.y);

        if (columnsEven && !xEven)
        {
            if (newCoord.x != selectedCoord.x)
            {
                //Debug.Log("New Collision!");
                OnNewCoordinateSelected?.Invoke(newCoord);
                selectedCoord = newCoord;
            }
        }
        else if (xEven)
        {
            if (newCoord.x != selectedCoord.x)
            {
                //Debug.Log("New Collision!");
                OnNewCoordinateSelected?.Invoke(newCoord);
                selectedCoord = newCoord;
            }
        }
    }

    private void TargetArea(Vector2 newCoord)
    {
        int maxHeight = ca.AreaHeightToTarget;
        if (EnemyGrid.EnemyEncounter.Columns < maxHeight) maxHeight = EnemyGrid.EnemyEncounter.Columns;
        int maxWidth = ca.AreaWidthToTarget;
        if (EnemyGrid.EnemyEncounter.Rows < maxWidth) maxWidth = EnemyGrid.EnemyEncounter.Columns;

        bool widthEven = CombatActionTargetingOddOrEven(maxWidth);
        bool heightEven = CombatActionTargetingOddOrEven(maxHeight);
        bool xEven = OddOrEven(newCoord.x);
        bool yEven = OddOrEven(newCoord.y);

        if(!widthEven && !heightEven && xEven && yEven)
        {
            //Debug.Log("New Collision!");
            OnNewCoordinateSelected?.Invoke(newCoord);
            selectedCoord = newCoord;
        }
        else if(widthEven && heightEven && !xEven && !yEven)
        {
            //Debug.Log("New Collision!");
            OnNewCoordinateSelected?.Invoke(newCoord);
            selectedCoord = newCoord;
        }
        else if(widthEven && !heightEven && !xEven && yEven)
        {
            //Debug.Log("New Collision!");
            OnNewCoordinateSelected?.Invoke(newCoord);
            selectedCoord = newCoord;
        }
        else if(!widthEven && heightEven && xEven && !yEven)
        {
            //Debug.Log("New Collision!");
            OnNewCoordinateSelected?.Invoke(newCoord);
            selectedCoord = newCoord;
        }
    }

    void SetCombatAction(PlayerCombatAction combataction)
    {
        ca= combataction;
    }

    private bool OddOrEven(float newCoord)
    {

        if (newCoord == 0 || newCoord % 2 == 0)
            return true;
        else
            return false;
    }

    private bool CombatActionTargetingOddOrEven(int line)
    {
        if (line == 0 || line % 2 == 0) return true;
        else return false;
    }

    private void ResetCoord()
    {
        selectedCoord = startCoord;
    }
}
