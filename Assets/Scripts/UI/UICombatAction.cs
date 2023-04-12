using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEditor.SearchService;
using UnityEngine.Events;

public class UICombatAction : MonoBehaviour
{
    [SerializeField] private GameObject visualContainer;
    [SerializeField] private Button[] combatActionButtons;
    [SerializeField] private Button defenseButton;
    [SerializeField] private Texture2D cursorTargetReticule;
    [SerializeField] private GameObject endTurnButton;

    public static event UnityAction<PlayerCombatAction> OnCombatActionSelected;

    public static UICombatAction Instance;
    [SerializeField] private EnemyGrid enemyGrid;

    //private bool enableSelectTarget = false;

    /*private void Update()
    {
        if (Input.GetMouseButtonDown(0) && enableSelectTarget)
        {
            //Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            onCombatActionSelected?.Invoke(null);
        }
    }*/

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
            Instance = this;
    }

    private void OnEnable()
    {
        TurnManager.Instance.OnBeginTurn +=SetUIOnBeginTurn;
        TurnManager.Instance.OnEndTurn += OnEndTurn;
        EnemyGrid.OnTargetsSelected += SetTargets;
        EnemyGrid.OnTargetsSelected += ResetCombatAction;
        CharacterManager.Instance.OnNewPlayerSelected += OnNewPlayerCharacterSelect;
    }

    private void OnDisable()
    {
        TurnManager.Instance.OnBeginTurn -= SetUIOnBeginTurn;
        TurnManager.Instance.OnEndTurn -= OnEndTurn;
        EnemyGrid.OnTargetsSelected -= SetTargets;
        EnemyGrid.OnTargetsSelected -= ResetCombatAction;
        CharacterManager.Instance.OnNewPlayerSelected -= OnNewPlayerCharacterSelect;
    }

    void SetUIOnBeginTurn(bool firstTurn)
    {
        visualContainer.SetActive(true);
        endTurnButton.SetActive(true);
        //Get character toggle and set active
    }

    public void OnNewPlayerCharacterSelect(Character character)
    {
        //Highlight selected character UI
        //Display any info on status effects
        UpdateMenuOptions(character);
    }

    public void UpdateMenuOptions(Character character)
    {
        for (int i = 0; i < combatActionButtons.Length; i++)
        {
            if (i < character.CombatActions.Count)
            {
                combatActionButtons[i].gameObject.SetActive(true);
                CombatAction ca = character.CombatActions[i];

                combatActionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = ca.DisplayName;
                combatActionButtons[i].onClick.RemoveAllListeners();
                combatActionButtons[i].onClick.AddListener(() => OnClickCombatAction(character, ca as PlayerCombatAction, false));
            }
            else
            {
                combatActionButtons[i].gameObject.SetActive(false);
            }
        }

        var da = character.DefenseAction;
        defenseButton.GetComponentInChildren<TextMeshProUGUI>().text = da.DisplayName;
        defenseButton.onClick.RemoveAllListeners();
        defenseButton.onClick.AddListener(() => OnClickCombatAction(character, da as PlayerCombatAction, true));
    }

   /* void SetTargetSelectCursor()
    {
        Cursor.SetCursor(cursorTargetReticule, Vector2.zero, CursorMode.Auto);
    }*/

    void SetTargets(List<Character> characters)
    {
            CharacterManager.Instance.selectedPlayerCharacter.SetTargets(characters);
            //enableSelectTarget = false;
    }

    void OnEndTurn()
    {
        visualContainer.SetActive(false);
        endTurnButton.SetActive(false);
        enemyGrid.DeHighlightAllCells();
    }

    public void OnClickCombatAction(Character character, PlayerCombatAction combatAction, bool isDefensive)
    {
        character.ChangeCurrentCombatAction(combatAction);
        //enableSelectTarget= true;
        enemyGrid.DeHighlightAllCells();
        //SetTargetSelectCursor();
        if(!isDefensive)
            OnCombatActionSelected?.Invoke(combatAction);
        //Change mouse cursor to Target reticule
        //Next click selects the target
    }

    public void ResetCombatAction(List<Character> dummy)
    {
        OnCombatActionSelected?.Invoke(null);
    }
}