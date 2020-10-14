using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Prime31;

[SelectionBase]
[RequireComponent(typeof(CharacterController2D), typeof(PlayerStateController), typeof(PlayerAttributes))]
public class PlayerMovement : MonoBehaviour
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
    [SerializeField] float wallJumpHeight = 3.5f;
    private float vel_peak = 1.5f; //this is the magnitude of vertical velocity that signifies that the character is at the peak of their jump
    [Space]
    [Header("Dodge Stuff")]
    [SerializeField] float dodgeSpeed = 12f;
    [Tooltip("how long the player dodges for")]
    [SerializeField] float dodgeTime = 0.25f;
    [Tooltip("The size of the player's box collider while dodging")]
    [SerializeField] Vector2 dodgeSize;
    [Tooltip("The offset for the player's box collider while dodging")]
    [SerializeField] Vector2 dodgeOffset;
    BoxCollider2D m_playerCollision;
    //the offset of the BoxCollider2D on the player normally
    Vector2 startOffset;
    //the size of the BoxCollider2D on the player normally
    Vector2 startSize;
    

    [Space]
    [Header("Horizontal Movement")]
    [SerializeField] float runSpeed = 8f;
    [SerializeField] float wallJumpSpeed = 12f;
    [SerializeField] float groundDamping = 20f; // how fast do we change direction? higher means faster
    [SerializeField] float inAirDamping = 5f;

    [Space]
    [Header("Timers")]
    //this should really be done by animation frames, but idk how to do that really, another thing to learn
    [Tooltip("how long the player is invincible while dodging")]
    [SerializeField] float invTime = 0.2f;
    [Tooltip("how long the player is invincible when damaged")]
    [SerializeField] float invTimeDamaged = 0.5f;
    [SerializeField] float knockbackTime = 0.25f;
    [Tooltip("The time between player flashing when they get hit")]
    [SerializeField] private float flashTime = 0.1f;
    private float flashTimer = 0;
    float dodgeTimer = 0.0f;
    float invTimer = 0.0f;
    float knockbackTimer = 0f;

    [Space]
    [Header("Sound Effects")]
    [Tooltip("The sound that plays when the player jumps")]
    [SerializeField] private AudioClip jumpSound;
    [Range(0,1)]
    [Tooltip("how loud the jump sound is played")]
    [SerializeField] private float jumpVolume = 0.85f;
    [Tooltip("The sound that plays when the player dodges")]
    [SerializeField] private AudioClip dodgeSound;
    [Tooltip("the amount that the pitch can shift, X is lower bound, Y is upper bound")]
    [SerializeField] Vector2 pitchRange;

    [Space]
    [Header("Misc.")]
    [Tooltip("The color used to tint the sprite when the player is dodging and invincible")]
    [SerializeField] private Color dodgeTint;
    private Color m_initialColor;

    private bool leftPressed = false; //bools used to handle when both left and right are pressed
    private bool rightPressed = false;
    private bool canMoveHoriz = true; //flag for when the script should process horizontal movement
    public bool facingRight { get { return transform.localScale.x > 0f; } }

    private bool canJump { get { return (int)_stateController.state < 6; } }
    private bool aiming { get { return _stateController.state == PlayerState.Aim; } }
    public bool grounded { get { return _controller.isGrounded; } }
    public bool invincible { get { return invTimer > 0; } }

    private CharacterController2D _controller;
    private PlayerStateController _stateController;
    private PlayerAttributes _player;

    private Animator _animator;
    private SpriteRenderer m_spriteRenderer;
    private RaycastHit2D _lastControllerColliderHit;

    [Space]
    [Header("Debug")]
    [ReadOnly]
    [SerializeField]
    Vector3 _velocity;

    float normalizedHorizontalSpeed = 0;

    //public event Action<bool> Dodge;

    void Awake()
    {
        //getcomponent stuff
        _controller = GetComponent<CharacterController2D>();
        _player = GetComponent<PlayerAttributes>();
        _stateController = GetComponent<PlayerStateController>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_playerCollision = GetComponent<BoxCollider2D>();
        // listen to some events
        //_controller.onControllerCollidedEvent += onControllerCollider;
        //_controller.onTriggerEnterEvent += onTriggerEnterEvent;
        //_controller.onTriggerExitEvent += onTriggerExitEvent;
        PlayerStateController.StateChanged += OnPlayerStateChanged;
    }

    private void Start()
    {
        m_initialColor = m_spriteRenderer.color;
        startSize = m_playerCollision.size;
        startOffset = m_playerCollision.offset;
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

    void OnPlayerStateChanged(PlayerState newstate)
    {
        if (newstate == PlayerState.Dodge)
        {
            m_playerCollision.size = dodgeSize;
            m_playerCollision.offset = dodgeOffset;
            _controller.recalculateDistanceBetweenRays();
        }
        else if (_stateController.previousState == PlayerState.Dodge)
        {
            m_playerCollision.offset = startOffset;
            m_playerCollision.size = startSize;
            _controller.recalculateDistanceBetweenRays();
        }
        //if newstate is one in which we cannot move horizontally, mark that
        if ((int)newstate >= 8)
        {
            canMoveHoriz = false;
        }
        else
        {
            canMoveHoriz = true;
        }
    }

    #endregion
    // I know using update to move is a faux pas, but it should only be a problem if there's framerate issues
    // and that's not something I'm worried about rn
    void Update()
    {
        //if we're dead, don't keep sliding on the floor
        if (_controller.isGrounded && (int)_stateController.state == 9)
        {
            _velocity.x = 0;
        }
        //update all the timers we're keeping track of
        UpdateTimers();

        //figure out if left or right are being pressed so we can handle the case where both are pressed
        GetHorizontalInput();

        //check what buttons have been pressed and set speed based on that
        //TODO: add in animations for running, jumping, etc.
        //if we're in a state where we can move normally
        if ((int)_stateController.state <= 5)
        {
            HandleFacingAndHSpeed();
        }
        //if we can dodge, and the player is trying to
        if (Input.GetButtonDown("Dodge") && (int)_stateController.state < 7)
        {
            Dodge();
        }
        else if (Input.GetButtonDown("Jump") && canJump)
        {
            Jump();
        }

        //if we're not in knockback, dead, or attacking update horizontal speed
        if (canMoveHoriz)
        {
            // apply horizontal speed smoothing. TODO: learn how to use SmoothDamp and change from lerp to SmoothDamp
            var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
            float hSpeed = (_stateController.state == PlayerState.Dodge && _controller.isGrounded) ? dodgeSpeed : runSpeed; //are we dodging or running?
            //if we're aiming or attacking, set speed to 0
            hSpeed = (_stateController.state == PlayerState.Aim || _stateController.state == PlayerState.Attack) ? 0 : hSpeed;
            _velocity.x = Mathf.Lerp(_velocity.x, normalizedHorizontalSpeed * hSpeed, Time.deltaTime * smoothedMovementFactor);
        }

        HandleGravity();

        _controller.move(_velocity * Time.deltaTime);

        // grab our current _velocity to use as a base for all calculations
        _velocity = _controller.velocity;
    }
    
    /// <summary>
    /// update all the timers we're keeping track of
    /// </summary>
    void UpdateTimers()
    {
        //knockback timer
        if (knockbackTimer > 0)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer < 0 && _stateController.state != PlayerState.Dead)
            {
                if (_controller.isGrounded)
                {
                    _stateController.ChangeState(0);
                }
                else
                {
                    _stateController.ChangeState(PlayerState.Fall);
                }
            }
        }
        //invincibility timer
        if (invTimer > 0)
        {
            invTimer -= Time.deltaTime;
            if (_stateController.state != PlayerState.Dead && _stateController.state != PlayerState.Dodge)
            {
                Flash();
            }

            if (invTimer <= 0)
            {
                m_spriteRenderer.enabled = true;
                //reset color
                m_spriteRenderer.color = m_initialColor;
            }
        }
        //dodge timer
        if (dodgeTimer > 0)
        {
            CeilingCheck();
            dodgeTimer -= Time.deltaTime;
            if (dodgeTimer <= 0)
            {
                //if we can undodge
                if (CeilingCheck())
                {
                    _stateController.ChangeState(0);
                }
                else
                {
                    //add a tiny amount to dodge timer so we check if we can undodge next frame
                    dodgeTimer = 0.01f;
                }
            }
        }
    }

    /// <summary>
    /// enables and disables the sprite renderer to create a flashing effect
    /// </summary>
    void Flash()
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

    /// <summary>
    /// determines which horizontal movement buttons we've pressed
    /// </summary>
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
    /// handles changing state and changing horizontal speed for the player
    /// </summary>
    void HandleFacingAndHSpeed()
    {
        if (leftPressed)
        {
            if (rightPressed)
            {
                normalizedHorizontalSpeed = 0;
                if (_controller.isGrounded && !aiming)
                {
                    //set state to idle
                    _stateController.ChangeState(0);
                }
            }
            else
            {
                //make player face left
                if (facingRight)
                {
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                }
                normalizedHorizontalSpeed = -1f;

                if (_controller.isGrounded && !aiming)
                {
                    //set state to run
                    _stateController.ChangeState(1);
                }
            }
        }
        else if (rightPressed)
        {
            //make player face right
            if (!facingRight)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
            normalizedHorizontalSpeed = 1f;

            if (_controller.isGrounded && !aiming)
            {
                //set state to run
                _stateController.ChangeState(1);
            }
        }
        else
        {
            normalizedHorizontalSpeed = 0f;
            //set state to idle
            if (_controller.isGrounded && !aiming)
            {
                _stateController.ChangeState(0);
            }
        }
    }
    
    /// <summary>
    /// sets variables to make the player dodge, actual movement is done in Update
    /// </summary>
    void Dodge()
    {
        dodgeTimer = dodgeTime;
        //if player just took damage, make sure that the invisibility timer isn't cut short
        invTimer = Mathf.Max(invTime,invTimer);
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
        _stateController.ChangeState(PlayerState.Dodge);
        AudioManager.instance.PlaySound(dodgeSound, pitchRange);
        m_spriteRenderer.color = dodgeTint;
    }
    
    /// <summary>
    /// handles jumping and wall jumping
    /// </summary>
    void Jump()
    {
        if (_controller.isGrounded)
        {
            _velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);
            _stateController.ChangeState(2); //change state to jump
            AudioManager.instance.PlaySound(jumpSound, pitchRange, jumpVolume);
        }

        //if we can walljump, do it
        //TODO: put in a check for if we're in knockback
        else if (_player.wallJumpUnlocked && (_controller.collisionState.right || _controller.collisionState.left))
        {
            _velocity.y = Mathf.Sqrt(2f * wallJumpHeight * -gravity);
            _velocity.x = wallJumpSpeed * -normalizedHorizontalSpeed;
            _stateController.ChangeState(2); //change state to jump
            AudioManager.instance.PlaySound(jumpSound, pitchRange, jumpVolume);
        }
    }
    
    /// <summary>
    /// changes vertical velocity based on a number of factors
    /// </summary>
    void HandleGravity()
    {
        ////state changes are being weird, so just putting a failsafe here
        //if (_velocity.y > 0 && _stateController.state == PlayerState.WallSlide && !(_controller.collisionState.left || _controller.collisionState.right))
        //{
        //    _stateController.ChangeState(PlayerState.Jump);
        //}
        //if they aren't grounded and we're going up slowly, slow down gravity to give the player a moment to adjust their jump
        if (_velocity.y < vel_peak && _stateController.state == PlayerState.Jump)
        {
            _velocity.y += peakGravity * Time.deltaTime;
        }
        else
        {
            _velocity.y += gravity * Time.deltaTime;
            //set max fall speed based on if we're wall sliding or not
            float max = 0;
            if (_player.wallJumpUnlocked && (_controller.collisionState.right || _controller.collisionState.left) && (int)_stateController.state < 5)
            {
                max = wallSlideMaxSpeed;
                if (!_controller.isGrounded && _velocity.y < 0)
                {
                    _stateController.ChangeState(4); //change state to wall sliding
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
        if (_velocity.y < 0 && !_controller.isGrounded && (int)_stateController.state < 6 && _stateController.state != PlayerState.WallSlide)
        {
            _stateController.ChangeState(PlayerState.Fall);
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
            _player.TakeDamage(amount);
            if (_stateController.state != PlayerState.Dead)
            {
                _velocity = knockbackVel;
                knockbackTimer = knockbackTime;
                invTimer = invTimeDamaged;
                _stateController.ChangeState(8);
            }
        }
    }
    /// <summary>
    /// checks if we'll bang our head on the ceiling when we stop dodging
    /// returns false if there's a ceiling in the way of standing up, true otherwise
    /// </summary>
    /// <returns></returns>
    bool CeilingCheck()
    {
        float rayDist = (startSize.y - dodgeSize.y) + _controller.skinWidth;
        float boundsWidth = m_playerCollision.bounds.max.x - m_playerCollision.bounds.min.x - (_controller.skinWidth * 2);
        //divide by total rays - 1 so we hit both edges
        float distBetweenRays = boundsWidth / (_controller.totalVerticalRays - 1);
        //set the raycast origin to the top left corner
        Vector3 raycastOrigin;
        RaycastHit2D raycastHit;
        //check for collision along each ray
        for (int i = 0; i < _controller.totalVerticalRays; i++)
        {
            
            raycastOrigin = new Vector3(m_playerCollision.bounds.min.x + _controller.skinWidth + distBetweenRays * i,
                m_playerCollision.bounds.max.y - _controller.skinWidth,
                m_playerCollision.bounds.max.z);
            raycastHit = Physics2D.Raycast(raycastOrigin, Vector2.up, rayDist, _controller.platformMask);
            Debug.DrawRay(raycastOrigin, new Vector3(0, rayDist, 0), Color.red);
            if (raycastHit)
            {
                //Debug.Log("It works the way I think it does");
                return false;
            }
        }
        return true;
    }

}
