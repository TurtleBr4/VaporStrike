using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TextScroller : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 100f; // Pixels per second
    [SerializeField] private float spacing = 50f;     // Gap size between the original text and its twin

    private RectTransform rectTransform;
    private TextMeshProUGUI originalText;
    private float textWidth;
    public bool isClone =false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalText = GetComponent<TextMeshProUGUI>();

        // Force TMPro to calculate actual bounds based on the character mesh
        originalText.ForceMeshUpdate();
        textWidth = originalText.preferredWidth;

        // Clone the text element to serve as the tailing twin
        if (!isClone)
        {
            GameObject cloneGO = Instantiate(gameObject, transform.parent);

            // Strip the looping script and size fitter from the clone to avoid infinite loops/glitches
            //Destroy(cloneGO.GetComponent<TextScroller>());
            //Destroy(cloneGO.GetComponent<UnityEngine.UI.ContentSizeFitter>());
            cloneGO.GetComponent<TextScroller>().isClone = true;
            RectTransform cloneRect = cloneGO.GetComponent<RectTransform>();

            // Parent the clone directly to the original so they move together seamlessly
            //cloneRect.SetParent(rectTransform);

            // Position the duplicate exactly at the end of the original text string
            cloneRect.anchoredPosition = new Vector2(0-spacing, 0);
        }

    }

    void Update()
    {
        // Continuously translate leftwards
        rectTransform.anchoredPosition += new Vector2(scrollSpeed * Time.deltaTime, 0);

        // The moment the core string clears the left side, reset position by its exact width interval
        if (rectTransform.anchoredPosition.x >= 0 + spacing)
        {
            // Modulo arithmetic or direct addition ensures fractional frame timing isn't lost (eliminates micro-stutter)
            rectTransform.anchoredPosition = new Vector2(-412.0546f + (spacing/2), 0);
        }
    }
}