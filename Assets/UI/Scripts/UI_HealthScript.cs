using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_HealthScript : MonoBehaviour
{
    [Tooltip("the PlayerAttributes component of the player, optional (but probably better for performance to put this in)")]
    [SerializeField] PlayerAttributes player;
    [Tooltip("the three sprites used to display health, should be empty heart, half heart, full heart")]
    [SerializeField] Sprite[] heartSprites = new Sprite[3];
    [Tooltip("The Horizontal layout group attached to this object, calls GetComponent if left blank")]
    [SerializeField] HorizontalLayoutGroup layout;
    //The heart prefab we're using to spawn objects
    [SerializeField] GameObject heart;
    int framesToDisableLayout;

    //the list of hearts we're using to display health
    List<GameObject> hearts = new List<GameObject>();

    private void Awake()
    {
        if (player == null)
        {
            player = FindObjectOfType<PlayerAttributes>();
        }
    }

    private void OnEnable()
    {
        PlayerAttributes.MaxHealthChanged += UpdateHearts;
    }

    private void OnDisable()
    {
        PlayerAttributes.MaxHealthChanged -= UpdateHearts;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
    }

    void UpdateHearts(int maxHealth)
    {
        int numHearts = (int)Mathf.Ceil(maxHealth / 2.0f);
        while (hearts.Count < numHearts)
        {
            AddHeart();
        }
        while (hearts.Count > numHearts)
        {
            RemoveHeart();
        }
    }

    void UpdateUI()
    {
        int healthAmount = player.health;
        for (int i = 0; i < hearts.Count; i++)
        {
            Image heartImage = hearts[i].GetComponent<Image>();
            if (healthAmount > 0)
            {
                for (int j = 0; j < 2; j++)
                {
                    if (healthAmount > 0)
                    {
                        healthAmount--;
                        heartImage.sprite = heartSprites[j + 1];
                    }
                }
            }
            else
            {
                heartImage.sprite = heartSprites[0];
            }
        }
        if (framesToDisableLayout > 0)
        {
            framesToDisableLayout -= 1;
        }
        else if (layout.enabled)
        {
            layout.enabled = false;
        }
    }

    /// <summary>
    /// adds a heart to the list and toggles the layout to make them space themselves out
    /// </summary>
    void AddHeart()
    {
        GameObject newHeart = Instantiate(heart, transform);
        layout.enabled = true;
        framesToDisableLayout = 1;
        hearts.Add(newHeart);
    }

    /// <summary>
    /// removes the last heart from the UI and returns false if the UI is empty afterwards (true otherwise)
    /// </summary>
    /// <returns></returns>
    bool RemoveHeart()
    {
        if (hearts.Count > 0)
        {
            GameObject tempHeart = hearts[hearts.Count-1];
            hearts.Remove(tempHeart);
            Destroy(tempHeart);
            //return true if there's still hearts in the list
            if (hearts.Count > 0)
            {
                return true;
            }
        }
        return false;
    }
}
