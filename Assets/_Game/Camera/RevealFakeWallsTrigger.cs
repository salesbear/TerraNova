using UnityEngine;
using UnityEngine.Tilemaps;

public class RevealFakeWallsTrigger : MonoBehaviour
{
    [SerializeField] Tilemap wallTilemap;
    bool invisible = false;
    Color initialColor;
    Color invisColor;
    [SerializeField] float timeToDisappear = 0.3f;
    //we want to be able to make a volume start invisible so the player doesn't see a black volume on initially loading up the game
    [SerializeField] bool startInvisible = false;
    float lerpProgress = 0;
    

    private void Start()
    {
        if (wallTilemap != null)
        {
            initialColor = Color.white;
            invisColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0);
        }
        if (startInvisible)
        {
            lerpProgress = timeToDisappear;
        }
    }

    private void Update()
    {
        if (wallTilemap != null)
        {
            if (invisible)
            {
                lerpProgress += Time.deltaTime;
            }
            else
            {
                lerpProgress -= Time.deltaTime;
            }
            lerpProgress = Mathf.Clamp(lerpProgress, 0, timeToDisappear);
            wallTilemap.color = Color.Lerp(initialColor, invisColor, lerpProgress / timeToDisappear);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        //if it's the player
        if (collision.gameObject.layer == 10)
        {
            invisible = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10)
        {
            invisible = false;
        }
    }
}
