using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DamageVolume : MonoBehaviour
{
    public int damage = 1;
    [SerializeField] int enemyDamage = 3;
    [SerializeField] bool affectsEnemies = true;
    [SerializeField] float knockbackSpeed = 4f;
    [SerializeField] float yOffset = 2f;

    private void OnTriggerStay2D(Collider2D collision)
    {
        //Debug.Log("Object Detected");
        
        if (collision.gameObject.layer == 10 && !collision.CompareTag("Hitbox"))
        {
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
            Vector3 knockbackVector = new Vector3();
            if (transform.position.x >= collision.gameObject.transform.position.x)
            {
                knockbackVector.x = -1;
            }
            else
            {
                knockbackVector.x = 1;
            }
            knockbackVector.y = yOffset;
            player.TakeDamage(damage, knockbackVector * knockbackSpeed);
        }
        if (affectsEnemies && collision.gameObject.layer == 11 && collision.CompareTag("Enemy"))
        {
            EnemyStats enemy = collision.gameObject.GetComponent<EnemyStats>();
            Vector3 knockbackVector = new Vector3();
            if (transform.position.x >= collision.gameObject.transform.position.x)
            {
                knockbackVector.x = -1;
            }
            else
            {
                knockbackVector.x = 1;
            }
            knockbackVector.y = yOffset;
            enemy.LogDamage(enemyDamage, knockbackVector * knockbackSpeed);
        }
    }
}
