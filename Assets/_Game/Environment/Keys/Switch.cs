using UnityEngine;

public abstract class Switch : MonoBehaviour
{
    public bool active = false;
    public float timeToDeactivate;
    protected float deactivationTimer;

    public abstract void Activate();
}
