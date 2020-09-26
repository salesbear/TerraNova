using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    
    [SerializeField] int hp = 3;
    //the max amount of damage they've taken this frame. Needed for when the player overlaps the enemy with multiple hitboxes
    int maxDamage;
    //used to pass knockback to the TakeDamage function
    Vector3 knockbackTaken;

    private void Update()
    {
        if (maxDamage > 0)
        {
            TakeDamage(maxDamage, knockbackTaken);
            maxDamage = 0;
        }
    }

    void TakeDamage(int amount, Vector3 knockbackVector)
    {
        //TODO: implement enemy knockback, display enemy damage
        hp -= amount;
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

    public void LogDamage(int amount, Vector3 knockbackVector)
    {
        maxDamage = Mathf.Max(maxDamage, amount);
        knockbackTaken = knockbackVector;
    }
}
