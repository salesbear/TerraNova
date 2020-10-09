using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Fireball : MonoBehaviour
{
    [Tooltip("the amount of knockback enemies take when hit by this")]
    [SerializeField] Vector3 knockback;
    [SerializeField] int damage = 2; //the amount of damage you do to enemies
    [SerializeField] float speed = 12f;
    [SerializeField] float timeToDespawn = 2f;
    [Header("Audio")]
    [Tooltip("The sound that plays when the fireball hits something")]
    [SerializeField] AudioClip explosionClip;

    Rigidbody2D m_rigidbody;

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
        //if we hit a door
        if (collision.gameObject.layer == 12)
        {
            Door door = collision.gameObject.GetComponentInParent<Door>();
            if (door != null)
            {
                if (door.isClosed)
                {
                    door.Open(true);
                }
                else
                {
                    //don't kill fireball if we hit an open door
                    return;
                }
            }
        }
        //if we hit a switch
        if (collision.gameObject.layer == 14)
        {
            Switch theSwitch = collision.gameObject.GetComponent<Switch>();
            if (theSwitch != null)
            {
                theSwitch.Activate();
            }
        }
        else
        {
            Kill();
        }
    }

    void Kill()
    {
        AudioSource.PlayClipAtPoint(explosionClip, transform.position);
        //TODO: Spawn explosion particle efffect
        Destroy(this.gameObject);
    }
}
