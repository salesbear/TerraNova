using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HealthPickup : Pickup
{
    Rigidbody2D m_rigidbody;
    [Tooltip("the parent object of the pickup")]
    [SerializeField] GameObject parent;
    [Tooltip("the amount to heal the player by")]
    [SerializeField] int healAmount = 1;
    [Tooltip("the range of possible x velocities when spawned")]
    [SerializeField] Vector2 xVelRange;
    [Tooltip("the range of possible y velocities when spawned")]
    [SerializeField] Vector2 yVelRange;

    private void Awake()
    {
        m_rigidbody = GetComponentInParent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        Spawn();
    }

    private void Spawn()
    {
        float xVel = Random.Range(xVelRange.x, xVelRange.y);
        float yVel = Random.Range(yVelRange.x, yVelRange.y);
        Vector2 newVelocity = new Vector2(xVel, yVel);
        m_rigidbody.velocity = newVelocity;
    }

    //when the player collides with this object, have them collect it
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if we collided with the player
        if (collision.gameObject.layer == 10)
        {
            PlayerAttributes thePlayer = collision.gameObject.GetComponent<PlayerAttributes>();
            if (thePlayer != null)
            {
                Collect(thePlayer);
            }
        }
    }

    public override void Collect(PlayerAttributes player)
    {
        player.IncreaseHealth(healAmount);
        Despawn();
    }
    //TODO: play some collect particle effect, sound effect, or animation
    void Despawn()
    {
        Destroy(parent);
    }
}
