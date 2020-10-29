using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    [SerializeField] float timeToDestroy = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, timeToDestroy);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
