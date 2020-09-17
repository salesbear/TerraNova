using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Prime31;

[RequireComponent(typeof(CharacterController2D), typeof(PlayerStateController), typeof(PlayerAttributes))]
public class PlayerMovement1 : MonoBehaviour
{
    [Header("Vertical Movement")]
    [Range(-50,-1)]   [SerializeField] float gravity = -25f;
    [Tooltip("the fastest you can fall while sliding down a wall")]
    [Range(0, 25f)] [SerializeField] float wallSlideMaxSpeed = 5f;
    [SerializeField] float jumpHeight = 3f;
    [Tooltip("the fastest you can fall normally")]
    [SerializeField] float maxFallSpeed = 15f;
    [Tooltip("Gravity used at the peak of the jump")]
    [SerializeField] float peakGravity = -10f;
    public bool canWalljump = true;
    [SerializeField] float wallJumpHeight = 3.5f;
    private float vel_peak = 1.5f; //this is the magnitude of vertical velocity that signifies that the character is at the peak of their jump

    [Space]
    [Header("Horizontal Movement")]
    [SerializeField] float runSpeed = 8f;
    [SerializeField] float wallJumpSpeed = 12f;
    [SerializeField] float dodgeSpeed = 12f;
    [SerializeField] float groundDamping = 20f; // how fast do we change direction? higher means faster
    [SerializeField] float inAirDamping = 5f;

    [Header("Timers")]
    //this should really be done by animation frames, but idk how to do that really, another thing to learn
    [Tooltip("how long the player dodges for")]
    [SerializeField] float dodgeTime = 0.25f;
    [Tooltip("how long the player is invincible while dodging")]
    [SerializeField] float invTime = 0.2f;
    [Tooltip("how long the player is invincible when damaged")]
    [SerializeField] float invTimeDamaged = 0.5f;
    [SerializeField] float knockbackTime = 0.25f;
    float dodgeTimer = 0.0f;
    float invTimer = 0.0f;
    float knockbackTimer = 0f;

    //the number of frames between each swap of player visibility
    [Tooltip("The time between player flashing when they get hit")]
    [SerializeField] private float flashTime = 0.1f;
    private float flashTimer = 0;

    bool leftPressed = false; //used to handle when both left and right are pressed
    bool rightPressed = false;
    public bool grounded { get { return _controller.isGrounded; } }
    public bool invincible { get { return invTimer > 0; } }

    private CharacterController2D _controller;
    private PlayerStateController m_stateController;
    private PlayerAttributes m_player;

    private Animator _animator;
    private SpriteRenderer m_spriteRenderer;
    private RaycastHit2D _lastControllerColliderHit;
    [ReadOnly]
    [SerializeField]
    Vector3 _velocity;

    float normalizedHorizontalSpeed = 0;

    //public event Action<bool> Dodge;

    void Awake()
    {
        _controller = GetComponent<CharacterController2D>();
        m_player = GetComponent<PlayerAttributes>();
        //TODO: create and add event listeners
        // listen to some events for illustration purposes
        _controller.onControllerCollidedEvent += onControllerCollider;
        _controller.onTriggerEnterEvent += onTriggerEnterEvent;
        _controller.onTriggerExitEvent += onTriggerExitEvent;
        m_stateController = GetComponent<PlayerStateController>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    #region Event Listeners

    void onControllerCollider(RaycastHit2D hit)
    {
        // bail out on plain old ground hits cause they arent very interesting
        if (hit.normal.y == 1f)
            return;

        // logs any collider hits if uncommented. it gets noisy so it is commented out for the demo
        //Debug.Log( "flags: " + _controller.collisionState + ", hit.normal: " + hit.normal );
    }


    void onTriggerEnterEvent(Collider2D col)
    {
        //if the object is in the hazard or enemy layer
        if (col.gameObject.layer == 9 || col.gameObject.layer == 11)
        {
            //DamageVolume temp = col.GetComponent<DamageVolume>();
            //if (temp != null && !invincible && m_stateController.state != PlayerState.Dead)
            //{
            //    TakeKnockback(temp.knockbackVector);
            //    m_player.TakeDamage(temp.damage);
            //}
        }
        //Debug.Log("onTriggerEnterEvent: " + col.gameObject.name);
    }


    void onTriggerExitEvent(Collider2D col)
    {
        //Debug.Log("onTriggerExitEvent: " + col.gameObject.name);
    }

    #endregion
    // Update is called once per frame
    void Update()
    {
        //if we're dead, don't keep sliding on the floor
        if (_controller.isGrounded && (int)m_stateController.state == 9)
        {
            _velocity.x = 0;
        }
        if (knockbackTimer > 0)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer < 0 && m_stateController.state != PlayerState.Dead)
            {
                if (_controller.isGrounded)
                {
                    m_stateController.ChangeState(0);
                }
                else
                {
                    m_stateController.ChangeState(PlayerState.Fall);
                }
            }
        }
        if (dodgeTimer > 0)
        {
            dodgeTimer -= Time.deltaTime;
            if (dodgeTimer <= 0)
            {
                m_stateController.ChangeState(0);
            }
        }
        if (invTimer > 0)
        {
            invTimer -= Time.deltaTime;
            if (m_stateController.state != PlayerState.Dead && m_stateController.state != PlayerState.Dodge)
            {
                if (flashTimer <= 0)
                {
                    if (m_spriteRenderer.enabled)
                    {
                        m_spriteRenderer.enabled = false;
                    }
                    else
                    {
                        m_spriteRenderer.enabled = true;
                    }
                    flashTimer = flashTime;
                }
                else
                {
                    flashTimer -= Time.deltaTime;
                }
            }
            
            if (invTimer <= 0)
            {
                m_spriteRenderer.enabled = true;
            }
        }
        //figure out if left or right are being pressed so we can handle the case where both are pressed
        GetHorizontalInput();

