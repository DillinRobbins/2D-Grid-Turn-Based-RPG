using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ClassCounter : MonoBehaviour
{
    TextMeshProUGUI countText;
    int count = 0;
    Image counterImage;

    [SerializeField] Character character;
    CharacterType.Race race;

    [SerializeField] Sprite impCounterSprite;
    //[SerializeField] Sprite faeCounterSprite;

    public void SetCharacter(Character character)
    {
        this.character = character;
        SetCounterImage();
    }

    void SetCounterImage()
    {
        counterImage = GetComponentInChildren<Image>();
        countText = GetComponentInChildren<TextMeshProUGUI>();

        race = character.GetCharacterType().RaceofCharacter;

        if(race == CharacterType.Race.Imp)
        {
            counterImage.sprite = impCounterSprite;
        }
        /*
        switch(race)
        {
            case CharacterType.Race.Imp:
                counterImage.sprite = impCounterSprite;
                break;

            case CharacterType.Race.Fae:
                break;
        }*/
    }

    public void IncrementCount()
    {
        count++;
        countText.text = count.ToString();
    }

    public void ResetCount()
    {
        count = 0;
        countText.text = count.ToString();
    }
}
