using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropManager : MonoBehaviour
{
    public static DropManager instance;
    [Tooltip("the player attributes script from the player, will find it if left null")]
    [SerializeField] PlayerAttributes player;
    [Tooltip("the health pickup prefab")]
    [SerializeField] GameObject healthPickup;
    [Tooltip("the mana pickup prefab")]
    [SerializeField] GameObject manaPickup;
    [Tooltip("when the player is missing health and mana, this is the percent chance that they to get health back")]
    [Range(0,100)]
    [SerializeField] int healthWeight = 60;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }

        if (player == null)
        {
            player = FindObjectOfType<PlayerAttributes>();
        }
    }
    /// <summary>
    /// given a drop chance between 0 and 100, the function will return
    /// the prefab of a drop based on what the player currently has missing
    /// </summary>
    /// <param name="dropChance"></param>
    /// <returns></returns>
    public GameObject Drop(int dropChance)
    {
        //if there's a drop, determine which thing to drop
        if (Random.Range(0,100) < dropChance)
        {
            if (player.health != player.maxHealth)
            {
                if (player.mana == player.maxMana)
                {
                    return healthPickup;
                }
                //if we're missing health and mana, choose which one to drop based on our health weight variable
                if (Random.Range(0,100) < healthWeight)
                {
                    return healthPickup;
                }
                return manaPickup;
            }
            //if we're missing mana but not health, drop mana
            else if (player.mana != player.maxMana)
            {
                return manaPickup;
            }
            //if we're full on both, randomly select one of them
            else
            {
                if (Random.Range(0,100) < 50)
                {
                    return healthPickup;
                }
                return manaPickup;
            }
        }
        //if there isn't a drop, return null to show that
        return null;
    }
}
