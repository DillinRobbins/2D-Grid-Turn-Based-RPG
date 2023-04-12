using System;
using UnityEngine;
using UnityEngine.Events;

public class CharacterStat : MonoBehaviour
{
    [NonSerialized] public UnityEvent<float> OnMinValueChanged = new();
    [NonSerialized] public UnityEvent<float> OnMaxValueChanged = new();
    public UnityEvent<float> OnValueChanged = new();
    public UnityEvent<float> OnBaseValueChanged = new();

    public CharacterStatType statType = CharacterStatType.None;
    public string statName;

    [SerializeField] float maxValue = 1;
    [SerializeField] float minValue = 0;
    [SerializeField] bool setToMaxOnEnable = false;
    [SerializeField] bool hasBase = false;

    [SerializeField]private float currentValue;
    [SerializeField] private float baseValue;

    private void OnEnable()
    {
        if (setToMaxOnEnable)
            SetValue(maxValue);

        if (statType != CharacterStatType.None)
            statName = statType.ToString();
    }

    private void OnValidate()
    {
        if (statType != CharacterStatType.None && statName != statType.ToString())
            statName = statType.ToString();
    }

    public void StatConstructor(CharacterStatType type, float baseValue, float minValue, float maxValue, bool setToMax)
    {
        statType = type;

        SetMinValue(minValue);
        SetMaxValue(maxValue);

        SetBaseValue(baseValue);

        if (setToMax)
            SetValue(GetMaxValue());

        hasBase = true;

        statName = statType.ToString();
    }

    public void StatConstructor(CharacterStatType type, float baseValue, float minValue, float maxValue, float setValue)
    {
        statType = type;

        SetMinValue(minValue);
        SetMaxValue(maxValue);

        SetBaseValue(baseValue);

        SetValue(setValue);

        hasBase = true;

        statName = statType.ToString();
    }

    public void StatConstructorNoBase(CharacterStatType type, float minValue, float maxValue, bool setToMax)
    {
        statType = type;

        SetMinValue(minValue);
        SetMaxValue(maxValue);

        if(setToMax)
            SetValue(GetMaxValue());

        statName = statType.ToString();
    }

    public void StatConstructorNoBase(CharacterStatType type, float minValue, float maxValue, float setValue)
    {
        statType = type;

        SetMinValue(minValue);
        SetMaxValue(maxValue);

        SetValue(setValue);

        statName = statType.ToString();
    }

    public float GetValue()
    {
        return currentValue;
    }

    public float GetBaseValue()
    {
        return baseValue;
    }

    public float RaiseValue(float amountToAdd)
    {
        return SetValue(currentValue + amountToAdd);
    }

    public float RaiseBaseValue(float amountToAdd)
    {
        return SetBaseValue(baseValue + amountToAdd);
    }

    public float ReduceValue(float amountToRemove)
    {
        return SetValue(currentValue - amountToRemove);
    }

    public float SetValue(float newValue)
    {
        newValue = Mathf.Clamp(newValue, minValue, maxValue);
        if (newValue == currentValue)
            return currentValue;

        currentValue = newValue;
        OnValueChanged.Invoke(currentValue);
        return currentValue;
    }

    public float SetBaseValue(float newValue)
    {
        newValue = Mathf.Clamp(newValue, minValue, maxValue);
        if (newValue == baseValue)
            return baseValue;

        baseValue = newValue;
        OnBaseValueChanged.Invoke(baseValue);
        return baseValue;
    }

    public float SetValueToBase()
    {
        return SetValue(baseValue);
    }

    public float GetMinValue()
    {
        return minValue;
    }

    public float RaiseMinValue(float amountToAdd)
    {
        return SetMinValue(minValue + amountToAdd);
    }

    public float ReduceMinValue(float amountToRemove)
    {
        return SetMinValue(minValue - amountToRemove);
    }

    public float SetMinValue(float newMinValue)
    {
        if (minValue == newMinValue)
            return minValue;

        minValue = newMinValue;
        OnMinValueChanged.Invoke(minValue);

        if (GetValue() < minValue)
            SetValue(minValue);

        return minValue;
    }

    public float GetMaxValue()
    { 
        return maxValue;
    }

    public float RaiseMaxValue(float amountToAdd)
    {
        return SetMaxValue(maxValue + amountToAdd);
    }

    public float ReduceMaxValue(float amountToRemove)
    {
        return SetMaxValue(maxValue - amountToRemove);
    }

    public float SetMaxValue(float newMaxValue)
    {
        if (maxValue == newMaxValue)
            return maxValue;

        maxValue = newMaxValue;
        OnMaxValueChanged.Invoke(maxValue);

        if (GetValue() > maxValue)
            SetValue(maxValue);

        return maxValue;
    }

    public bool HasBase()
    {
        return hasBase;
    }
}
