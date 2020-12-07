using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePickup : Pickup
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10)
        {
            PlayerAttributes thePlayer = collision.gameObject.GetComponentInParent<PlayerAttributes>();
            if (thePlayer != null)
            {
                Collect(thePlayer);
                SpawnCollectFX(collision.gameObject.transform);
            }
        }
    }

    public override void Collect(PlayerAttributes player)
    {
        player.UnlockFire();
        gameObject.SetActive(false);
    }
}
