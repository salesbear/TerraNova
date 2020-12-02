using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;

//TODO: create and implement a real state system for enemies in a separate class
[RequireComponent(typeof(CharacterController2D))]
public class EnemyMovement : MonoBehaviour
{
    public enum EnemyBehavior { Idle = 0,Chase,Return, Wander }

    CharacterController2D _controller;
    Transform target;
    PlayerMovement player;
    
    [Header("Player Detection")]
    [Tooltip("How far the enemy can see")]
    [SerializeField] float sightRange = 10f;
    [SerializeField] float peripheralVision = 3f;
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
    [SerializeField] DetectWalls wallDetector;
    [Tooltip("How long the enemy waits before turning around, randomized to be between x and y")]
    [SerializeField] Vector2 waitTime = new Vector2(0.4f, 1f);
    float waitTimer = 0f;
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
    [ReadOnly]
    [SerializeField]
    bool inGroundLastFrame = true;
    [ReadOnly]
    [SerializeField]
    bool inWallLastFrame = false;
    [ReadOnly]
    [SerializeField]
    bool inHazardLastFrame = false;

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
        if (wallDetector == null)
        {
            wallDetector = GetComponentInChildren<DetectWalls>();
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
        if (waitTimer > 0)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0 && currentBehavior == EnemyBehavior.Wander)
            {
                if (wallDetector.inWall || wallDetector.inHazard || !groundDetector.inGround)
                {
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                }
            }
        }
        //look to see if we can see the player
        LookForPlayer();

        //determine how to move based off of current behavior
        switch(currentBehavior)
        {
            case EnemyBehavior.Idle:
                if (_controller.isGrounded)
                {
                    normalizedHorizontalSpeed = 0f;
                }
                break;
            //if we're chasing the player
            case EnemyBehavior.Chase:
                if (_controller.isGrounded)
                {
                    normalizedHorizontalSpeed = (transform.position.x <= target.position.x) ? 1 : -1;
                }
                break;
            case EnemyBehavior.Return:
                //if the enemy isn't where it started, return home
                normalizedHorizontalSpeed = (home.x > transform.position.x) ? 1 : -1;
                break;
            case EnemyBehavior.Wander:
                normalizedHorizontalSpeed = (facingRight) ? 1 : -1;
                //note: I think this might break if the enemy can't move, but I might be wrong
                if (wallDetector.inWall || wallDetector.inHazard || ! groundDetector.inGround)
                {
                    if (waitTimer <= 0)
                    {
                        waitTimer = Random.Range(waitTime.x, waitTime.y);
                    }
                }
                else if (waitTimer > 0)
                {
                    waitTimer = 0;
                }
                break;
        }

        int isRunning = Animator.StringToHash("IsRunning");
        if (normalizedHorizontalSpeed == 0 || stunned)
        {
            enemyAnimator.SetBool(isRunning, false);
        }
        else if (_controller.isGrounded)
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
            if (_controller.isGrounded && groundDetector.inGround && !(wallDetector.inWall || wallDetector.inHazard) )
            {
                _velocity.x = Mathf.Lerp(_velocity.x, speed * normalizedHorizontalSpeed, Time.deltaTime * groundDamping);
            }
            else
            {
                if (_controller.isGrounded)
                {
                    _velocity.x = 0;
                }
                enemyAnimator.SetBool(isRunning, false);
            }
        }
        _velocity.y += player.gravity * Time.deltaTime;
        _velocity.y = Mathf.Max(_velocity.y, -maxFallSpeed);

        _controller.move(_velocity * Time.deltaTime);
        _velocity = _controller.velocity;

        inWallLastFrame = wallDetector.inWall;
        inHazardLastFrame = wallDetector.inHazard;
        inGroundLastFrame = groundDetector.inGround;
    }

    /// <summary>
    /// looks for the player based on the direction the enemy is facing
    /// enemy has some peripheral vison so if a player gets too close
    /// the enemy will notice them regardless of how the enemy is facing
    /// 
    /// ideally, their awareness would slowly fill up if the player was too close
    /// but I only have so much time for this
    /// </summary>
    void LookForPlayer()
    {
        playerSpotted = false;
        if (Vector3.Distance(transform.position, target.position) < sightRange)
        {
            //if the player is in the enemy's peripheral vision, or if the player is in line of sight
            if (Vector3.Distance(transform.position,target.position) < peripheralVision ||
                (facingRight && target.position.x >= transform.position.x) || (!facingRight && target.position.x < transform.position.x))
            {
                RaycastHit2D raycastHit;
                raycastHit = Physics2D.Raycast(transform.position, target.position - transform.position, sightRange, sightMask);
                Debug.DrawRay(transform.position, target.position - transform.position);
                if (raycastHit)
                {
                    if (raycastHit.collider.CompareTag("Player"))
                    {
                        currentBehavior = EnemyBehavior.Chase;
                        playerSpotted = true;
                    }
                }
            }
        }
        if (!playerSpotted)
        {
            if (Mathf.Abs(home.y - transform.position.y) < homeRadius)
            {
                if (Mathf.Abs(home.x - transform.position.x) > homeRadius)
                {
                    currentBehavior = EnemyBehavior.Return;
                }
                else
                {
                    currentBehavior = EnemyBehavior.Idle;
                }
            }
            else if (_controller.isGrounded)
            {
                currentBehavior = EnemyBehavior.Wander;
            }
            else
            {
                currentBehavior = EnemyBehavior.Idle;
            }
        }
    }

    

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(229f / 255f, 149f / 255f, 69f / 255f);
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = new Color(140 / 255f, 63 / 255f, 70 / 255f);
        Gizmos.DrawWireSphere(transform.position, peripheralVision);
    }

    public void TakeKnockback(Vector3 knockback)
    {
        _velocity = knockback * knockbackMultiplier;
        stunTimer = stunTime;
        //turn to face the player if necessary
        if ((target.position.x < transform.position.x && facingRight )
            || (target.position.x > transform.position.x && !facingRight))
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }
}
