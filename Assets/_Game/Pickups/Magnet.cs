using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Magnet : MonoBehaviour
{
    [SerializeField] Rigidbody2D m_rigidbody;
    [SerializeField] float moveForce = 8f;
    private void Awake()
    {
        if (m_rigidbody == null)
        {
            m_rigidbody = GetComponent<Rigidbody2D>();
        }
    }
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.gameObject.layer == 10)
    //    {
    //        Debug.Log("Magnet trigger entered: " + Time.time);
    //    }
    //}
    private void OnTriggerStay2D(Collider2D collision)
    {
        //if it's the player
        if (collision.gameObject.layer == 10)
        {
            Rigidbody2D playerBody = collision.gameObject.GetComponentInParent<Rigidbody2D>();
            Vector2 direction = new Vector2(playerBody.position.x - m_rigidbody.position.x, playerBody.position.y - m_rigidbody.position.y);
            m_rigidbody.AddForce(direction.normalized * moveForce);
        }
    }
}
