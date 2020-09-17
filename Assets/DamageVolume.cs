using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DamageVolume : MonoBehaviour
{
    public int damage = 1;
    public Vector3 knockbackVector;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10)
        {
            PlayerMovement1 player = collision.gameObject.GetComponent<PlayerMovement1>();
            player.TakeDamage(damage, knockbackVector);
        }
    }
}
