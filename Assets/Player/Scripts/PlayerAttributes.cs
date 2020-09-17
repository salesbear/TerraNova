using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttributes : MonoBehaviour
{
    public int health { get; private set; }
    public int maxHealth { get; private set; }
    public int mana { get; private set; }
    public int maxMana { get; private set; }

    //PlayerMovement1 player;
    PlayerStateController ps_controller;

    // Start is called before the first frame update
    void Start()
    {
        ps_controller = GetComponent<PlayerStateController>();
        //TODO: Load Max Health instead of setting it.
        SetMaxHealth(6);
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Sets max health to value given or 1, whichever's higher
    /// </summary>
    /// <param name="value"></param>
    public void SetMaxHealth(int value)
    {
        maxHealth = Mathf.Max(value,1);
    }

    /// <summary>
    /// increases max health by a set increment, equal to 1 heart on the UI
    /// </summary>
    public void IncreaseMaxHealth()
    {
        maxHealth += 2;
        health = maxHealth;
    }
    /// <summary>
    /// sets Max Mana to value, or zero if value is negative
    /// </summary>
    /// <param name="value"></param>
    public void SetMaxMana(int value)
    {
        maxMana = Mathf.Max(value, 0);
        mana = maxMana;
    }

    public void TakeDamage(int amount)
    {
        Debug.Log("Take Damage Called");
        health -= amount;
        Debug.Log("Current Health: " + health);
        if (health <= 0)
        {
            Die();
            Debug.Log("You Dead");
        }
    }

    public void Die()
    {
        // TODO: play death animation
        ps_controller.ChangeState(PlayerState.Dead);
        // TODO: put up UI telling player how to restart from last checkpoint
    }
}
