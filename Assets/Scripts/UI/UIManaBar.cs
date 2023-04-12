using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManaBar : MonoBehaviour
{
    [SerializeField] private Image manaFill;
    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private Character character;

    private void OnDisable()
    {
        character.OnManaChange -= OnUpdateMana;
    }

    public void SetCharacter(Character character)
    {
        this.character = character;
        character.OnManaChange += OnUpdateMana;
        manaText.text = character.GetStat(CharacterStatType.MP).GetValue() + " / " + character.GetStat(CharacterStatType.MP).GetMaxValue();
    }

    void OnUpdateMana()
    {
        manaFill.fillAmount = character.GetManaPercentage();
        manaText.text = character.GetStat(CharacterStatType.MP).GetValue() + " / " + character.GetStat(CharacterStatType.MP).GetMaxValue();
    }
}
