using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class EnemyGrid : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Encounter EnemyEncounter;
    private int HorizCellCount;
    private int VertCellCount;
    private float cellWidth;
    private float cellHeight;
    private bool boxFollowMouse = false;
    private int[] encounterEnemyTypeCount = new int[5];

    private bool gridSelectionAllowed = false;
    private int numberOfTargetSelections = 0;

    private int totalCells;
    private List<CharacterType> characterTypes = new();
    private List<int> totalEnemiesToSpawn = new();
    private List<Character> targetCharacters = new();

    [SerializeField] private Canvas canvas;
    private RectTransform rt;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private GameObject testPoint;
    [SerializeField] private GameObject targetBox;
    [SerializeField] private GameObject gridContainer;
    [SerializeField] private GameObject grid;
    [SerializeField] private Cell Cell;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;

    public event UnityAction OnCellsInstantiated;
    public static event UnityAction DehighlightAllCells;
    public static event UnityAction<List<Character>> OnTargetsSelected;
    public static event UnityAction OnCharactersSpawned;

    [SerializeField] private Cell[,] cellArray;
    private Vector2[,] gridPointsLocations;

    private CombatAction selectedCombatAction;
    private Vector2 cellCoord = new();

    private void OnEnable()
    {
        TurnManager.Instance.OnBeginTurn += SpawnNewCharacters;
        Character.OnDie += OnEnemyDeath;
        UICombatAction.OnCombatActionSelected += GridSelectionToggle;
        TargetingBox.OnNewCoordinateSelected += HighlightEnemies;
        CharacterManager.Instance.OnNewPlayerSelected += NewPlayerDeselectCells;
    }

    private void OnDisable()
    {
        TurnManager.Instance.OnBeginTurn -= SpawnNewCharacters;
        Character.OnDie -= OnEnemyDeath;
        UICombatAction.OnCombatActionSelected -= GridSelectionToggle;
        TargetingBox.OnNewCoordinateSelected -= HighlightEnemies;
        CharacterManager.Instance.OnNewPlayerSelected -= NewPlayerDeselectCells;
    }

    public void Awake()
    {
        grid = gameObject;
        HorizCellCount = EnemyEncounter.Columns; VertCellCount = EnemyEncounter.Rows;
        cellArray = new Cell[HorizCellCount, VertCellCount];
        totalCells = HorizCellCount * VertCellCount;
        rt = canvas.GetComponent<RectTransform>();

        GetEncounterData();
        InitializeGrid();
        SpawnInitialCharacters(totalCells, characterTypes.Count);
    }

    private void FixedUpdate()
    {
        if (boxFollowMouse)
        {
            SetTargetBoxToMousePosition();
        }
    }

    //Grouped Functions
    void GetEncounterData()
    {
        CreateListEnemyTypes();
        GetEnemiesTypeToSpawnFromEncounter();
        SetTotalEnemiesOFEachTypeToSpawn();
    }

    private void InitializeGrid()
    {
        SetGridLayout();
        InitializeGridCells();
        SetGridSnapPoints();
    }


    //Grid Functions
    private void SetGridLayout()
    {
        if (totalCells > 25)
        {
            gridLayoutGroup.constraintCount = HorizCellCount;
            gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        }
        else
        {
            gridLayoutGroup.constraintCount = VertCellCount;
            gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
        }
    }

    private void InitializeGridCells()
    {
        for (int x = 0; x < HorizCellCount; x++)
        {
            for (int y = 0; y < VertCellCount; y++)
            {
                var cellObj = Instantiate<GameObject>(cellPrefab, transform);
                var Cell = cellObj.GetComponent<Cell>();
                Cell.Initialize(x, y);
                cellArray[x, y] = Cell;
            }
        }
        OnCellsInstantiated?.Invoke();
    }

    private void SetGridSnapPoints()
    {
        int a = HorizCellCount + HorizCellCount - 1;
        int b = VertCellCount + VertCellCount - 1;
        cellWidth = gridLayoutGroup.cellSize.x;
        cellHeight = gridLayoutGroup.cellSize.y;
        float space = gridLayoutGroup.spacing.x;
        float gridWidth = (cellWidth * HorizCellCount + space * (HorizCellCount));
        float gridHeight = (cellHeight * VertCellCount + space * (VertCellCount));
        gridPointsLocations = new Vector2[a, b];
        Vector2 origin = grid.transform.position;
        Vector2 actual = origin - new Vector2(gridWidth / 2, gridHeight / 2);
        Vector2 offset = new();

        float horizPointSpace = (cellWidth + space) / 2;
        float vertPointSpace = (cellHeight + space) / 2;

        for (int x = 0; x < a; x++)
        {
            for (int y = 0; y < b; y++)
            {
                offset.x = horizPointSpace * (x + 1);
                offset.y = vertPointSpace * (y + 1);
                gridPointsLocations[x, y] = new();
                gridPointsLocations[x, y] = actual + offset;
                var obj = Instantiate(testPoint, gridPointsLocations[x, y], transform.rotation, gridContainer.transform);
                obj.GetComponent<TargetPoint>().Initialize(x, y);
            }
        }
    }


    //Spawn/Despawn Functions
    void GetEnemiesTypeToSpawnFromEncounter()
    {
        encounterEnemyTypeCount[0] = EnemyEncounter.Enemy1Number;
        encounterEnemyTypeCount[1] = EnemyEncounter.Enemy2Number;
        encounterEnemyTypeCount[2] = EnemyEncounter.Enemy3Number;
        encounterEnemyTypeCount[3] = EnemyEncounter.Enemy4Number;
        encounterEnemyTypeCount[4] = EnemyEncounter.Enemy5Number;
    }

    void SetTotalEnemiesOFEachTypeToSpawn()
    {
        int totalTypes = characterTypes.Count;

        for (int i = 0; i < totalTypes; i++)
        {
            totalEnemiesToSpawn.Add(encounterEnemyTypeCount[i]);
        }
    }

    private void CreateListEnemyTypes()
    {
        for (int i = 0; i < EnemyEncounter.CharacterTypes.Length; i++)
        {
            var characterType = EnemyEncounter.CharacterTypes[i];
            characterTypes.Add(characterType);
        }
    }

    void SpawnInitialCharacters(int totalGridCells, int enemyTypes)
        //The initial spawn makes sure that enemies are spawned evenly by type on the grid
    {
        int[] floatPlusRemainder = new int[2];
        List<int> enemiesToSpawn = new();
        List<CharacterType> initialCharacterTypes = new();
        foreach (CharacterType enemyType in characterTypes)
        {
            initialCharacterTypes.Add(enemyType);
        }

        floatPlusRemainder = DivideWithRemainder(floatPlusRemainder, totalGridCells, enemyTypes);
        for (int i = 0; i < enemyTypes; i++)
        {
            enemiesToSpawn.Add(floatPlusRemainder[0]);
            if (floatPlusRemainder[1] > 0)
            {
                enemiesToSpawn[i] += 1;
                floatPlusRemainder[1]--;
            }
        }

        foreach (var cell in cellArray)
        {
            int x = UnityEngine.Random.Range(0, enemiesToSpawn.Count);

            if (cell.cellCharacter == null)
            {
                var enemy = Instantiate(enemyPrefab, cell.transform);
                cell.cellCharacter = enemy.GetComponent<Character>();
                cell.cellCharacter.Initialize(initialCharacterTypes[x]);
                enemiesToSpawn[x]--;
                totalEnemiesToSpawn[x]--;
                if (enemiesToSpawn[x] == 0)
                {
                    enemiesToSpawn.RemoveAt(x);
                    initialCharacterTypes.RemoveAt(x);
                }
                if (totalEnemiesToSpawn[x] == 0)
                {
                    characterTypes.RemoveAt(x);
                    totalEnemiesToSpawn.RemoveAt(x);
                }
            }
        }
        OnCharactersSpawned?.Invoke();
    }

    public void SpawnNewCharacters(bool firstTurn)
    {
        //This just spawns random characters in to empty cells
        if (!firstTurn && totalEnemiesToSpawn.Count != 0)
        {
            foreach (var cell in cellArray)
            {
                int x = UnityEngine.Random.Range(0, totalEnemiesToSpawn.Count);

                if (cell.cellCharacter == null)
                {
                    var enemy = Instantiate<GameObject>(enemyPrefab, cell.transform);
                    cell.cellCharacter = enemy.GetComponent<Character>();
                    cell.cellCharacter.Initialize(characterTypes[x]);
                    totalEnemiesToSpawn[x]--;
                    if (totalEnemiesToSpawn[x] == 0)
                    {
                        characterTypes.RemoveAt(x);
                        totalEnemiesToSpawn.RemoveAt(x);
                        if (totalEnemiesToSpawn.Count == 0) 
                            return;
                    }
                }
            }
        }
        OnCharactersSpawned?.Invoke();
    }

    void OnEnemyDeath(Character character)
    {
        if (!character.isPlayer)
        {
            var deadChar = CycleCellArrayForDeadCharacter(character);
            deadChar.cellCharacter = null;
        }
    }

    Cell CycleCellArrayForDeadCharacter(Character character)
    {
        for (int x = 0; x < HorizCellCount; x++)
        {
            for (int y = 0; y < VertCellCount; y++)
            {
                if (cellArray[x, y].cellCharacter == character)
                {
                    return cellArray[x, y];
                }
            }
        }
        return null;
    }


    //Helper Functions
    public int GetTotalEnemies()
    {
        var totalEnemies = EnemyEncounter.Enemy1Number + EnemyEncounter.Enemy2Number + EnemyEncounter.Enemy3Number + EnemyEncounter.Enemy4Number + EnemyEncounter.Enemy5Number;
        return totalEnemies;
    }

    public int GetEnemyCellTotal()
    {
        return HorizCellCount * VertCellCount;
    }

    private int[] DivideWithRemainder(int[] array, float dividend, float divisor)
    {
        float quotient;
        float remainder;
        quotient = dividend / divisor;
        remainder = dividend % divisor;
        array[0] = Mathf.FloorToInt(quotient);
        array[1] = Mathf.FloorToInt(remainder);

        return array;
    }


    //Grid Targeting Functions
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (gridSelectionAllowed)
        {
            boxFollowMouse = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (gridSelectionAllowed)
        {
            DeHighlightUnselectedCells();
        }
        boxFollowMouse = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (gridSelectionAllowed && numberOfTargetSelections > 1)
        {
            numberOfTargetSelections--;

            for (int x = 0; x < HorizCellCount; x++)
            {
                for (int y = 0; y < VertCellCount; y++)
                {
                    if (cellArray[x, y].isHighlighted && cellArray[x, y].isSelected == false)
                    {
                        cellArray[x, y].Select();
                    }
                }
            }
        }
        else
        {
            for (int x = 0; x < HorizCellCount; x++)
            {
                for (int y = 0; y < VertCellCount; y++)
                {
                    if (cellArray[x, y].isHighlighted && cellArray[x, y].isSelected == false)
                    {
                        cellArray[x, y].Select();
                    }
                }
            }

            gridSelectionAllowed = false;
            targetCharacters.Clear();

            for (int x = 0; x < HorizCellCount; x++)
            {
                for (int y = 0; y < VertCellCount; y++)
                {
                    if (cellArray[x, y].isSelected && cellArray[x, y].cellCharacter != null)
                    {
                        targetCharacters.Add(cellArray[x, y].cellCharacter);
                    }
                }
            }

            OnTargetsSelected?.Invoke(targetCharacters);
        }
    }

    private void SetTargetBoxToMousePosition()
    {
        Vector2 mousePos;
        //Convert mouse position to local coordinates
        mousePos = Input.mousePosition;
        mousePos = rt.InverseTransformPoint(mousePos);

        targetBox.transform.localPosition = mousePos;
    }

    private void HighlightEnemies(Vector2 pointCoord)
    {
        int maxHeight;
        int maxWidth;
        int xSubtractAmt;
        int ySubtractAmt;
        int startIntx;
        int startInty;

        DeHighlightUnselectedCells();

        PlayerCombatAction playerCombatAction = selectedCombatAction as PlayerCombatAction;

        //Set target width and height
        if (playerCombatAction.RowsToTarget > 0)
        {
            if (EnemyEncounter.Rows < playerCombatAction.RowsToTarget) maxHeight = EnemyEncounter.Rows;
            else maxHeight = playerCombatAction.RowsToTarget;
        }
        else
        {
            if (EnemyEncounter.Rows < playerCombatAction.AreaHeightToTarget) maxHeight = EnemyEncounter.Rows;
            else maxHeight = playerCombatAction.AreaHeightToTarget;
        }

        if (playerCombatAction.ColumnsToTarget > 0)
        {
            if (EnemyEncounter.Columns < playerCombatAction.ColumnsToTarget) maxWidth = EnemyEncounter.Columns;
            else maxWidth = playerCombatAction.ColumnsToTarget;
        }
        else
        {
            if (EnemyEncounter.Columns < playerCombatAction.AreaWidthToTarget) maxWidth = EnemyEncounter.Columns;
            else maxWidth = playerCombatAction.AreaWidthToTarget;
        }

        //Set Even or Odd for Coordinate and Width and Height of target area
        bool widthEven = EvenOrOdd(maxWidth);
        bool heightEven = EvenOrOdd(maxHeight);

        //Set Subtract Amt for conversion and start coord for highlight loop
        if (widthEven)
        {
            xSubtractAmt = maxWidth - 1;
            if (pointCoord.x - xSubtractAmt <= 0) startIntx = 0;
            else startIntx = ((int)pointCoord.x - xSubtractAmt) / 2;
        }
        else
        {
            xSubtractAmt = (maxWidth - 1) / 2;
            if (pointCoord.x == 0) startIntx = 0;
            else startIntx = (int)pointCoord.x / 2 - xSubtractAmt;
        }


        if (heightEven)
        {
            ySubtractAmt = maxHeight - 1;
            if (pointCoord.y - ySubtractAmt <= 0) startInty = 0;
            else startInty = ((int)pointCoord.y - ySubtractAmt) / 2;
        }
        else
        {
            ySubtractAmt = (maxHeight - 1) / 2;
            if (pointCoord.y == 0) startInty = 0;
            else startInty = (int)pointCoord.y / 2 - ySubtractAmt;
        }

        if (EnemyEncounter.Rows - startInty < maxHeight) startInty = EnemyEncounter.Rows - maxHeight;
        else if (startInty < 0) startInty = 0;

        if (EnemyEncounter.Columns - startIntx < maxWidth) startIntx = EnemyEncounter.Columns - maxWidth;
        else if (startIntx < 0) startIntx = 0;

        //Highlight Cells depending on Targeting type
        if (playerCombatAction.TargetingType == PlayerCombatAction.TargetType.Single)
        {
            cellCoord = ConvertEvenPointToCellCoordinate(pointCoord);
            cellArray[(int)cellCoord.x, (int)cellCoord.y].Highlight();
        }
        else if (playerCombatAction.TargetingType == PlayerCombatAction.TargetType.All)
        {
            TriggerHighlightCells(0, HorizCellCount, 0, VertCellCount);
        }
        else if (playerCombatAction.TargetingType == PlayerCombatAction.TargetType.Rows)
        {
            TriggerHighlightCells(0, HorizCellCount, startInty, startInty + maxHeight);
        }
        else if (playerCombatAction.TargetingType == PlayerCombatAction.TargetType.Columns)
        {
            TriggerHighlightCells(startIntx, startIntx + maxWidth, 0, VertCellCount);
        }
        else if (playerCombatAction.TargetingType == PlayerCombatAction.TargetType.Area)
        {
            TriggerHighlightCells(startIntx, startIntx + maxWidth, startInty, startInty + maxHeight);
        }
    }

    private void TriggerHighlightCells(int startX, int endX, int startY, int endY)
    {
        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                cellArray[x, y].Highlight();
            }
        }
    }

    //Externally triggered Functions
    private void NewPlayerDeselectCells(Character character)
    {
        DeHighlightAllCells();

        //Highlight all cells that match the character's Targets
        if(character.GetTargetList().Count > 0)
        {
            for (int x = 0; x < HorizCellCount; x++)
            {
                for (int y = 0; y < VertCellCount; y++)
                {
                    var foundCharacter = character.GetTargetList().Where(c => c == cellArray[x, y].cellCharacter).Any();
                    if (foundCharacter) cellArray[x, y].Highlight();
                }
            }
        }
    }

    public void DeHighlightAllCells()
    {
        DehighlightAllCells?.Invoke();
    }

    public void DeHighlightUnselectedCells()
    {
        for (int x = 0; x < HorizCellCount; x++)
        {
            for (int y = 0; y < VertCellCount; y++)
            {
                if (cellArray[x, y].isHighlighted && cellArray[x, y].isSelected == false)
                {
                    cellArray[x, y].DeHighlight();
                }
            }
        }
    }

    private void GridSelectionToggle(PlayerCombatAction combataction)
    {
        selectedCombatAction = combataction;

        if (combataction != null)
        {
            if (combataction.MultiSelect == true) numberOfTargetSelections = combataction.NumberToSelect;
            gridSelectionAllowed = true;
        }
        else
        {
            gridSelectionAllowed = false;
            numberOfTargetSelections= 0;
        }
    }

    //Internal helper Functions
    Vector2 ConvertEvenPointToCellCoordinate(Vector2 point)
    {
        Vector2 cellCoord = new();

        if (point.x == 0) cellCoord.x = 0;
        else cellCoord.x = point.x / 2;

        if (point.y == 0) cellCoord.y = 0;
        else cellCoord.y = point.y / 2;

        return cellCoord;
    }

    bool EvenOrOdd(int numb)
    {
        if (numb % 2 == 0 || numb == 0) return true;
        else return false;
    }
}
