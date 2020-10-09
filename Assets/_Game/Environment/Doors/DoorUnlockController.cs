using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorUnlockController : MonoBehaviour
{
    List<Switch> keysToDoor = new List<Switch>();
    [Tooltip("The sprite renderers for all the key icons")]
    [SerializeField] SpriteRenderer[] unlockIcons;
    [Tooltip("the sprite used when a key isn't active")]
    [SerializeField] Sprite iconKeyNotActive;
    [Tooltip("the sprite used when a key is active")]
    [SerializeField] Sprite iconKeyActive;
    [Tooltip("The door that this script opens, calls getComponent if not provided")]
    [SerializeField] Door doorToOpen;
    [Tooltip("If true, door won't lock itself after being unlocked")]
    public bool staysUnlocked = true;
    bool unlocked = false;

    private void Awake()
    {
        if (doorToOpen == null)
        {
            doorToOpen = GetComponent<Door>();
        }
    }


    // Update is called once per frame
    void Update()
    {
        //just double checking to be safe
        if (doorToOpen != null)
        {
            UnlockDoor();
        }
    }

    public void AddKey(Switch key)
    {
        keysToDoor.Add(key);
    }

    public void RemoveKey(Switch key)
    {
        keysToDoor.Remove(key);
    }

    /// <summary>
    /// determines if the door should be unlocked, and unlocks it if it should
    /// disables this script if the door should stay open
    /// </summary>
    void UnlockDoor()
    {
        unlocked = true;
        int activeCount = 0;
        foreach (Switch key in keysToDoor)
        {
            if (!key.active)
            {
                unlocked = false;
            }
            else
            {
                activeCount++;
            }
        }

        for (int i = 0; i < unlockIcons.Length; i++)
        {
            if (i < activeCount)
            {
                unlockIcons[i].sprite = iconKeyActive;
            }
            else
            {
                unlockIcons[i].sprite = iconKeyNotActive;
            }
        }

        if (unlocked)
        {
            doorToOpen.Unlock();
            //disable the unlock controller and the keys if the door stays opened and it's been unlocked
            if (staysUnlocked)
            {
                foreach (Switch key in keysToDoor)
                {
                    key.enabled = false;
                }
                this.enabled = false;
            }
        }
        else if (doorToOpen.unlocked)
        {
            doorToOpen.Lock();
        }
    }
}
