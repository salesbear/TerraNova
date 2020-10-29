using UnityEngine;

public abstract class Pickup : MonoBehaviour
{
    public GameObject collectFX;
    public abstract void Collect(PlayerAttributes player);

    public void SpawnCollectFX()
    {
        Instantiate(collectFX, transform.position, transform.rotation);
    }
    public void SpawnCollectFX(Transform spawnTransform)
    {
        Instantiate(collectFX, spawnTransform.position, spawnTransform.rotation);
    }
}
