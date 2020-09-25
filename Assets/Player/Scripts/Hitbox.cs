using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] Vector3 knockback;
    [SerializeField] int damage = 2; //the amount of damage you do to enemies
    bool hit_enemy = false;
    //[SerializeField] bool sweetSpot; //do more damage if you hit with the head of the hammer

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Object Detected from Hitbox");
        //if it's an enemy
        if (collision.gameObject.layer == 11 && !hit_enemy)
        {
            //Debug.Log("Enemy Hit: " + collision.gameObject.name);
            hit_enemy = true;
            EnemyStats enemy = collision.gameObject.GetComponent<EnemyStats>();
            if (enemy != null)
            {
                enemy.LogDamage(damage, knockback);
            }
        }
    }

    private void OnDisable()
    {
        hit_enemy = false;
    }
}
