using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private float speed = 1200;

    private Character target;
    private Character attackingCharacter;
    private Effect effectToApply;

    public void Initialize(Character projectileTarget, int damageToTake, Effect effect, Character character)
    {
        target = projectileTarget;
        damage = damageToTake;
        effectToApply = effect;
        attackingCharacter = character;
    }

    private void FixedUpdate()
    {
        if(target.canvasGroup.alpha == 0)
            Destroy(gameObject);
    }

    private void Update()
    {
        if(target == null)
        {
            Destroy(gameObject);
        }

        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);

        if(Vector3.Distance(transform.position, target.transform.position) < .01f && Vector3.Distance(transform.position, target.transform.position) > -.01f)
        {
            if(target.canvasGroup.alpha != 0)
            {
                target.TakeDamage(damage);
                CharacterManager.Instance.IncrementAttacks();
            }

            if (effectToApply != null)
            {
                target.GetComponent<CharacterEffects>().AddNewEffect(effectToApply);
                CharacterManager.Instance.IncrementEffects();
            }

            attackingCharacter.InvokeTurnExecutedEvent();

            Destroy(gameObject);
        }
    }

    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;

        if(target.col == collision)
        {
            collision.GetComponent<Character>().TakeDamage(damage);

            if(effectToApply != null)
                collision.GetComponent<CharacterEffects>().AddNewEffect(effectToApply);

            Destroy(gameObject);
        }
    }*/
}
