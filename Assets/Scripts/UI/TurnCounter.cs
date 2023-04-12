using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.Events;

public class TurnCounter : MonoBehaviour
{
    [SerializeField] private GameObject portraitRef;
    private List<Portrait> portraitList = new();
    [SerializeField] private GameObject combatActionIconContainer;
    [SerializeField] private GameObject targetPortraitContainer;

    public static event UnityAction OnPortraitsCleared;

    private void OnEnable()
    {
        Character.OnPlayerCombatActionChange += BeginPortraitInstantiation;
        Character.OnPlayerSelectTargets += BeginPortraitInstantiation;
        CharacterManager.Instance.OnAllEnemyTargetsSelected += BeginPortraitInstantiation;
        CharacterManager.Instance.OnCharactersReset += BeginPortraitInstantiation;
    }

    private void OnDisable()
    {
        Character.OnPlayerCombatActionChange -= BeginPortraitInstantiation;
        Character.OnPlayerSelectTargets -= BeginPortraitInstantiation;
        CharacterManager.Instance.OnAllEnemyTargetsSelected -= BeginPortraitInstantiation;
        CharacterManager.Instance.OnCharactersReset -= BeginPortraitInstantiation;
    }

    private void BeginPortraitInstantiation()
    {
        ClearPortraits();

        var Characters = CharacterManager.Instance.GetCharactersList();

        foreach (Character character in Characters)
        {
            CharacterType type = character.GetCharacterType();
            CombatAction ca = character.GetCombatAction();

            if(portraitList.Count == 0)
            {
                var thisPortraitObj = Instantiate(portraitRef, gameObject.transform) as GameObject;
                var PortraitScript = thisPortraitObj.GetComponent<Portrait>();
                portraitList.Add(PortraitScript);
                PortraitScript.InitializePortrait(type, ca);
                if (character.GetTargets() != 0)
                {
                    PortraitScript.SetTargetList(character.GetTargetList());
                    PortraitScript.SetTargetTypeList(character.GetTargetTypeList());
                }
            }
            else
            {
                var isSimilar = portraitList.Where(p => p.characterType == type && p.combatAction == ca).Any();

                if (isSimilar)
                {
                    /*var numberOfCharacters = portraitList.FindAll(c => c.characterType == type).Count;*/
                    
                    foreach (Portrait portrait in portraitList)
                    {
                        if (portrait.characterType == type && portrait.combatAction == ca)
                        {
                            portrait.UpdateNumberofCharacters(/*numberOfCharacters*/);
                        }
                    }
                }
                else
                {
                    var thisPortraitObj = Instantiate(portraitRef, gameObject.transform) as GameObject;
                    var PortraitScript = thisPortraitObj.GetComponent<Portrait>();
                    portraitList.Add(PortraitScript);
                    PortraitScript.InitializePortrait(type, ca);
                    if (character.GetTargets() != 0)
                    {
                        PortraitScript.SetTargetList(character.GetTargetList());
                        PortraitScript.SetTargetTypeList(character.GetTargetTypeList());
                    }
                }
            }
        }
        foreach(Portrait portrait in portraitList)
        {
            var attackIconObj = Instantiate(portraitRef, combatActionIconContainer.transform);
            var portraitScript = attackIconObj.GetComponent<Portrait>();
            portraitScript.InitializePortrait(null, portrait.combatAction);

            var targetPortraits = Instantiate(portraitRef, targetPortraitContainer.transform);
            var portraitScript2 = targetPortraits.GetComponent<Portrait>();
            portraitScript2.SetTargetList(portrait.targetList);
            portraitScript2.SetTargetTypeList(portrait.targetTypeList);
            portraitScript2.InitializePortraitTargets();
        }
    }

    private void ClearPortraits()
    {
        portraitList.Clear();
        OnPortraitsCleared?.Invoke();
    }
}
