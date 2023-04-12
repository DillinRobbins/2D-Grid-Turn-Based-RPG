using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI DisplayName;
    [SerializeField] Outline selectedOutline;

    [SerializeField] PlayerCell cell;
    [SerializeField] GameObject effectsIconDisplay;
    UIHealthBar healthBar;
    UIManaBar manaBar;
    ClassCounter counter;

    private void OnEnable()
    {
        CharacterManager.Instance.OnNewPlayerSelected += UpdateOutline;
    }

    private void OnDisable()
    {
        CharacterManager.Instance.OnNewPlayerSelected -= UpdateOutline;
    }

    public void Initialize(Character character)
    {
        DisplayName.text = character.GetCharacterType().TypeDisplayName;

        if(CharacterManager.Instance.GetSelectedPlayer() == character)
            selectedOutline.enabled = true;
        else
            selectedOutline.enabled = false;

        healthBar = GetComponentInChildren<UIHealthBar>();
        manaBar = GetComponentInChildren<UIManaBar>();
        counter = GetComponentInChildren<ClassCounter>();

        healthBar.SetCharacter(character);
        manaBar.SetCharacter(character);
        counter.SetCharacter(character);

        character.characterEffects.SetEffectIconDisplay(effectsIconDisplay);
    }

    void UpdateOutline(Character character)
    {
        if(cell.GetCharacter() == character)
            selectedOutline.enabled=true;
        else
            selectedOutline.enabled=false;

        /*var playerSaveData = SaveSystem.LoadFile<PlayerProfile>();
        playerSaveData.Save();*/
    }
}
