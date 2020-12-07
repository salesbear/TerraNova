using UnityEngine;
[RequireComponent(typeof(BoxCollider2D))]
public class BreakableWall : MonoBehaviour
{
    public void BreakWall()
    {
        //TODO: put in an animation here
        gameObject.SetActive(false);
    }
}
