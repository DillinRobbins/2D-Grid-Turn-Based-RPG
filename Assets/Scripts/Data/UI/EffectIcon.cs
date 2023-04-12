using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EffectIcon : MonoBehaviour
{
    public Effect effect;
    public Image icon;
    [SerializeField] TextMeshProUGUI stacksText;
    public int stacksCount;


    void Awake()
    {
        icon = GetComponent<Image>();
    }

    public void Initialize(Effect effect)
    {
        this.effect = effect;
        icon.sprite = effect.icon;
        stacksCount = 1;

        if (effect.isBuff)
            icon.color = Color.green;
    }

    public void AddCount()
    {
        stacksCount++;
        if(stacksCount > 1 && stacksText.enabled == false)
            stacksText.enabled = true;
            
        stacksText.text = stacksCount.ToString();
    }

    public void RemoveCount(int numberToRemove)
    {
        stacksCount -= numberToRemove;
        stacksText.text = stacksCount.ToString();

        if(stacksCount == 1 && stacksText.enabled == true)
            stacksText.enabled = false;
    }
}
