using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] Bounds enemyRoom;
    [SerializeField] EnemyStats enemy;
    [SerializeField] EnemyMovement enemyMovement;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] BoxCollider2D[] enemyHitboxes;
    Transform player;
    bool despawned = false;

    private void Awake()
    {
        player = FindObjectOfType<PlayerMovement>().gameObject.GetComponent<Transform>();
        if (enemy == null)
        {
            enemy = GetComponentInChildren<EnemyStats>();
        }
        if (enemyMovement == null)
        {
            enemyMovement = GetComponentInChildren<EnemyMovement>();
        }
        if (sprite == null)
        {
            sprite = GetComponentInChildren<SpriteRenderer>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (despawned && player.position.x < enemyRoom.min.x
            || player.position.y < enemyRoom.min.y
            || player.position.x > enemyRoom.max.x
            || player.position.y > enemyRoom.max.y)
        {
            Respawn();
        }
    }

    public void Despawn()
    {
        enemy.enabled = false;
        enemyMovement.enabled = false;
        sprite.enabled = false;
        foreach (BoxCollider2D collider in enemyHitboxes)
        {
            collider.enabled = false;
        }
        despawned = true;
    }

    public void Respawn()
    {
        enemy.enabled = true;
        enemyMovement.enabled = true;
        sprite.enabled = true;
        foreach (BoxCollider2D collider in enemyHitboxes)
        {
            collider.enabled = true;
        }
        despawned = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(enemyRoom.center, enemyRoom.size);
    }
}
