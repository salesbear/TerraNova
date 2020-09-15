using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttributes : MonoBehaviour
{
    public int health { get; private set; }
    public int maxHealth { get; private set; }
    public int mana { get; private set; }
    public int maxMana { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        //TODO: Load Max Health
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMaxHealth(int value)
    {
        maxHealth = Mathf.Max(value,1);
    }
}
