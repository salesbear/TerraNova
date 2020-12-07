using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;

public class DetectGround : MonoBehaviour
{
    [SerializeField] CharacterController2D _controller;
    public bool inGround { get; private set; } = false;
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
        if ((_controller.platformMask & 1 << collision.gameObject.layer ) != 0)
        {
            if (!inGround)
            {
                inGround = true;
            }
        }
        //else if the object is a one-way platform
        else if (collision.gameObject.layer == 17)
        {
            if (!inGround)
            {
                inGround = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //if the layer is in our platform mask
        if ((_controller.platformMask & 1 << collision.gameObject.layer) != 0
            || collision.gameObject.layer == 17)
        {
            if (inGround)
            {
                inGround = false;
            }
        }
    }
}
