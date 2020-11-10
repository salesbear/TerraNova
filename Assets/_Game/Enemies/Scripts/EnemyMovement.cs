using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;

//TODO: create and implement a real state system for enemies in a separate class
[RequireComponent(typeof(CharacterController2D))]
public class EnemyMovement : MonoBehaviour
{
    public enum EnemyBehavior { Idle = 0,Return, Wander }

    CharacterController2D _controller;
    Transform target;
    PlayerMovement player;
    
    [Header("Player Detection")]
    [Tooltip("How far the enemy can see")]
    [SerializeField] float sightRange = 10f;
    [Tooltip("the layers that the enemy can see")]
    [SerializeField] LayerMask sightMask = new LayerMask();

    [Header("Movement")]
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float passiveSpeed = 4f;
    [Tooltip("If the enemy is this close to its home it stops moving")]
    [SerializeField] float homeRadius = 1f;
    [SerializeField] float groundDamping = 5f;
    [Tooltip("How fast the enemy can fall")]
    [Range(1,100)]
    [SerializeField] float maxFallSpeed = 20f;
    [SerializeField] DetectGround groundDetector;

    bool stunned { get { return stunTimer > 0; } }
    [Header("Knockback")]
    [Tooltip("How long the enemy is stunned for when hit")]
    [SerializeField] float stunTime = 0.1f;
    [Tooltip("How much to multiply knockback by, higher = more knockback")]
    [SerializeField] Vector2 knockbackMultiplier = new Vector2(1,1);

    [Header("Misc.")]
    [SerializeField] Animator enemyAnimator;
    public EnemyBehavior Startbehavior;

    [Header("Debug")]
    [ReadOnly]
    [SerializeField]
    float stunTimer = 0f;
    [ReadOnly]
    [SerializeField]
    Vector3 home;
    [ReadOnly]
    [SerializeField]
    float normalizedHorizontalSpeed = 0f;
    [ReadOnly]
    [SerializeField]
    Vector3 _velocity;
    [ReadOnly]
    [SerializeField]
    bool playerSpotted = false;
    //what the enemy is currently doing
    [ReadOnly]
    [SerializeField]
    EnemyBehavior currentBehavior;
    private bool facingRight { get { return transform.localScale.x > 0f; } }

    private void Awake()
    {
        player = FindObjectOfType<PlayerMovement>();
        target = player.gameObject.transform;
        _controller = GetComponent<CharacterController2D>();
        if (enemyAnimator == null)
        {
            enemyAnimator = GetComponentInChildren<Animator>();
        }
        if (groundDetector == null)
        {
            groundDetector = GetComponentInChildren<DetectGround>();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        home = transform.position;
    }

    //private void OnEnable()
    //{
    //    _controller.onControllerCollidedEvent += onControllerCollider;
    //}
    //private void OnDisable()
    //{
    //    _controller.onControllerCollidedEvent -= onControllerCollider;
    //}

    //void onControllerCollider(RaycastHit2D hit)
    //{
    //    //if we hit solid ground, set our home
    //    if (hit.normal.y == 1f)
    //        home = hit.transform.position;

    //    // logs any collider hits if uncommented. it gets noisy so it is commented out for the demo
    //    //Debug.Log( "flags: " + _controller.collisionState + ", hit.normal: " + hit.normal );
    //}

    // Update is called once per frame
    void Update()
    {
        if (stunned)
        {
            stunTimer -= Time.deltaTime;
        }
        //reset this each frame in case player isn't visible anymore
        playerSpotted = false;
        if (Vector3.Distance(transform.position, target.position) < sightRange)
        {
            RaycastHit2D raycastHit;
            raycastHit = Physics2D.Raycast(transform.position, target.position - transform.position, sightRange, sightMask);
            Debug.DrawRay(transform.position, target.position - transform.position);
            if (raycastHit.collider.CompareTag("Player"))
            {
                if (_controller.isGrounded)
                {
                    normalizedHorizontalSpeed = (transform.position.x <= target.position.x) ? 1 : -1;
                }
                //if we're in the air, don't move horizontally
                else
                {
                    normalizedHorizontalSpeed = 0f;
                }
                playerSpotted = true;
            }
            //if we can't see the player, don't move horizontally
            else
            {
                normalizedHorizontalSpeed = 0;
            }
        }
        //if the player's out of our sight range, don't move horizontally
        else
        {
            //we only want to move towards home if we're on the same plane as it
            if (Mathf.Abs(home.x - transform.position.x) > homeRadius)
            {
                normalizedHorizontalSpeed = (home.x > transform.position.x) ? 1 : -1;
            }
            else
            {
                normalizedHorizontalSpeed = 0;
            }
        }
        int isRunning = Animator.StringToHash("IsRunning");
        if (normalizedHorizontalSpeed == 0 || stunned)
        {
            enemyAnimator.SetBool(isRunning, false);
        }
        else
        {
            enemyAnimator.SetBool(isRunning, true);
        }
        //handle facing
        if ((facingRight && normalizedHorizontalSpeed < 0)
                        || (!facingRight && normalizedHorizontalSpeed > 0))
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

        if (!stunned)
        {
            float speed = (playerSpotted) ? moveSpeed : passiveSpeed;
            if (groundDetector.inGround)
            {
                _velocity.x = Mathf.Lerp(_velocity.x, speed * normalizedHorizontalSpeed, Time.deltaTime * groundDamping);
            }
            else
            {
                _velocity.x = 0;
            }
        }
        _velocity.y += player.gravity * Time.deltaTime;
        _velocity.y = Mathf.Max(_velocity.y, -maxFallSpeed);

        _controller.move(_velocity * Time.deltaTime);
        _velocity = _controller.velocity;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(229f / 255f, 149f / 255f, 69f / 255f);
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

    public void TakeKnockback(Vector3 knockback)
    {
        _velocity = knockback * knockbackMultiplier;
        stunTimer = stunTime;
    }
}
