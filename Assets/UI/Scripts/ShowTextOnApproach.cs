using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowTextOnApproach : MonoBehaviour
{
    TextMeshProUGUI m_text;
    [Tooltip("The distance at which the text is invisible")]
    [SerializeField] float disappearDist = 8f;
    [Tooltip("The distance at which the text is completely opaque")]
    [SerializeField] float visibleDist = 3f;
    [Tooltip("The player's transform, more efficient if you put it in but script can find player automatically")]
    [SerializeField] Transform player;

    private Color m_initialColor;

    private void Awake()
    {
        m_text = GetComponent<TextMeshProUGUI>();
        if (player == null)
        {
            player = FindObjectOfType<PlayerAttributes>().gameObject.transform;
        }
    }

    private void Start()
    {
        m_initialColor = m_text.color;
    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(player.position, transform.position);
        float lerpPercent = Mathf.InverseLerp(disappearDist, visibleDist, dist);
        if (dist > visibleDist)
        {
            m_text.color = new Color(m_initialColor.r, m_initialColor.g, m_initialColor.b, m_initialColor.a * lerpPercent);
        }
        else
        {
            m_text.color = m_initialColor;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, disappearDist);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, visibleDist);
    }
}
