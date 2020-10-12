using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerAttributes : MonoBehaviour
{
    [Header("Health and Mana")]
    [Tooltip("the player's max health points")]
    public int maxHealth = 6;
    [ReadOnly]
    [Tooltip("Your current health")]
    public int health;
    [Tooltip("the player's max mana points")]
    public int maxMana = 10;
    public int mana { get; private set; }
    public bool hasMana { get { return mana > 0; } }

    [Header("Abilities")]
    [Tooltip("controls if the player can fire a fireball")]
    public bool fireUnlocked = true;
    [Tooltip("controls if the player can walljump")]
    public bool wallJumpUnlocked = true;

    [Header("Audio")]
    [Tooltip("The sound that plays when the player regains health")]
    [SerializeField] private AudioClip healSound;
    [Tooltip("The sound that plays when the player regains mana")]
    [SerializeField] private AudioClip manaRegenSound;
    [Tooltip("The sound that plays when the player gets hurt")]
    [SerializeField] private AudioClip hurtSound;
    [Tooltip("The sound that plays when the player dies")]
    [SerializeField] private AudioClip deathSound;
    [Tooltip("the range for randomizing the pitch")]
    [SerializeField] private Vector2 pitchRange = new Vector2(-0.1f,0.1f);
    
    [Header("Debug")]
    [Tooltip("Allows you to increase your health, max health, mana, and max mana at will")]
    [SerializeField] bool debugMode = true;

    public static event Action<int> MaxHealthChanged = delegate { };

    //PlayerMovement1 player;
    PlayerStateController ps_controller;

    // Start is called before the first frame update
    void Start()
    {
        ps_controller = GetComponent<PlayerStateController>();
        //TODO: Load Max Health and mana instead of setting it.
        SetMaxHealth(maxHealth);
        SetMaxMana(maxMana);
    }

    // Update is called once per frame
    void Update()
    {
        //key codes to allow you to arbitrarily change health and mana in debug mode
        if (debugMode)
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                TakeDamage(1);
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                IncreaseHealth(1);
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                ReduceMana(1);
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                IncreaseMana(1);
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                IncreaseHealth(maxHealth);
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                IncreaseMana(maxMana);
            }
            if (Input.GetKeyDown(KeyCode.PageDown))
            {
                IncreaseMaxHealth();
            }
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                SetMaxHealth(7);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //TODO: Change this to open up a menu, also probably put this somewhere else
            Application.Quit();
        }
    }

    /// <summary>
    /// Sets max health to value given or 1, whichever's higher
    /// </summary>
    /// <param name="value"></param>
    public void SetMaxHealth(int value)
    {
        maxHealth = Mathf.Max(value,1);
        health = maxHealth;
        MaxHealthChanged.Invoke(maxHealth);
    }

    /// <summary>
    /// increases max health by a set increment, equal to 1 heart on the UI
    /// </summary>
    public void IncreaseMaxHealth()
    {
        maxHealth += 2;
        health = maxHealth;
        MaxHealthChanged.Invoke(maxHealth);
    }

    /// <summary>
    /// increases your health by the amount given, won't increase past max
    /// </summary>
    /// <param name="amount"></param>
    public void IncreaseHealth(int amount)
    {
        health = Mathf.Min(health + amount, maxHealth);
        AudioManager.instance.PlaySound(healSound, pitchRange);
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

    /// <summary>
    /// reduces mana by amount and returns true 
    /// if the player has enough mana 
    /// to make that reduction
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public bool ReduceMana(int amount)
    {
        mana -= amount;
        if (mana < 0)
        {
            mana = 0;
            return false;
        }
        return true;
    }

    /// <summary>
    /// increases mana by amount, bounded by maxMana
    /// </summary>
    /// <param name="amount"></param>
    public void IncreaseMana(int amount)
    {
        mana = Mathf.Min(mana + amount, maxMana);
        AudioManager.instance.PlaySound(manaRegenSound, pitchRange);
    }

    /// <summary>
    /// reduces health by amount and kills the player if it's less than or equal to 0
    /// </summary>
    /// <param name="amount"></param>
    public void TakeDamage(int amount)
    {
        //Debug.Log("Take Damage Called");
        health -= amount;
        //Debug.Log("Current Health: " + health);
        if (health <= 0 && ps_controller.state != PlayerState.Dead)
        {
            Die();
            //Debug.Log("You Dead");
        }
        else if (ps_controller.state != PlayerState.Dead)
        {
            AudioManager.instance.PlaySound(hurtSound, pitchRange);
        }
    }

    public void Die()
    {
        // TODO: play death animation (in animation controller script)
        AudioManager.instance.PlaySound(deathSound, pitchRange);
        ps_controller.ChangeState(PlayerState.Dead);
        // TODO: put up UI telling player how to restart
    }
}
