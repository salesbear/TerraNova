using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;

[RequireComponent(typeof(CharacterController2D))]
public class PlayerMovement1 : MonoBehaviour
{
    [Header("Vertical Movement")]
    [Range(-50,-1)]   [SerializeField] float gravity = -25f;
    [Tooltip("Gravity value used while clinging to a wall")]
    [Range(0f,25f)] [SerializeField] float wallJumpGravity = 5f;
    [SerializeField] float jumpHeight = 3f;
    [SerializeField] float maxFallSpeed = 15f;
    [Tooltip("Gravity used at the peak of the jump")]
    [SerializeField] float peakGravity = 10f;

    [Space]
    [Header("Horizontal Movement")]
    [SerializeField] float runSpeed = 8f;
    [SerializeField] float dodgeSpeed = 12f;
    [SerializeField] float groundDamping = 20f; // how fast do we change direction? higher means faster
    [SerializeField] float inAirDamping = 5f;
    [Tooltip("how long the player is invincible while dodging")]
    [SerializeField] float invTime = 0.2f;


    bool leftPressed = false; //used to handle when both left and right are pressed
    bool rightPressed = false;

    private CharacterController2D _controller;
    private Animator _animator;
    private RaycastHit2D _lastControllerColliderHit;
    public Vector3 _velocity;

    float normalizedHorizontalSpeed = 0;

    void Awake()
    {
        _controller = GetComponent<CharacterController2D>();

        //TODO: create and add event listeners
        // listen to some events for illustration purposes
        _controller.onControllerCollidedEvent += onControllerCollider;
        _controller.onTriggerEnterEvent += onTriggerEnterEvent;
        _controller.onTriggerExitEvent += onTriggerExitEvent;
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
        Debug.Log("onTriggerEnterEvent: " + col.gameObject.name);
    }


    void onTriggerExitEvent(Collider2D col)
    {
        Debug.Log("onTriggerExitEvent: " + col.gameObject.name);
    }

    #endregion
    // Update is called once per frame
    void Update()
    {
        if (_controller.isGrounded)
        {
            _velocity.y = 0;
        }
        //figure out if left or right are being pressed so we can handle the case where both are pressed
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
        //check what buttons have been pressed and set speed based on that
        //TODO: add in animations for running, jumping, etc.
        if (leftPressed)
        {
            if (rightPressed)
            {
                normalizedHorizontalSpeed = 0;
            }
            else
            {
                normalizedHorizontalSpeed = -1f;
            }
        }
        else if (rightPressed)
        {
            normalizedHorizontalSpeed = 1f;
        }
        else
        {
            normalizedHorizontalSpeed = 0f;
        }

        if (Input.GetButtonDown("Jump"))
        {
            //TODO: add in check for if we're wall jumping
            if (_controller.isGrounded)
            {
                _velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);
            }
        }

        // apply horizontal speed smoothing. TODO: learn how to use SmoothDamp and change from lerp to SmoothDamp
        var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
        _velocity.x = Mathf.Lerp(_velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor);

        // apply gravity before moving
        _velocity.y += gravity * Time.deltaTime;

        _controller.move(_velocity * Time.deltaTime);

        // grab our current _velocity to use as a base for all calculations
        _velocity = _controller.velocity;
    }
}
