using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] bool unlocked = true;
    [SerializeField] bool fireRequired = false;
    [SerializeField] bool destroyPermanently = false;
    [Tooltip("the color that the door is tinted by when hit")]
    [SerializeField] Color hitColor;
    [Tooltip("The color that the door is tinted by while it's locked")]
    [SerializeField] Color lockColor;
    [SerializeField] float timeToReenable = 3.0f;
    [SerializeField] BoxCollider2D mainBoxCollider;
    float reenableTimer = 0f;
    Color initialColor;
    SpriteRenderer spriteRenderer;

    bool playerInCollision = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        initialColor = spriteRenderer.color;
        if (!unlocked)
        {
            spriteRenderer.color = lockColor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (reenableTimer > 0)
        {
            reenableTimer -= Time.deltaTime;
        }
        else if (!mainBoxCollider.enabled && !playerInCollision)
        {
            Close();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //if it's the player, mark that they're in the hitbox so the door doesn't come back while the player's in the hitbox
        if (collision.gameObject.layer == 10)
        {
            playerInCollision = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //check if it's the player just to be safe
        if (collision.gameObject.layer == 10)
        {
            playerInCollision = false;
        }
    }

    /// <summary>
    /// opens the door
    /// </summary>
    public void Open(bool hitWithFire)
    {
        if (unlocked)
        {
            if (mainBoxCollider.enabled && (hitWithFire || !fireRequired))
            {
                if (destroyPermanently)
                {
                    gameObject.SetActive(false);
                }
                mainBoxCollider.enabled = false;
                spriteRenderer.color = hitColor;
                reenableTimer = timeToReenable;
            }
        }
    }

    void Close()
    {
        spriteRenderer.color = initialColor;
        mainBoxCollider.enabled = true;
    }

    public void Unlock()
    {
        unlocked = true;
        spriteRenderer.color = initialColor;
    }
}
