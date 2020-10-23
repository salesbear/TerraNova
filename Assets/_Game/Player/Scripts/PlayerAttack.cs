using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    PlayerStateController _stateController;

    [Tooltip("The hitboxes used for a ground attack")]
    [SerializeField] GameObject[] groundHBoxes;
    [Header("Attack Controls")]
    [Tooltip("Amount of time before hitboxes come out")]
    [Range(0, 0.5f)]
    [SerializeField] float anticipationTime = 0.12f;
    float anticipationTimer = 0f;

    [Tooltip("Amount of time before hitboxes disappear")]
    [Range(0, 0.5f)]
    [SerializeField] float attackTime = 0.25f;
    float attackTimer = 0f;

    [Tooltip("Amount of time before player regains control")]
    [Range(0, 0.5f)]
    [SerializeField] float coolDownTime = 0.08f;
    float coolDownTimer = 0f;

    [Header("Audio")]
    [SerializeField] AudioClip attackSound;
    [SerializeField] float attackVolume = 0.9f;

    void OnStateChanged(PlayerState newstate)
    {
        if (_stateController.previousState == PlayerState.Attack)
        {
            DisableGroundHitBoxes();
            ResetTimers();
        }
    }

    private void Awake()
    {
        _stateController = GetComponent<PlayerStateController>();

        PlayerStateController.StateChanged += OnStateChanged;
    }

    // Update is called once per frame
    void Update()
    {
        HandleTimers();
        //can only attack if we're on the ground and not dodging or in knockback
        if (((int)_stateController.state <= 1 || _stateController.state == PlayerState.Aim) && Input.GetButtonDown("Melee"))
        {
            anticipationTimer = anticipationTime;
            _stateController.ChangeState(PlayerState.Attack);
        }
    }

    /// <summary>
    /// handles all the timers to make the attack code work. checks done in reverse of how the timers are set so that we don't
    /// mess up and double reduce timers
    /// </summary>
    void HandleTimers()
    {
        if (coolDownTimer > 0)
        {
            coolDownTimer -= Time.deltaTime;
            if (coolDownTimer <= 0)
            {
                _stateController.ChangeState(0);
            }
        }

        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                DisableGroundHitBoxes();
                coolDownTimer = coolDownTime;
            }
        }

        if (anticipationTimer > 0)
        {
            anticipationTimer -= Time.deltaTime;
            if (anticipationTimer <= 0)
            {
                attackTimer = attackTime;
                EnableGroundHitBoxes();
                AudioManager.instance.PlaySound(attackSound,attackVolume);
            }
        }
    }

    /// <summary>
    /// sets all of the timers in this class to 0
    /// </summary>
    void ResetTimers()
    {
        anticipationTimer = 0f;
        attackTimer = 0f;
        coolDownTimer = 0f;
    }
    /// <summary>
    /// enable all the ground hit boxes
    /// </summary>
    void EnableGroundHitBoxes()
    {
        for (int i = 0; i < groundHBoxes.Length; i++)
        {
            groundHBoxes[i].gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// disable all the ground hitboxes
    /// </summary>
    void DisableGroundHitBoxes()
    {
        for (int i = 0; i < groundHBoxes.Length; i++)
        {
            groundHBoxes[i].gameObject.SetActive(false);
        }
    }
}
