using UnityEngine;
using UnityEngine.UI;
public class TV : MonoBehaviour
{
    private Image m_image;
    private Color m_color;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetScreen(Sprite sprite, Color color)
    {
        m_image.sprite = sprite;
        m_image.color = color;
    }
    public void ResetScreen()
    {
        m_image.sprite = null;
        m_image.color = new Color(0,0,0);
    }
}
