using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBarUpdater : MonoBehaviour
{
    [Tooltip("the slider used to fill the mana bar")]
    [SerializeField] Slider fill;
    [Tooltip("the time it takes for the mana bar to update")]
    [SerializeField] float timeToChange = 0.15f;
    float _smoothDampVelocity;
    [Tooltip("the PlayerAttributes component of the player, optional (but probably better for performance to put this in)")]
    [SerializeField] PlayerAttributes player;
    
    private void Awake()
    {
        if (player == null)
        {
            player = FindObjectOfType<PlayerAttributes>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player.maxMana != fill.maxValue)
        {
            fill.maxValue = Mathf.Max(player.maxMana,1);
        }

        if (player.mana != fill.value)
        {
            fill.value = Mathf.SmoothDamp(fill.value, player.mana, ref _smoothDampVelocity, timeToChange);
        }
    }
}
