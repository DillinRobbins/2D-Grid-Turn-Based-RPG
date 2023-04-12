using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TotemInstance : MonoBehaviour
{
    [SerializeField] Totem totem;
    public Sprite totemSprite;

    public Image image;

    public bool eventActivated;

    public int EventsUntilEffect;
    public int turnsRemaining;
    public List<Effect> totemEffects;
    public List<Effect> totemAwakeEffects;
    public List<Effect> totemDestroyEffects;

    public bool awakeApplyToCasterOnly;
    public bool normalApplyToCasterOnly;
    public bool destroyApplyToCasterOnly;
    public Character caster;
    public GameObject clone;

    protected virtual void Awake()
    {
        totemSprite = totem.totemSprite;
        eventActivated = totem.eventActivated;
        totemEffects = totem.totemNormalEffects;
        totemAwakeEffects = totem.totemAwakeEffects;
        totemDestroyEffects = totem.totemDestroyEffects;

        awakeApplyToCasterOnly = totem.awakeApplyToCasterOnly;
        normalApplyToCasterOnly = totem.normalApplyToCasterOnly;
        destroyApplyToCasterOnly = totem.destroyApplyToCasterOnly;

        if(eventActivated)
        {
            EventsUntilEffect = totem.EventsUntilEffect;
        }
        else
        {
            turnsRemaining = totem.durationOfTurns + 1;
        }

        Initialize();
    }

    protected virtual void Initialize()
    {
        if(totemAwakeEffects.Count > 0)
        {
            TotemManager.Instance.ApplyTotemEffects(this, totemAwakeEffects, caster, awakeApplyToCasterOnly);
        }
    }

    public virtual void DestructionSequence()
    {
        if (totemDestroyEffects.Count > 0)
        {
            TotemManager.Instance.ApplyTotemEffects(this, totemDestroyEffects, caster, destroyApplyToCasterOnly);
        }

        Destroy(clone);
    }

    public virtual void ApplyCustomEffects()
    {

    }
}
