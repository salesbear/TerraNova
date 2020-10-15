using UnityEngine;
using System.Collections;
using Prime31;


public class SmoothFollow : MonoBehaviour
{
	public Transform target;
	public float smoothDampTime = 0.2f;
    //public float smoothDampTimeFalling = 0.15f;
    //used to make the camera less jerky when sliding on a wall
    public float smoothDampTimeWallslide = 0.5f;
	[HideInInspector]
	public new Transform transform;
	public Vector3 cameraOffset;
    //[Tooltip("Camera offset used while falling")]
    //public Vector3 cameraOffsetFalling;
    //offset used while walljumping, should make camera shift slowly to look below you while sliding so you can see what's down there
    [Tooltip("Camera offset used while wall sliding")]
    public Vector3 cameraOffsetWallslide;
	public bool useFixedUpdate = false;
	
	private CharacterController2D _playerController;
	private Vector3 _smoothDampVelocity;
    private bool prevFacingRight = true;
    private PlayerMovement _playerMove;
    PlayerStateController _stateController;
    //private bool prevRight;
	
	void Awake()
	{
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
            //case 3:
            //    cameraOffsetLocal = cameraOffsetFalling;
            //    smoothDampTimeLocal = smoothDampTimeFalling;
            //    break;
            case 4:
                cameraOffsetLocal = cameraOffsetWallslide;
                smoothDampTimeLocal = smoothDampTimeWallslide;
                break;
            default:
                cameraOffsetLocal = cameraOffset;
                smoothDampTimeLocal = smoothDampTime;
                break;
        }

        if ( _playerMove.facingRight )
		{
			transform.position = Vector3.SmoothDamp( transform.position, target.position - cameraOffsetLocal, ref _smoothDampVelocity, smoothDampTimeLocal );
            prevFacingRight = true;
		}
		else
		{
			var leftOffset = cameraOffsetLocal;
			leftOffset.x *= -1;
			transform.position = Vector3.SmoothDamp( transform.position, target.position - leftOffset, ref _smoothDampVelocity, smoothDampTimeLocal );
            prevFacingRight = false;
		}
	}
	
}
