using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaGyre : TotemInstance
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    public override void DestructionSequence()
    {
        base.DestructionSequence();
    }

    public override void ApplyCustomEffects()
    {
        var spellMod = caster.stats.Find(s => s.statType == CharacterStatType.SpellModifier);
        spellMod.RaiseValue(spellMod.GetValue() * .1f);
    }
}
