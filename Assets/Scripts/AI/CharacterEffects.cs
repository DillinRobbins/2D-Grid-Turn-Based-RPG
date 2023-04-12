using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Globalization;

public class CharacterEffects : MonoBehaviour
{
    public int numberOfEffects = 0;
    private List<EffectInstance> curEffects = new();
    [SerializeField] List<EffectIcon> curIcons = new();

    private List<EffectIcon> iconsToRemove = new();
    private List<EffectInstance> effectsToRemove = new();

    [SerializeField] private GameObject effectIconDisplay;
    [SerializeField] private GameObject effectIconPrefab;

    private Character character;

    public bool destroyedEffect = false;

    private void Awake()
    {
        character = gameObject.GetComponent<Character>();
    }

    public void AddNewEffect(Effect effect)
    {
        if (!effect.isStackable && curEffects.Any(e => e.effect == effect))
        {
            Debug.Log("effect cancelled");
            return;
        }
            
        //Should i be using typeof here? Otherwise it will be looking for an exact effect right?
        //EffectType was a bandaid to work around having to have an exact match to an effect

        EffectInstance effectInstance = new(effect);

        if (effect.activePrefab != null)
            effectInstance.activeParticleGameObject = Instantiate(effect.activePrefab, transform);

        if (effect.tickPrefab != null)
            effectInstance.curTickParticle = Instantiate(effect.tickPrefab, transform).GetComponent<ParticleSystem>();

        curEffects.Add(effectInstance);
        numberOfEffects++;

        if (effect.applyOnSameTurn)
            ApplyEffect(effectInstance);
    }

    public void ApplyCurrentEffects()
    {
        if(curEffects.Count > 0)
        {
            for (int i = 0; i < curEffects.Count; i++)
            {
                ApplyEffect(curEffects[i]);

                if (!curEffects[i].effect.applyOnSameTurn)
                    AddIcon(curEffects[i].effect);
            }

        }
    }

    void ApplyEffect(EffectInstance effectInstance)
    {
        //effectInstance.curTickParticle.Play();

        if (!effectInstance.effectApplied)
        {
            AddIcon(effectInstance.effect);
            effectInstance.effectApplied = true;
        }

        if (effectInstance.speedMod != 0)
        {
            character.UpdateSpeed(effectInstance.effect.speedMod);
        }
        else if(effectInstance.damageMod != 0)
        {
            var damageModStat = character.stats.Find(s => s.statType == CharacterStatType.DamageModifier);
            damageModStat.RaiseValue(effectInstance.effect.damageMod);
        }
        else if(effectInstance.manaRegen != 0)
        {
            var manaRegenStat = character.stats.Find(s => s.statType == CharacterStatType.MP);
            manaRegenStat.RaiseValue(manaRegenStat.GetMaxValue() * effectInstance.manaRegen);
        }

        if (effectInstance.hasTurnLimit && destroyedEffect == false)
        {
            effectInstance.turnsRemaining--;
            if (effectInstance.turnsRemaining <= 0)
                RemoveEffect(effectInstance);
        }
        else if(destroyedEffect == true)
            destroyedEffect = false;
    }

    void RemoveEffect(EffectInstance effectInstance)
    {
        if(effectInstance.activeParticleGameObject != null)
            Destroy(effectInstance.activeParticleGameObject);

        if(effectInstance.curTickParticle != null)
            Destroy(effectInstance.curTickParticle.gameObject);

        if (effectInstance.applyOnDestroy)
        {
            destroyedEffect = true;
            ApplyEffect(effectInstance);
        }

        effectsToRemove.Add(effectInstance);
    }

    private void AddIcon(Effect effect)
    {
        EffectIcon existingIcon = curIcons.Find(i => i.effect.effectType == effect.effectType);
        if (existingIcon != null)
            existingIcon.AddCount();
        else
        {
            var icon = Instantiate(effectIconPrefab, effectIconDisplay.transform);
            var effectIcon = icon.GetComponent<EffectIcon>();
            effectIcon.Initialize(effect);
            curIcons.Add(effectIcon);
        }
    }

    public void RemoveExhaustedEffectIcons()
    {
            for(int i = 0; i < curIcons.Count; i++)
            {
                var effectList = curEffects.FindAll(e => e.effect.effectType == curIcons[i].effect.effectType).ToList();

                if (effectList.Count == 0)
                {
                    iconsToRemove.Add(curIcons[i]);
                }
                    
                else if (curIcons[i].stacksCount < effectList.Count)
                    curIcons[i].RemoveCount(effectList.Count - curIcons[i].stacksCount);
            }

            DestroyIconsToRemove();
    }

    void DestroyIconsToRemove()
    {
        foreach (var icon in iconsToRemove)
        {
            curIcons.RemoveAll(c => c.effect == icon.effect);
        }

        for (int x = iconsToRemove.Count - 1; x >= 0; x--)
        {
            Destroy(iconsToRemove[x].gameObject);
        }

        iconsToRemove.Clear();
    }

    public void DestroyExhaustedEffects()
    {
        foreach(var effectInstance in effectsToRemove)
        {
            curEffects.RemoveAll(e => e.effect == effectInstance.effect);
        }

        numberOfEffects -= effectsToRemove.Count;

        effectsToRemove.Clear();
    }

    public void SetEffectIconDisplay(GameObject iconDisplay)
    {
        effectIconDisplay = iconDisplay;
    }
}
