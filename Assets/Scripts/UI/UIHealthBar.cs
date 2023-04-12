using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHealthBar : MonoBehaviour
{
    [SerializeField] private Image healthFill;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Character character;

    private void OnDisable()
    {
        character.OnHealthChange -= OnUpdateHealth;
    }

    private void Start()
    {
        if (!character.isPlayer)
        {
            character.OnHealthChange += OnUpdateHealth;
            healthText.text = character.GetStat(CharacterStatType.HP).GetValue() + " / " + character.GetStat(CharacterStatType.HP).GetMaxValue();
        }
           
    }

    public void SetCharacter(Character character)
    {
        this.character = character;
        character.OnHealthChange += OnUpdateHealth;
        healthText.text = character.GetStat(CharacterStatType.HP).GetValue() + " / " + character.GetStat(CharacterStatType.HP).GetMaxValue();
    }

    void OnUpdateHealth()
    {
        healthFill.fillAmount = character.GetHealthPercentage();
        healthText.text = character.GetStat(CharacterStatType.HP).GetValue() + " / " + character.GetStat(CharacterStatType.HP).GetMaxValue();
    }
}
