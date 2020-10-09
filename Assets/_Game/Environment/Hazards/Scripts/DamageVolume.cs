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
        //Debug.Log("Object Detected");
        if (collision.gameObject.layer == 10 && !collision.CompareTag("Hitbox"))
        {
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
            player.TakeDamage(damage, knockbackVector);
        }
    }
}
