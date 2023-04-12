using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    public string characterName;
    [SerializeField] private CharacterType characterType;
    public Image image;
    //public Collider2D col;
    public CanvasGroup canvasGroup;
    private PlayerCell cell;
    public bool isPlayer;

    public List<CombatAction> CombatActions = new();
    public CombatAction DefenseAction;

    [SerializeField] private List<Character> targets;

    private Vector3 startPos;

    public event UnityAction<Character> OnCharacterInitialized;
    public event UnityAction OnHealthChange;
    public event UnityAction OnManaChange;
    public static event UnityAction OnPlayerCombatActionChange;
    public static event UnityAction OnSpeedChange;
    public static event UnityAction<Character> OnDie;
    public static event UnityAction<Character> OnPlayerDie;
    public static event UnityAction OnPlayerSelectTargets;
    public static event UnityAction OnEnemyCombatActionSelected;
    public CombatAction CurrentCombatAction;
    public static event UnityAction OnTurnExecuted;

    public CharacterEffects characterEffects;

    private CharacterStat HP, MP, Speed, DamageModifier, Defense, SpellModifier;
    public List<CharacterStat> stats;

    private void Awake()
    {
        HP = gameObject.AddComponent<CharacterStat>();
        Speed = gameObject.AddComponent<CharacterStat>();
        DamageModifier = gameObject.AddComponent<CharacterStat>();
        Defense = gameObject.AddComponent<CharacterStat>();
        SpellModifier = gameObject.AddComponent<CharacterStat>();

        //For now stats are set to their Character Type data. When persistent data is introduced, needs to be extended)
        if (isPlayer)
        {
            MP = gameObject.AddComponent<CharacterStat>();

            HP.StatConstructorNoBase(CharacterStatType.HP, 0, characterType.HealthAmount, true);
            MP.StatConstructorNoBase(CharacterStatType.MP, 0, 100, true);
            Speed.StatConstructor(CharacterStatType.Speed, characterType.speed, 0, 200, characterType.speed);
            DamageModifier.StatConstructor(CharacterStatType.DamageModifier, 1f, 0.5f, 2f, 1f);
            SpellModifier.StatConstructor(CharacterStatType.SpellModifier, 1f, 0.5f, 2f, 1f);
            Defense.StatConstructor(CharacterStatType.Defense, 1, 0, 2, 1);
        }

        characterEffects = gameObject.GetComponent<CharacterEffects>();
        //col = GetComponent<Collider2D>();
        stats = GetComponents<CharacterStat>()?.ToList();
    }

    void OnEnable()
    {
        CharacterManager.Instance.AddCharacterToList(this);

        if(isPlayer)
            cell = GetComponentInParent<PlayerCell>();
    }

    public void Initialize(CharacterType characterType)
    {
        image = gameObject.GetComponent<Image>();
        if (!isPlayer)
        {
            this.characterType = characterType;
            CombatActions = characterType.typeCombatActions;
            image.sprite = characterType.characterSprite;

            HP.StatConstructorNoBase(CharacterStatType.HP, 0, characterType.HealthAmount, true);
            Speed.StatConstructor(CharacterStatType.Speed, characterType.speed, 0, 200, characterType.speed);
            DamageModifier.StatConstructor(CharacterStatType.DamageModifier, 1f, 0.5f, 2f, 1f);
            Defense.StatConstructor(CharacterStatType.Defense, 1, 0, 2, 1);
        }
        OnCharacterInitialized?.Invoke(this);
    }

    public void TakeDamage(int damageToTake)
    {
        /*var health = GetStat(CharacterStatType.HP);
        health.ReduceValue(damageToTake);*/
            if (Defense.GetValue() > 1 && (int)(damageToTake / Defense.GetValue()) == damageToTake)
                HP.ReduceValue(damageToTake - 1);
            else
                HP.ReduceValue((int)(damageToTake / Defense.GetValue()));

        OnHealthChange?.Invoke();

        if (HP.GetValue() == 0)
        {
            Die();
        }
    }

    public void SetTargets(List<Character> characters)
    {
        targets.Clear();
        for(int i = 0; i < characters.Count; i++)
        {
            targets.Add(characters[i]);
        }
        if(isPlayer) OnPlayerSelectTargets?.Invoke();
    }

    public void ClearTargets()
    {
        targets.Clear();
    }

    public CombatAction GetCombatAction()
    {
        return CurrentCombatAction;
    }

    public CharacterType GetCharacterType()
    {
        return characterType;
    }

    public CharacterType.Race GetCharacterRace()
    {
        return characterType.RaceofCharacter;
    }

    //public

    void Die()
    {
        StopAllCoroutines();
        PrepareDeath();
        OnDie?.Invoke(this);
        if (isPlayer)
            OnPlayerDie?.Invoke(this);
    }

    public void Heal(int healAmount)
    {
        HP.RaiseValue(healAmount);

        OnHealthChange?.Invoke();
    }

    public void UpdateSpeed(float speedMod)
    {
        Speed.SetValue(speedMod + Speed.GetValue());

        if(isPlayer)
            OnSpeedChange?.Invoke();
    }

    public void ResetStats()
    {
        for (int i = 0; i < stats.Count; i++)
        {
            if (!stats[i].HasBase())
                continue;

            var baseValue = stats[i].GetBaseValue();
            var value = stats[i].GetValue();
            if (value == baseValue)
                continue;

            stats[i].SetValueToBase();
        }
    }

    public void ChangeCurrentCombatAction(CombatAction combataction)
    {
        CurrentCombatAction = combataction;
        if(combataction != null)
        UpdateSpeed(combataction.SpeedModifier);

        if (isPlayer)
            OnPlayerCombatActionChange?.Invoke();
        else
            OnEnemyCombatActionSelected?.Invoke();
                
    }

    public void CastCombatAction()
    {
        if (this.canvasGroup.alpha == 0)
        {
            Debug.Log("Character is Dead");
            InvokeTurnExecutedEvent();
            return;
        }
            
        if(CurrentCombatAction == null)
        {
            Debug.Log("No Combat Action");
            InvokeTurnExecutedEvent();
            return;
        }

        if(targets.Count != 0)
        {
            if(CurrentCombatAction.SpellDamage > 0)
            {
                if(isPlayer && MP.GetValue() - CurrentCombatAction.ManaCost >= 0)
                {
                    MP.ReduceValue(CurrentCombatAction.ManaCost);
                    this.OnManaChange?.Invoke();
                    StartCoroutine(CastSpell(CurrentCombatAction));
                }
                else
                {
                    StartCoroutine(CastSpell(CurrentCombatAction));
                }
            }
            else if (CurrentCombatAction.Damage > 0)
            {
                StartCoroutine(PhysicalAttack(CurrentCombatAction));
            }
            else if (CurrentCombatAction.HealAmount > 0)
            {
                Heal(CurrentCombatAction.HealAmount);
            }
            else if(CurrentCombatAction.ActionType == CombatAction.Type.Defense)
            {
                Defense.RaiseValue(CurrentCombatAction.DefenseAmount);
                InvokeTurnExecutedEvent();
            }
        }
        else if(CurrentCombatAction.ActionType == CombatAction.Type.Totem)
        {
            TotemManager.Instance.AddNewTotem(CurrentCombatAction.totem, cell);
            InvokeTurnExecutedEvent();
        } 
        else
        {
            InvokeTurnExecutedEvent();
        }
            
    }
    
    IEnumerator CastSpell(CombatAction combatAction)
    {
        yield return new WaitForSeconds(.2f);
        startPos = transform.position;

        foreach(var target in targets)
        {
            if(target.canvasGroup.alpha != 0)
            {
                var proj = Instantiate(combatAction.projectilePrefab, startPos + new Vector3(55, 50, 0), Quaternion.identity, this.gameObject.transform) as GameObject;
                proj.GetComponent<Projectile>().Initialize(target, FinalDamage(), CurrentCombatAction.effect, this);
            }
            yield return null;
        }
    }

    IEnumerator PhysicalAttack(CombatAction combatAction)
    {
        yield return new WaitForSeconds(.2f);
        if(isPlayer)
            startPos = cell.transform.position;
        else
            startPos = transform.position;

        if (!targets.All(c => c == null))
        {
            while (Vector3.Distance(transform.position, AttackPoint()) > .01f || Vector3.Distance(transform.position, AttackPoint()) < -.01f)
            {
                if (targets.All(c => c.canvasGroup.alpha == 0)) break;

                transform.position = Vector3.MoveTowards(transform.position, AttackPoint(), 1600 * Time.deltaTime);
                yield return null;
            }

            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i].canvasGroup.alpha != 0)
                {
                    targets[i].TakeDamage(FinalDamage());
                    CharacterManager.Instance.IncrementAttacks();
                }
                else
                    Debug.Log("Could Not complete Attack");

                if (combatAction.effect != null)
                {
                    targets[i].characterEffects.AddNewEffect(combatAction.effect);
                    CharacterManager.Instance.IncrementEffects();
                }
                    
                yield return null;
            }

            while (Vector3.Distance(transform.position, startPos) > .01f || Vector3.Distance(transform.position, startPos) < -.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, startPos, 1200 * Time.deltaTime);
                yield return null;
            }
        }

        InvokeTurnExecutedEvent();
    }

    public void InvokeTurnExecutedEvent()
    {
        OnTurnExecuted?.Invoke();
    }

    public void PrepareDeath()
    {
        canvasGroup.alpha = 0;
    }

    //Helper Functions
    private int FinalDamage()
    {
        int damage = 0;
        //Extend if more damage types needed
        if(CurrentCombatAction.SpellDamage > 0)
            damage = (int)(CurrentCombatAction.SpellDamage * SpellModifier.GetValue());
        else if(CurrentCombatAction.Damage > 0)
            damage = (int)(CurrentCombatAction.Damage * DamageModifier.GetValue());

        return damage;
    }

    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;
        else if (isPlayer && collision.CompareTag("Player")) return;
        else if (!isPlayer && collision.CompareTag("Enemy")) return;

        if (targets.Any(c => c.col == collision))
        {
            collision.GetComponent<Character>().TakeDamage(FinalDamage());
            if (CurrentCombatAction.effect != null)
                collision.GetComponent<CharacterEffects>().AddNewEffect(CurrentCombatAction.effect);
        }
    }*/

    private Vector2 AttackPoint()
    {
        int xTotal = 0;
        int yTotal = 0;

        for(int i = 0; i < targets.Count; i++)
        {
            xTotal += (int)targets[i].transform.position.x;
            yTotal += (int)targets[i].transform.position.y;
        }
        xTotal /= targets.Count;
        yTotal /= targets.Count;

        Vector2 attackPoint = new(xTotal, yTotal);
        return attackPoint;
    }

    public float GetHealthPercentage()
    {
        return HP.GetValue()/HP.GetMaxValue();
    }

    public float GetManaPercentage()
    {
        return MP.GetValue()/MP.GetMaxValue();
    }

    public int GetTargets()
    {
        return targets.Count;
    }

    public List<Character> GetTargetList()
    {
        return targets;
    }

    public List<CharacterType> GetTargetTypeList()
    {
        List<CharacterType> targetTypeList = new();

        foreach (Character character in targets)
        {
            targetTypeList.Add(character.characterType);
        }
        targetTypeList = targetTypeList.Distinct().ToList();

        return targetTypeList;
    }

    public PlayerCell GetPlayerCell()
    {
        return cell;
    }

    public CharacterStat GetStat(CharacterStatType statType)
    {
        return GetStat(statType.ToString());
    }

    public CharacterStat GetStat(string statName)
    {
        return stats?.FirstOrDefault(stat => stat.statName == statName);
    }
}
