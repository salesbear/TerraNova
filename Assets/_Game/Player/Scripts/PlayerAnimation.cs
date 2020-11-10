using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] Animator playerAnimator;

    private void Awake()
    {
        if (playerAnimator == null)
        {
            playerAnimator = GetComponentInChildren<Animator>();
        }
    }

    private void OnEnable()
    {
        PlayerStateController.StateChanged += OnStateChange;
    }

    private void OnDisable()
    {
        PlayerStateController.StateChanged -= OnStateChange;
    }

    public void OnStateChange(PlayerState newState)
    {
        if (playerAnimator != null)
        {
            int stateid = Animator.StringToHash("State");
            playerAnimator.SetInteger(stateid, (int)newState);
        }
    }
}
