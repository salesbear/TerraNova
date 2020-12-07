using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class DeathManager : MonoBehaviour
{
    [SerializeField] float timeToShow = 0.4f;
    [SerializeField] Transform target;
    bool playerDead = false;
    Vector3 initialPosition;
    private Vector3 _smoothDampVel;


    private void Start()
    {
        initialPosition = transform.position;
    }

    private void OnEnable()
    {
        PlayerStateController.StateChanged += OnStateChange;
    }
    private void OnDisable()
    {
        PlayerStateController.StateChanged -= OnStateChange;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerDead)
        {
            transform.position = Vector3.SmoothDamp(transform.position, target.position, ref _smoothDampVel, timeToShow);
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    public void OnStateChange(PlayerState newState)
    {
        if (newState == PlayerState.Dead)
        {
            playerDead = true;
        }
    }

    
}
