using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectInstance
{
    public Effect effect;
    public int turnsRemaining;
    public bool hasTurnLimit;

    public GameObject activeParticleGameObject;
    public ParticleSystem curTickParticle;

    public bool applyOnDestroy;
    public bool effectApplied = false;

    public int speedMod;
    public float spellMod;
    public float damageMod;
    public float manaRegen;

    public EffectInstance(Effect effect)
    {
        this.effect = effect;
        turnsRemaining = effect.durationOfTurns;
        hasTurnLimit = effect.hasTurnLimit;
        applyOnDestroy = effect.applyOnDestroy;

        speedMod = effect.speedMod;
        damageMod = effect.damageMod;
        speedMod= effect.speedMod;
        manaRegen= effect.manaRegen;
    }
}
