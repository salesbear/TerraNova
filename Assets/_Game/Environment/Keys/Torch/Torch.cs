using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : Switch
{
    Animator torchAnimator;
    DoorUnlockController parent;

    private void Awake()
    {
        torchAnimator = GetComponent<Animator>();
        parent = GetComponentInParent<DoorUnlockController>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        parent.AddKey(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (deactivationTimer > 0)
        {
            deactivationTimer -= Time.deltaTime;
            if (deactivationTimer <= 0)
            {
                Deactivate();
            }
        }
    }

    public override void Activate()
    {
        active = true;
        deactivationTimer = timeToDeactivate;
        torchAnimator.SetBool("Active", true);
    }

    void Deactivate()
    {
        active = false;
        torchAnimator.SetBool("Active", false);
    }
}
