using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TextScroller : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 100f;
    [SerializeField] private float spacing = 50f;     

    private RectTransform rectTransform;
    private TextMeshProUGUI originalText;
    public float textWidth;
    public bool isClone = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalText = GetComponent<TextMeshProUGUI>();

        // Force TMPro to calculate actual bounds based on the character mesh
        originalText.ForceMeshUpdate();
        textWidth = originalText.preferredWidth;

        if (!isClone) // Only runs once
        {
            // Clone GameObject for seamless loop
            GameObject cloneGO = Instantiate(gameObject, transform.parent);


            cloneGO.GetComponent<TextScroller>().isClone = true;
            RectTransform cloneRect = cloneGO.GetComponent<RectTransform>();


            // Position the duplicate exactly at the end of the original text string (left)
            cloneRect.anchoredPosition = new Vector2(0-spacing, 0);
        }

    }

    void Update()
    {
        // Continuously translate right
        rectTransform.anchoredPosition += new Vector2(scrollSpeed * Time.deltaTime, 0);

        // The moment the core string clears the right side + offset, reset the position
        if (rectTransform.anchoredPosition.x >= 0 + spacing)
        {
            // Resets the position to the left, offscreen upon reaching the threshold
            rectTransform.anchoredPosition = new Vector2(-412.0546f + (spacing/2), 0);
        }
    }
}