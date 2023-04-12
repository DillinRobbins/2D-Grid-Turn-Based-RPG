using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private List<Character> Characters = new();
    [SerializeField] private List<Character> PlayerCharacters = new();
    [SerializeField] private List<Character> EnemyCharacters = new();
    [SerializeField] private List<Character> DeadCharacters = new();

    [SerializeField] private TurnCounter TurnCounter;
    [SerializeField] private EnemyGrid EnemyGrid;
    [SerializeField] private TotemManager TotemManager;

    float shortDelay = .2f;
    float longDelay = 1f;
    private WaitForSeconds shortWFS;
    private WaitForSeconds longWFS;
    //int characterTurnIndex = 0;
    private bool lastIsPlayer = false;

    public int charactersToSpawn { get; private set; }
    private int enemyCombatActionsSelected = 0;
    [SerializeField] private int characterTurnsExecuted = 0;
    public event UnityAction OnAllEnemyTargetsSelected;

    public Character selectedPlayerCharacter;

    public event UnityAction<Character> OnNewPlayerSelected;
    public event UnityAction<List<Character>> OnPlayersInitialized;
    public event UnityAction OnCharactersReset;

    private bool isResetting = false;

    public int testNumberofAttacks = 0;
    public int testNumberofEffects = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
            Instance = this;

        shortWFS = new WaitForSeconds(shortDelay);
        longWFS = new WaitForSeconds(longDelay);
    }

    public static CharacterManager Instance;

    private void OnEnable()
    {
        Character.OnDie += OnCharacterDie;
        Character.OnSpeedChange += SortBySpeed;
        Character.OnEnemyCombatActionSelected += EnemyCombatActionsSelectedUpdate;
        Character.OnTurnExecuted += CharacterTurnExecutedUpdate;
        TurnManager.Instance.OnEndTurn += StartExecuteTurn;
        TurnManager.Instance.OnBeginTurn += ResetCharactersAndPortraits;
        EnemyGrid.OnCellsInstantiated += SetCharactersToSpawn;
    }

    private void OnDisable()
    {
        Character.OnDie -= OnCharacterDie;
        Character.OnSpeedChange -= SortBySpeed;
        Character.OnEnemyCombatActionSelected -= EnemyCombatActionsSelectedUpdate;
        Character.OnTurnExecuted -= CharacterTurnExecutedUpdate;
        TurnManager.Instance.OnEndTurn -= StartExecuteTurn;
        TurnManager.Instance.OnBeginTurn -= ResetCharactersAndPortraits;
        EnemyGrid.OnCellsInstantiated -= SetCharactersToSpawn;
    }

    private void Start()
    {
        foreach (Character character in Characters)
        {
            if (character.isPlayer)
            {
                PlayerCharacters.Add(character);
            }
        }

        OnPlayersInitialized?.Invoke(PlayerCharacters);
        UICombatAction.Instance.UpdateMenuOptions(selectedPlayerCharacter);
    }

    private void SortBySpeed()
    {
        if (isResetting) return;

        Characters = Characters.OrderByDescending(c => c.GetStat(CharacterStatType.Speed).GetValue()).ToList();
    }

    public void AddCharacterToList(Character character)
    {
        Characters.Add(character);
        if(!character.isPlayer)
            EnemyCharacters.Add(character);
        charactersToSpawn--;
        if(charactersToSpawn == 0)
        {
            SortBySpeed();
        }
    }

    public void StartExecuteTurn()
    {
        StartCoroutine(ExecuteTurn());
    }

    IEnumerator ExecuteTurn()
    {
        var CharacterList = Characters.ToList();

        Debug.Log("Number of Character: " + Characters.Count);

        while(CharacterList.Count > 0)
        {
            CharacterList = CharacterList.OrderByDescending(c => c.GetStat(CharacterStatType.Speed).GetValue()).ToList();

            if (CharacterList[0].canvasGroup.alpha != 0)
                yield return DelayExecution(CharacterList[0].isPlayer);

            CharacterList[0].CastCombatAction();
            CharacterList.RemoveAt(0);
        }
    }

    private void AllTurnsExecuted()
    {
        TurnManager.Instance.InvokeBeginNextTurn();
    }

    public void ResetCharactersAndPortraits(bool firstTurn)
    {
        if(!firstTurn)
        {
            isResetting = true;

            Debug.Log(testNumberofAttacks.ToString());
            Debug.Log(testNumberofEffects.ToString());

            testNumberofAttacks = 0;
            testNumberofEffects = 0;

            ResetStatsAndSelections();
            
            RemoveAllExhaustedEffectIcons();
            TotemManager.ApplyCurrentTotemEffects(PlayerCharacters);
            ApplyAllCurrentEffects();

            RemoveDeadCharacters();
            isResetting = false;
            SortBySpeed();
            OnCharactersReset?.Invoke();
        }
    }
    
    private WaitForSeconds DelayExecution(bool isPlayer)
    {
        if (isPlayer)
        {
            lastIsPlayer = true;
            return longWFS;
        }
            
        else if (lastIsPlayer)
        {
            lastIsPlayer = false;
            return longWFS;
        }
        else
        {
            return shortWFS;
        }   
    }

    void OnCharacterDie(Character character)
    {
        DeadCharacters.Add(character);
        charactersToSpawn++;
    }

    void RemoveDeadCharacters()
    {
        foreach (Character character in DeadCharacters)
        {
            Characters.RemoveAll(c => c == character);
        }

        for(int x = DeadCharacters.Count - 1; x >= 0; x--)
        {
            Destroy(DeadCharacters[x].gameObject);
        }

        DeadCharacters.Clear();
    }

    public List<Character> GetCharactersList()
    {
        return Characters;
    }

    public List<Character> GetPlayerCharacterList()
    {
        return PlayerCharacters;
    }

    public Character GetSelectedPlayer()
    {
        return selectedPlayerCharacter;
    }

    public void SetSelectedPlayerCharacter(Character character)
    {
        selectedPlayerCharacter = character;
        OnNewPlayerSelected?.Invoke(character);
    }

    private void SetCharactersToSpawn()
    {
        charactersToSpawn = EnemyGrid.GetEnemyCellTotal() + 4;
    }

    private void CharacterTurnExecutedUpdate()
    {
        characterTurnsExecuted += 1;

        if(characterTurnsExecuted == Characters.Count)
        {
            AllTurnsExecuted();
            Debug.Log(characterTurnsExecuted.ToString());
            Debug.Log("All Turns Executed");
            characterTurnsExecuted = 0;
        }
    }

    private void EnemyCombatActionsSelectedUpdate()
    {
        enemyCombatActionsSelected += 1;
        if (enemyCombatActionsSelected == Characters.Count - PlayerCharacters.Count)
        {
            SortBySpeed();
            SetEnemyTargets();
            enemyCombatActionsSelected= 0;
        }
    }

    private void SetEnemyTargets()
    {
        List<Character> uniqueCharAndCombatActions = new();

        foreach(Character enemy in EnemyCharacters)
        {
            var match = uniqueCharAndCombatActions.Find(c => c.CurrentCombatAction == enemy.CurrentCombatAction && c.GetCharacterType() == enemy.GetCharacterType());

            if (uniqueCharAndCombatActions.Count == 0 || match == null)
            {
                uniqueCharAndCombatActions.Add(enemy);
                enemy.GetComponent<EnemyAI>().SetNewTargets();
            }
            else
            {
                enemy.GetComponent<EnemyAI>().CopyTargets(match.GetTargetList());
            }
        }

        OnAllEnemyTargetsSelected?.Invoke();
        Debug.Log("Turn Initialized");
    }

    //Grouper Functions
    private void ResetStatsAndSelections()
    {
        for (int i = 0; i < Characters.Count; i++)
        {
            Characters[i].ClearTargets();
            Characters[i].ResetStats();
            if (Characters[i].isPlayer)
                Characters[i].ChangeCurrentCombatAction(null);
            Characters[i].characterEffects.DestroyExhaustedEffects();
        }
    }

    private void ApplyAllCurrentEffects()
    {
        for (int i = 0; i < Characters.Count; i++)
        {
            Characters[i].characterEffects.ApplyCurrentEffects();
            //Add gater function to null check player characters are visible and not destroyed
        }
    }

    private void RemoveAllExhaustedEffectIcons()
    {
        for (int i = 0; i < Characters.Count; i++)
        {
            Characters[i].characterEffects.RemoveExhaustedEffectIcons();
        }
    }

    //Tester Functions
    public void IncrementAttacks()
    {
        testNumberofAttacks++;
    }

    public void IncrementEffects()
    {
        testNumberofEffects++;
    }
}
