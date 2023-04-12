using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class TotemManager : MonoBehaviour
{
    public List<TotemInstance> TotemList = new();
    public List<TotemCell> TotemCellList;

    public static TotemManager Instance;


    private void OnEnable()
    {
        Instance = this;
    }

    private void Start()
    {
        TotemCellList = GetComponentsInChildren<TotemCell>()?.ToList();
    }

    public void AddNewTotem(GameObject totemPrefab, PlayerCell playerCell)
    {
        TotemInstance totemInstance;

        if (!playerCell.totemSlot.HasTotem())
            totemInstance = playerCell.totemSlot.SpawnTotem(totemPrefab, playerCell.GetCharacter());
        else
        {
            var totemSpawnCell = TotemCellList.First(t => !t.HasTotem());
            totemInstance = totemSpawnCell.SpawnTotem(totemPrefab, playerCell.GetCharacter());
        }

        ApplyTotemEffects(totemInstance, totemInstance.totemEffects, totemInstance.caster, totemInstance.normalApplyToCasterOnly);
    }

    public void ApplyCurrentTotemEffects(List<Character> characters)
    {
        if(TotemList.Count == 0) return;

        for (int i = 0; i < TotemList.Count; i++)
        {
            TotemList[i].ApplyCustomEffects();

            //play totem animation
            if (TotemList[i].eventActivated) continue;

            TotemList[i].turnsRemaining--;


            if (TotemList[i].turnsRemaining <= 0)
            {
                TotemList[i].DestructionSequence();
            }

            TotemList.RemoveAll(t => t == null);
        }
    }

    public void ApplyTotemEffects(TotemInstance totemInstance, List<Effect> totemEffects, Character character, bool applyOnlyToCaster)
    {
        var playerList = CharacterManager.Instance.GetPlayerCharacterList().ToList();

        if (!applyOnlyToCaster)
            foreach (Character player in playerList)
            {
                foreach (Effect totemEffect in totemEffects)
                {
                    player.characterEffects.AddNewEffect(totemEffect);
                }
            }
        else
        {
            foreach (Effect totemEffect in totemEffects)
            {
                character.characterEffects.AddNewEffect(totemEffect);
            }
        }
    }

    public void AddSpawnedTotemToList(TotemInstance totemInstance)
    {
        TotemList.Add(totemInstance);
    }
}
