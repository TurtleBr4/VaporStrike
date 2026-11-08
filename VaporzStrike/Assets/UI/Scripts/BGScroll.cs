using UnityEngine;
using UnityEngine.UI;

public class BGScroll : MonoBehaviour
{
    [SerializeField] private float scrollX = 0;
    [SerializeField] private float scrollY = 0;
    private RawImage img;
    void Start()
    {
        img = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        Rect currUV = img.uvRect;
        currUV.position += new Vector2(scrollX, scrollY);
        img.uvRect = currUV;
    }
}