        //check what buttons have been pressed and set speed based on that
        //TODO: add in animations for running, jumping, etc.
        //if we're in a state where we can move normally
        if ((int)m_stateController.state <= 4)
        {
            HandleFacingAndHSpeed();
        }
        //if we can dodge, and the player is trying to
        if (Input.GetButtonDown("Dodge") && (int)m_stateController.state < 7)
        {
            dodgeTimer = dodgeTime;
            invTimer = invTime;
            if (_controller.isGrounded)
            {
                if (transform.localScale.x > 0f)
                {
                    normalizedHorizontalSpeed = 1;
                }
                else
                {
                    normalizedHorizontalSpeed = -1;
                }
            }
            m_stateController.ChangeState(PlayerState.Dodge);
        }
        else if (Input.GetButtonDown("Jump") && (int)m_stateController.state < 6)
        {
            if (_controller.isGrounded)
            {
                _velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);
                m_stateController.ChangeState(2); //change state to jump
            }

            //if we can walljump, do it
            //TODO: put in a check for if we're in knockback
            else if (canWalljump && (_controller.collisionState.right || _controller.collisionState.left))
            {
                _velocity.y = Mathf.Sqrt(2f * wallJumpHeight * -gravity);
                _velocity.x = wallJumpSpeed * -normalizedHorizontalSpeed;
                m_stateController.ChangeState(2); //change state to jump
            }
        }

        //if we're not in knockback or dead, update horizontal speed
        if ((int)m_stateController.state < 8)
        {
            // apply horizontal speed smoothing. TODO: learn how to use SmoothDamp and change from lerp to SmoothDamp
            var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
            float hSpeed = (m_stateController.state == PlayerState.Dodge && _controller.isGrounded) ? dodgeSpeed : runSpeed; //are we dodging or running?
            _velocity.x = Mathf.Lerp(_velocity.x, normalizedHorizontalSpeed * hSpeed, Time.deltaTime * smoothedMovementFactor);
        }
        
        
        //TODO: add in a check for if we're wall jumping, wall jumping feels floaty rn
        //if they aren't grounded and we're going up slowly, slow down gravity to give the player a moment to adjust their jump
        if (_velocity.y < vel_peak && m_stateController.state == PlayerState.Jump)
        {
            _velocity.y += peakGravity * Time.deltaTime;
        }
        else
        {
            _velocity.y += gravity * Time.deltaTime;
            //set max fall speed based on if we're wall sliding or not
            float max = 0;
            if (canWalljump && (_controller.collisionState.right || _controller.collisionState.left) && (int)m_stateController.state < 5)
            {
                max = wallSlideMaxSpeed;
                if (!_controller.isGrounded)
                {
                    m_stateController.ChangeState(4); //change state to wall sliding
                }
            }
            else
            {
                max = maxFallSpeed;
            }
            if (_velocity.y < -max)
            {
                _velocity.y = -max;
            }
        }
        //if we're falling, set state to falling
        if (_velocity.y < 0 && !_controller.isGrounded && (int)m_stateController.state < 6)
        {
            m_stateController.ChangeState(PlayerState.Fall);
        }

        _controller.move(_velocity * Time.deltaTime);

        // grab our current _velocity to use as a base for all calculations
        _velocity = _controller.velocity;
    }

    void GetHorizontalInput()
    {
        if (Input.GetButtonDown("Left"))
        {
            leftPressed = true;
        }
        else if (Input.GetButtonUp("Left"))
        {
            leftPressed = false;
        }
        if (Input.GetButtonDown("Right"))
        {
            rightPressed = true;
        }
        else if (Input.GetButtonUp("Right"))
        {
            rightPressed = false;
        }
    }

    /// <summary>
    /// handles changing state, playing animations, and changing horizontal speed for the player
    /// </summary>
    void HandleFacingAndHSpeed()
    {
        if (leftPressed)
        {
            if (rightPressed)
            {
                normalizedHorizontalSpeed = 0;
                if (_controller.isGrounded)
                {
                    //set state to idle
                    m_stateController.ChangeState(0);
                }
            }
            else
            {
                //make player face left
                if (transform.localScale.x > 0f)
                {
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                }
                normalizedHorizontalSpeed = -1f;

                if (_controller.isGrounded)
                {
                    //set state to run
                    m_stateController.ChangeState(1);
                }
            }
        }
        else if (rightPressed)
        {
            //make player face right
            if (transform.localScale.x < 0f)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
            normalizedHorizontalSpeed = 1f;

            if (_controller.isGrounded)
            {
                //set state to run
                m_stateController.ChangeState(1);
            }
        }
        else
        {
            normalizedHorizontalSpeed = 0f;
            //set state to idle
            if (_controller.isGrounded)
            {
                m_stateController.ChangeState(0);
            }
        }
    }

    /// <summary>
    /// take damage and then get knocked back if not dead
    /// </summary>
    /// <param name="amount">amount of damage to take</param>
    /// <param name="knockbackVel"></param>
    public void TakeDamage(int amount, Vector3 knockbackVel)
    {
        if (!invincible)
        {
            m_player.TakeDamage(amount);
            if (m_stateController.state != PlayerState.Dead)
            {
                _velocity = knockbackVel;
                knockbackTimer = knockbackTime;
                invTimer = invTimeDamaged;
                m_stateController.ChangeState(8);
            }
        }
    }

    void HandleInvincibility()
    {

    }
}
