using Prime31;
using UnityEngine;

public class DetectWalls : MonoBehaviour
{
    [SerializeField] CharacterController2D _controller;
    public bool inWall { get; private set; } = false;
    public bool inHazard { get; private set; } = false;
    private void Awake()
    {
        if (_controller == null)
        {
            _controller = GetComponentInParent<CharacterController2D>();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //if the layer is in our platform mask
        if ((_controller.platformMask & 1 << collision.gameObject.layer) != 0)
        {
            if (!inWall)
            {
                inWall = true;
            }
        }
        //else if the object is a hazard
        else if (collision.gameObject.layer == 9)
        {
            inHazard = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //if the layer is in our platform mask
        if ((_controller.platformMask & 1 << collision.gameObject.layer) != 0)
        {
            if (inWall)
            {
                inWall = false;
            }
        }
        if (collision.gameObject.layer == 9)
        {
            if (inHazard)
            {
                inHazard = false;
            }
        }
    }
}
