using UnityEngine;
using System.Collections;
using Prime31;

//this class is written as a singleton, which I *think* is fine
//but that means there can only ever be one camera following the player. Might need to redo this
//if I ever implement cutscenes
public class SmoothFollow : MonoBehaviour
{
    public static SmoothFollow instance;
	public Transform target;
	public float smoothDampTime = 0.2f;
    public float smoothDampTimeAim = 0.5f;
    public float smoothDampTimeBoundsTransition = 0.6f;
    [SerializeField] float transitionTime = 0.6f;
    float transitionTimer = 0f;
    //public float smoothDampTimeFalling = 0.15f;
    //used to make the camera less jerky when sliding on a wall
    public float smoothDampTimeWallslide = 0.5f;
	[HideInInspector]
	public new Transform transform;
	public Vector3 cameraOffset;
    public Vector3 fallOffset;
    public bool lockedToBounds;
    [Tooltip("the bounds that the camera is locked to")]
    public Bounds cameraBounds;
    //[Tooltip("Camera offset used while falling")]
    //public Vector3 cameraOffsetFalling;
    //offset used while walljumping, should make camera shift slowly to look below you while sliding so you can see what's down there
    [Tooltip("Camera offset used while wall sliding")]
    public Vector3 cameraOffsetWallslide;
	public bool useFixedUpdate = false;
	
	private CharacterController2D _playerController;
	private Vector3 _smoothDampVelocity;
    private Camera m_camera;
    private PlayerMovement _playerMove;
    PlayerStateController _stateController;
    //private bool prevRight;
	
	void Awake()
	{
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
        m_camera = GetComponent<Camera>();
		transform = gameObject.transform;
		_playerController = target.GetComponent<CharacterController2D>();
        _playerMove = target.GetComponent<PlayerMovement>();
        _stateController = target.GetComponent<PlayerStateController>();
	}
    //private void Start()
    //{
    //    cameraOffsetWallslide = new Vector3(cameraOffset.x, -cameraOffset.y, cameraOffset.z);
    //}
    void LateUpdate()
	{
		if( !useFixedUpdate )
			updateCameraPosition();
	}


	void FixedUpdate()
	{
		if( useFixedUpdate )
			updateCameraPosition();
	}


	void updateCameraPosition()
	{
		if( _playerController == null )
		{
			transform.position = Vector3.SmoothDamp( transform.position, target.position - cameraOffset, ref _smoothDampVelocity, smoothDampTime );
			return;
		}
        //set smooth damp time and camera offset based on state
        float smoothDampTimeLocal;
        Vector3 cameraOffsetLocal;
        //set smooth damp time and camera offset based on our state
        switch((int)_stateController.state)
        {
            case 4:
                cameraOffsetLocal = cameraOffsetWallslide;
                smoothDampTimeLocal = smoothDampTimeWallslide;
                break;
            case 5:
                //camera offset is changed in aim so that we can look in the same direction as we aiming
                cameraOffsetLocal = cameraOffset;
                smoothDampTimeLocal = smoothDampTimeAim;
                break;
            default:
                cameraOffsetLocal = cameraOffset;
                smoothDampTimeLocal = smoothDampTime;
                break;
        }
        //if we're falling, or we're dodging and falling, set our offset so we can still see our character
        if (_stateController.state == PlayerState.Fall || 
            (_stateController.state == PlayerState.Dodge 
            && ! _playerController.isGrounded
            && _playerController.velocity.y < 0))
        {
            cameraOffsetLocal = fallOffset;
        }
        if (transitionTimer > 0)
        {
            transitionTimer -= Time.deltaTime;
            smoothDampTimeLocal = smoothDampTimeBoundsTransition;
        }
        if ( _playerMove.facingRight )
		{
            //set target offset to where we want our camera to go
            Vector3 targetOffset = GetOffset(target.position - cameraOffsetLocal);
            transform.position = Vector3.SmoothDamp( transform.position, targetOffset, ref _smoothDampVelocity, smoothDampTimeLocal );
		}
		else
		{
			Vector3 leftOffset = cameraOffsetLocal;
			leftOffset.x *= -1;
            Vector3 targetOffset = GetOffset(target.position - leftOffset);
            transform.position = Vector3.SmoothDamp( transform.position, targetOffset, ref _smoothDampVelocity, smoothDampTimeLocal );
		}
	}
	
    /// <summary>
    /// takes in the offset we want to move towards, and binds it by our cameraBounds
    /// </summary>
    /// <param name="targetOffset"></param>
    /// <returns></returns>
    Vector3 GetOffset(Vector3 targetOffset)
    {
        if (lockedToBounds)
        {
            //if our x bounds are smaller than our camera width, don't update our x position
            if (cameraBounds.extents.x < m_camera.orthographicSize * m_camera.aspect)
            {
                targetOffset.x = cameraBounds.center.x;
            }
            //if our camera bounds are large enough for the camera to move, figure out if we've hit the edge of our bounds or not
            else
            {
                targetOffset.x = Mathf.Clamp(targetOffset.x, cameraBounds.min.x + (m_camera.orthographicSize * m_camera.aspect),
                    cameraBounds.max.x - (m_camera.orthographicSize * m_camera.aspect));
            }
            //if our y bounds are smaller than our camera height, don't update our y position
            if (cameraBounds.extents.y < m_camera.orthographicSize)
            {
                targetOffset.y = cameraBounds.center.y;
            }
            //if our camera bounds are large enough for the camera to move, figure out if we've hit the edge of our bounds or not
            else
            {
                targetOffset.y = Mathf.Clamp(targetOffset.y, cameraBounds.min.y + m_camera.orthographicSize,
                    cameraBounds.max.y - m_camera.orthographicSize);
            }
        }
        return targetOffset;
    }

    /// <summary>
    /// increase our smoothdamp time for a while when transitioning between bounds so that it's not too sudden
    /// </summary>
    public void SetBounds(Bounds bounds)
    {
        if (cameraBounds != bounds)
        {
            cameraBounds = bounds;
            //transitionTimer = transitionTime;
        }
    }
}
