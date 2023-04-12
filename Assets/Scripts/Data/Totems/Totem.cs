using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Totem", menuName = "new Totem")]
public class Totem : ScriptableObject
{
    public Sprite totemSprite;
    public Character caster;

    public TotemInstance totemInstance;

    public bool eventActivated;
    public bool awakeApplyToCasterOnly;
    public bool normalApplyToCasterOnly;
    public bool destroyApplyToCasterOnly;

    public int EventsUntilEffect;
    public int durationOfTurns;
    public List<Effect> totemAwakeEffects;
    public List<Effect> totemNormalEffects;
    public List<Effect> totemDestroyEffects;
}
