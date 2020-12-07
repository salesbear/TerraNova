using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] int maxHp = 4;
    [ReadOnly]
    [SerializeField] int hp = 4;
    //the max amount of damage they've taken this frame. Needed for when the player overlaps the enemy with multiple hitboxes
    int damageTaken;
    //used to pass knockback to the TakeDamage function
    Vector3 knockbackTaken;

    [SerializeField] SpriteRenderer sprite;
    Color initialColor;
    [Tooltip("The color that the enemy is tinted by when damaged")]
    [SerializeField] Color damageColor;
    [Tooltip("The amount of time before they can be damaged again")]
    [SerializeField] float invincibilityTime = 0.15f;
    [SerializeField] AudioClip hurtClip;
    [Tooltip("The percent chance that the enemy drops something on death")]
    [Range(0,100)]
    [SerializeField] int dropChance = 30;
    float invincibilityTimer = 0f;
    [SerializeField]
    EnemyMovement enemy;
    [SerializeField]
    EnemySpawner enemySpawner;

    bool invincible { get { return invincibilityTimer > 0; } }
    

    private void Awake()
    {
        initialColor = sprite.color;
        if (sprite == null)
        {
            sprite = GetComponent<SpriteRenderer>();
        }
        if (enemy == null)
        {
            enemy = GetComponent<EnemyMovement>();
        }
        if (enemySpawner == null)
        {
            enemySpawner = GetComponentInParent<EnemySpawner>();
        }
    }

    private void OnEnable()
    {
        sprite.color = initialColor;
        hp = maxHp;
        invincibilityTimer = 0;
    }

    private void Update()
    {
        if (damageTaken > 0)
        {
            TakeDamage(damageTaken, knockbackTaken);
            damageTaken = 0;
        }

        if (invincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0)
            {
                sprite.color = initialColor;
            }
        }
    }

    void TakeDamage(int amount, Vector3 knockbackVector)
    {
        if (!invincible)
        {
            hp -= amount;
            sprite.color = damageColor;
            invincibilityTimer = invincibilityTime;
            AudioManager.instance.PlaySound(hurtClip, 0.8f);
            if (enemy != null)
            {
                enemy.TakeKnockback(knockbackVector);
            }
        }

        if (hp <= 0)
        {
            Die();
        }
    }

    //TODO: implement death animation
    void Die()
    {
        GameObject drop = DropManager.instance.Drop(dropChance);
        //if there's a drop, drop it
        if (drop != null)
        {
            Instantiate(drop, transform.position, transform.rotation);
        }
        enemySpawner.Despawn();
    }

    /// <summary>
    /// log the amount of damage that the enemy takes, they take less damage if the player doesn't hit them with the sweet spot
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="knockbackVector"></param>
    public void LogDamage(int amount, Vector3 knockbackVector)
    {
        if (damageTaken == 0)
        {
            damageTaken = amount;
            knockbackTaken = knockbackVector;
        }
        else if (amount < damageTaken)
        {
            damageTaken = amount;
            knockbackTaken = knockbackVector;
        }
    }
}
