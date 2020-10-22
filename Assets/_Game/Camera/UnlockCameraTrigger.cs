using UnityEngine;

public class UnlockCameraTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if it's the player
        if (collision.gameObject.layer == 10)
        {
            SmoothFollow.instance.lockedToBounds = false;
        }
    }
}
