﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    
    [SerializeField] int hp = 3;
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
    AudioSource audioSource;
    float invincibilityTimer = 0f;

    bool invincible { get { return invincibilityTimer > 0; } }

    private void Awake()
    {
        if (sprite == null)
        {
            sprite = GetComponent<SpriteRenderer>();
        }
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        initialColor = sprite.color;
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
        //TODO: implement enemy knockback, display enemy damage
        if (!invincible)
        {
            hp -= amount;
            sprite.color = damageColor;
            invincibilityTimer = invincibilityTime;
            AudioSource.PlayClipAtPoint(hurtClip, transform.position);
        }

        if (hp <= 0)
        {
            Die();
        }
    }

    //TODO: implement death animation, drop system
    void Die()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// log the amount of damage that the enemy takes, they take less damage if the player doesn't hit them
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="knockbackVector"></param>
    public void LogDamage(int amount, Vector3 knockbackVector)
    {
        if (damageTaken == 0)
        {
            damageTaken = amount;
        }
        else
        {
            damageTaken = Mathf.Min(damageTaken, amount);
        }
        knockbackTaken = knockbackVector;
    }
}