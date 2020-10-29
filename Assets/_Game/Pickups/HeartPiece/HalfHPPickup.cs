using UnityEngine;

public class HalfHPPickup : Pickup
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
        player.AddHeartPiece();
        gameObject.SetActive(false);
    }
}
