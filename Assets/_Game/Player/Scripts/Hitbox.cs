﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [Tooltip("The knockback applied to whatever you hit. X should be positive if you're using player facing")]
    [SerializeField] Vector3 knockback;
    [SerializeField] int damage = 2; //the amount of damage you do to enemies
    
    PlayerMovement player;
    [Tooltip("If true, knockback direction is determined by player facing")]
    [SerializeField] bool usePlayerFacing = true;
    private void Awake()
    {
        if (usePlayerFacing)
        {
            player = GetComponentInParent<PlayerMovement>();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Object Detected from Hitbox");
        //if it's an enemy
        if (collision.gameObject.layer == 11 && collision.CompareTag("Enemy"))
        {
            //Debug.Log("Enemy Hit: " + collision.gameObject.name);
            EnemyStats enemy = collision.gameObject.GetComponentInParent<EnemyStats>();
            if (enemy != null)
            {
                Vector3 knockbackTemp = knockback;
                if (usePlayerFacing)
                {
                    knockbackTemp.x = (player.facingRight) ? knockback.x : -knockback.x;
                }
                enemy.LogDamage(damage, knockbackTemp);
            }
        }

        else if (collision.gameObject.layer == 12)
        {
            //Debug.Log("Door Detected");
            Door door = collision.gameObject.GetComponentInParent<Door>();
            if (door != null)
            {
                //Debug.Log("Opening Door");
                door.Open(false);
            }
            //else      
            //{
            //    //Debug.Log("Door is null");
            //}
        }
        //if it's a breakable wall
        else if (collision.gameObject.layer == 18)
        {
            BreakableWall breakableWall = collision.gameObject.GetComponent<BreakableWall>();
            if (breakableWall != null)
            {
                breakableWall.BreakWall();
            }
        }
    }
}
