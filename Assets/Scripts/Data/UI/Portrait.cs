using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Portrait : MonoBehaviour
{
    private int numberOfCharacters = 1;
    private bool isFirstCharacter = true;

    [SerializeField] private TextMeshProUGUI characterNumberText;
    [SerializeField] private GameObject CountText;
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private Sprite attackSprite;
    [SerializeField] private Sprite spellSprite;
    [SerializeField] private Sprite healSprite;
    [SerializeField] private Sprite defenseSprite;
    [SerializeField] private Sprite totemSprite;

    [SerializeField] private Image portraitSprite;
    [SerializeField] private GameObject targetPortraitsContainer;
    [SerializeField] private GameObject subPortraitRef;

    public CombatAction combatAction;
    public CharacterType characterType;
    public List<CharacterType> targetTypeList;
    public List<Character> targetList;



    public void SetCharacters(List<Character> characters)
    {

    }

    private void OnEnable()
    {
        TurnCounter.OnPortraitsCleared += DestroyThis;
    }

    private void OnDisable()
    {
        TurnCounter.OnPortraitsCleared -= DestroyThis;
    }

    public void InitializePortrait(CharacterType type, CombatAction ca)
    {
        characterType = type;
        combatAction = ca;
        if(type != null)
        {
            canvasGroup.alpha = 1;
            portraitSprite.sprite = type.characterSprite;
        }
        else if(ca != null)
            {
                canvasGroup.alpha = 1;

                switch (ca.ActionType)
                {
                    case CombatAction.Type.Attack:
                        portraitSprite.sprite = attackSprite;
                        SetDamage(ca.Damage);
                        break;

                    case CombatAction.Type.Spell:
                        portraitSprite.sprite = spellSprite;
                        SetDamage(ca.SpellDamage);
                    break;

                    case CombatAction.Type.Heal:
                        portraitSprite.sprite = healSprite;
                        SetDamage(ca.HealAmount);
                    break;

                    case CombatAction.Type.Defense:
                        portraitSprite.sprite = defenseSprite;
                        float defAmount = (ca.DefenseAmount * 100);
                        SetDefense((int)defAmount);
                    break;

                    case CombatAction.Type.Totem:
                        portraitSprite.sprite = totemSprite;
                    break;
                }
            }
    }

    public void InitializePortraitTargets()
    {
        foreach(CharacterType target in targetTypeList)
        {
            GameObject portraitObj = Instantiate(subPortraitRef, this.targetPortraitsContainer.transform);
            Portrait portraitScript = portraitObj.GetComponent<Portrait>();
            portraitScript.InitializePortrait(target, null);



            /*var numberOfCharacters = portraitScript.targetList.FindAll(c => c.GetCharacterType() == target).Count;
            portraitScript.UpdateNumberofCharacters(numberOfCharacters);*/

            foreach (Character character in targetList)
            {
                if (character.GetCharacterType() != target)
                    continue;

                if (portraitScript.isFirstCharacter)
                    portraitScript.isFirstCharacter = false;
                else
                    portraitScript.UpdateNumberofCharacters();
            }
        }
    }

    public void SetTargetTypeList(List<CharacterType> targets)
    {
        targetTypeList.AddRange(targets);
    }

    public void SetTargetList(List<Character> targets)
    {
        targetList.AddRange(targets);
    }

    public void UpdateNumberofCharacters(/*int characters*/)
    {
        if (!CountText.activeSelf)
        {
            CountText.SetActive(true);
        }
            numberOfCharacters++;
            characterNumberText.text = numberOfCharacters.ToString();
        /*if (characters > 1)
        {
            CountText.SetActive(true);
            characterNumberText.text = numberOfCharacters.ToString();
        }  */
    }

    public void SetDamage(int damage)
    {
        CountText.SetActive(true);
        characterNumberText.text = damage.ToString();
    }

    public void SetDefense(int defense)
    {
        CountText.SetActive(true);
        characterNumberText.text = defense.ToString() + " %";
    }

    private void DestroyThis()
    {
        Destroy(gameObject);
    }
}
