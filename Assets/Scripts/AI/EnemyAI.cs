using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private AnimationCurve healChanceCurve;
    private Character character;
    private List<Character> playerCharacters;
    private List<Character> targets = new();

    private EnemyCombatAction ca;

    private void OnEnable()
    {
        TurnManager.Instance.OnBeginTurn += TurnStartActions;
        CharacterManager.Instance.OnPlayersInitialized += SetPotentialTargets;
        Character.OnPlayerDie += RemoveDeadPlayerFromPotentialTargets;
    }

    private void OnDisable()
    {
        TurnManager.Instance.OnBeginTurn -= TurnStartActions;
        CharacterManager.Instance.OnPlayersInitialized -= SetPotentialTargets;
        Character.OnPlayerDie -= RemoveDeadPlayerFromPotentialTargets;
    }

    private void Awake()
    {
        character = GetComponent<Character>();
        playerCharacters = new();
    }

    private void SetPotentialTargets(List<Character> players)
    {
        foreach(Character player in players)
        {
            playerCharacters.Add(player);
        }
        DetermineCombatAction();
    }

    private void RemoveDeadPlayerFromPotentialTargets(Character player)
    {
        var c = playerCharacters.Find(c => c == player);
        playerCharacters.Remove(c);
    }

    void TurnStartActions(bool firstTurn)
    {
        if (!firstTurn)
            DetermineCombatAction();
    }

    void DetermineCombatAction()
    {
        if (!character.isPlayer)
        {

            ca = null;

            float combatActionChance = Random.value;

            for(int i = 0; i < character.CombatActions.Count; i++)
            {
                if(combatActionChance < character.CombatActions[i].CombatActionChance || i == character.CombatActions.Count - 1)
                {
                    ca = character.CombatActions[i] as EnemyCombatAction;
                    break;
                }
            }

            if (ca != null)
            {
                //Set symbol for combat action on enemy.
                character.ChangeCurrentCombatAction(ca);
            }
            else
            {
                character.ChangeCurrentCombatAction(null);
            }
                
        }
    }

    public void SetNewTargets()
    {
        targets.Clear();

        List<Character> players = new();
        foreach(Character player in playerCharacters)
        {
            players.Add(player);
        }

        for(int i = 0; i < ca.NumberofTargets; i++)
        {
            var x = (int)Random.Range(0, players.Count);
            targets.Add(players[x]);
            players.Remove(players[x]);
        }
        character.SetTargets(targets);
    }

    public void CopyTargets(List<Character> players)
    {
        targets.Clear();

        foreach (Character target in players)
        {
            targets.Add((Character)target);
        }
        character.SetTargets(targets);
    }

    /*bool HasCombatActionOfType(CombatAction.Type type)
    {
        return character.CombatActions.Exists(x => x.ActionType == type);
    }

    CombatAction GetCombatActionOfType(CombatAction.Type type)
    {
        List<CombatAction> availableActions = character.CombatActions.FindAll(x => x.ActionType == type);

        return availableActions[Random.Range(0, availableActions.Count)];
    }*/
}
