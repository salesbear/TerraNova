using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    [Tooltip("Has the player unlocked fire shot yet?")]
    [SerializeField] bool fireUnlocked;
    [Tooltip("The actual point we use to aim the shot")]
    [SerializeField] Transform aimPoint;
    [Tooltip("The renderer that we use to show where the player is aiming")]
    [SerializeField] SpriteRenderer aimSprite;
    [Tooltip("the fireball prefab")]
    [SerializeField] GameObject fireBall;

    PlayerStateController _stateController;
    PlayerMovement player;

    private void Awake()
    {
        _stateController = GetComponentInParent<PlayerStateController>();
        player = GetComponentInParent<PlayerMovement>();
        PlayerStateController.StateChanged += OnStateChange;
    }

    void OnStateChange(PlayerState state)
    {
        if (state == PlayerState.Aim)
        {
            aimSprite.enabled = true;
        }
        else
        {
            aimSprite.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (fireUnlocked && (int)_stateController.state <= 1 && Input.GetButtonDown("Fire1"))
        {
            StartAiming();
            //Debug.Log("Aiming Start!");
        }

        if (_stateController.state == PlayerState.Aim)
        {
            Aim();
            if (Input.GetButtonUp("Fire1"))
            {
                Fire();
            }
        }
    }

    /// <summary>
    /// change player to the aiming state
    /// </summary>
    void StartAiming()
    {
        _stateController.ChangeState(PlayerState.Aim);
    }

    /// <summary>
    /// rotate the aim circle based on player input
    /// </summary>
    void Aim()
    {
        bool downPressed = false;
        bool diagonal = false;
        bool upPressed = false;
        Quaternion newAngle = new Quaternion();
        //if left or right (exclusive) is pressed
        if (Input.GetButton("Right") ^ Input.GetButton("Left"))
        {
            diagonal = true;
        }
        if (Input.GetButton("Up"))
        {
            upPressed = true;
        }
        if (Input.GetButton("Down"))
        {
            downPressed = true;
        }

        int facingMultiplier = (player.facingRight) ? 1 : -1;
        //set angle
        if (downPressed)
        {
            newAngle = (upPressed) ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 0, -45 * facingMultiplier);
        }
        else if (upPressed)
        {
            newAngle = (diagonal) ? Quaternion.Euler(0, 0, 45 * facingMultiplier) : Quaternion.Euler(0, 0, 90 * facingMultiplier);
        }
        else
        {
            newAngle = Quaternion.Euler(0, 0, 0);
        }

        transform.rotation = newAngle;
    }

    /// <summary>
    /// spawn a fireball at the aimpoint and change state back to idle
    /// </summary>
    void Fire()
    {
        //Debug.Log("Fire!");
        //Debug.Log("Transform rotation: " + transform.rotation.eulerAngles);
        Quaternion fireAngle = new Quaternion();
        int offset = (player.facingRight) ? 0 : 180;
        fireAngle = Quaternion.Euler(transform.rotation.eulerAngles.x, 
            transform.rotation.eulerAngles.y, 
            transform.rotation.eulerAngles.z + offset);

        Instantiate(fireBall, aimPoint.position, fireAngle);
        _stateController.ChangeState(0);
    }
}
