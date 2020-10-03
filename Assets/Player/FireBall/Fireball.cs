using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Fireball : MonoBehaviour
{
    [SerializeField] Vector3 knockback;
    [SerializeField] int damage = 3; //the amount of damage you do to enemies
    [SerializeField] float speed = 8f;
    [SerializeField] float timeToDespawn = 2f;

    Rigidbody2D m_rigidbody;
    bool hit_enemy = false;
    //[SerializeField] bool sweetSpot; //do more damage if you hit with the head of the hammer

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        Vector2 dir = new Vector2();
        float angle = (transform.rotation.eulerAngles.z) * Mathf.Deg2Rad;
        dir.x = speed * Mathf.Cos(angle);
        dir.y = speed * Mathf.Sin(angle);
        m_rigidbody.velocity = dir;
        Destroy(gameObject, timeToDespawn);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Object Detected from Hitbox");
        //if it's an enemy
        if (collision.gameObject.layer == 11)
        {
            //Debug.Log("Enemy Hit: " + collision.gameObject.name);
            EnemyStats enemy = collision.gameObject.GetComponentInParent<EnemyStats>();
            if (enemy != null)
            {
                enemy.LogDamage(damage, knockback);
            }
        }
        if (collision.gameObject.layer == 12)
        {
            Door door = collision.gameObject.GetComponentInParent<Door>();
            if (door != null)
            {
                door.Open(true);
            }
        }
        Kill();
    }

    void Kill()
    {
        //TODO: Spawn explosion particle efffect
        Destroy(this.gameObject);
    }
}
