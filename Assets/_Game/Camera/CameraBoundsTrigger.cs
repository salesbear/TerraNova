using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CameraBoundsTrigger : MonoBehaviour
{
    BoxCollider2D _collider;
    [SerializeField] Bounds newBounds;
    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
    }
    private void Start()
    {
        if (newBounds == null)
        {
            newBounds = _collider.bounds;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Collision");
        //if it's the player
        if (collision.gameObject.layer == 10)
        {
            SmoothFollow.instance.SetBounds(newBounds);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(newBounds.center, newBounds.size);
    }
}
