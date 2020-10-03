using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerStateController : MonoBehaviour
{
    [ReadOnly]
    public PlayerState state;
    public static event Action<PlayerState> StateChanged = delegate { };
    //the state that the player starts at when they spawn
    [SerializeField] PlayerState m_startingState;
    public PlayerState previousState { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        ChangeState(m_startingState);
    }
    /// <summary>
    /// change to new state based on stateIndex,
    /// 0 = idle, 1 = Run, 2 = Jump, 3 = Fall, 4 = WallSlide
    /// 5 = Aim, 6 = Attack, 7 = Dodge, 8 = Knockback, 9 = Dead
    /// </summary>
    /// <param name="stateIndex"></param>
    public void ChangeState(int stateIndex)
    {
        //convert index to state type
        previousState = state;
        state = (PlayerState)stateIndex;
        StateChanged.Invoke(state);
    }
    /// <summary>
    /// change to specified new state
    /// </summary>
    /// <param name="newState"></param>
    public void ChangeState(PlayerState newState)
    {
        previousState = state;
        state = newState;
        StateChanged.Invoke(state);
    }

    public void ReturnToPreviousState()
    {
        ChangeState(previousState);
    }
}
