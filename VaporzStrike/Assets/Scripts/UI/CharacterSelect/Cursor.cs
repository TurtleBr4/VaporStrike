using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
public class CursorVirt : MonoBehaviour
{
    PlayerInput playerControls;
    public Vector2 dir;
    public float speed = 1f;
    public static int cursorID = 0;
    public RectTransform element;
    public GraphicRaycaster _graphicRaycaster;
    [SerializeField] private int _id = 0;
    [SerializeField] private RectTransform r;

    public Vector2 retroRes = new Vector2(480, 272);
    public Vector2 screenRes = new Vector2(1920,1080);

    public Vector2 sizeRatio = Vector2.zero;

    public Vector2 currentPos = Vector2.zero;
    public Vector2 finalPos = Vector2.zero;
    private TextMeshProUGUI idText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        playerControls = GetComponent<PlayerInput>();
        //playerControls.DeactivateInput();
        if (GameObject.Find("CharacterSelectCanvas"))
        {
            _graphicRaycaster = GameObject.Find("CharacterSelectCanvas").GetComponentInChildren<GraphicRaycaster>();
        }
        else
        {
            _graphicRaycaster = GameObject.Find("Canvas").GetComponentInChildren<GraphicRaycaster>();
        }

            r = GetComponent<RectTransform>();
    }
    private void Start()
    {
        idText = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void SetID(int id)
    {
        cursorID = id;
        _id = id;
    }
    public void Reset()
    {
        _id = 0;
    }
    // Update is called once per frame
    void Update()
    {
        r.localScale = new Vector3(9.8f, 9.8f, 9.8f);
        idText.text = (cursorID+1).ToString();
        playerControls.camera = Camera.main;
        r.anchoredPosition += dir * speed;

        //screenRes = new Vector2 (Screen.width, Screen.height);
        sizeRatio = retroRes/screenRes;

        currentPos = r.anchoredPosition;
        finalPos = r.anchoredPosition * sizeRatio;
    var pointer = new PointerEventData(EventSystem.current) { position = finalPos };
        if (GameManager.instance != null)
        {
            //GameManager.instance.DebugTxt = pointer.position.ToString();
        }
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        _graphicRaycaster.Raycast(pointer, raycastResults);
        //Debug.Log(raycastResults.Count);
        if (GameManager.instance != null)
        {
            //GameManager.instance.DebugTxt = raycastResults.Count.ToString();
        }
        foreach (RaycastResult raycastResult in raycastResults)
        {
            
            var ui = raycastResult.gameObject.GetComponent<UIBehaviour>();
            if (ui)
            {
                ExecuteEvents.Execute(raycastResult.gameObject, pointer, ExecuteEvents.pointerEnterHandler);
                ExecuteEvents.Execute(raycastResult.gameObject, pointer, ExecuteEvents.pointerExitHandler);
                if (Gamepad.current.buttonSouth.wasPressedThisFrame)
                {
                    Debug.Log($"Clicked on {raycastResult.gameObject}");
                    if (raycastResult.gameObject.GetComponentInChildren<MugshotHandler>() != null)
                    {
                        raycastResult.gameObject.GetComponentInChildren<MugshotHandler>().SelectChar(_id);
                    }

                    ExecuteEvents.Execute(raycastResult.gameObject, pointer, ExecuteEvents.pointerClickHandler);
                    
                    
                }
            }
            else
            {
                if (GameManager.instance != null)
                {
                   // GameManager.instance.DebugTxt = $"no ui";
                }
            }
        }
    }
   
    public void OnMove(InputAction.CallbackContext ctx)
    {

         dir = ctx.ReadValue<Vector2>();
    }

    public static bool AreRectsOverlapping(RectTransform rect1, RectTransform rect2)
    {
        if (rect1 == null || rect2 == null)
        {
            return false;
        }

        // Get world corners of both RectTransforms
        Vector3[] corners1 = new Vector3[4];
        rect1.GetWorldCorners(corners1);
        Rect r1 = new Rect(corners1[0].x, corners1[0].y, corners1[2].x - corners1[0].x, corners1[2].y - corners1[0].y);

        Vector3[] corners2 = new Vector3[4];
        rect2.GetWorldCorners(corners2);
        Rect r2 = new Rect(corners2[0].x, corners2[0].y, corners2[2].x - corners2[0].x, corners2[2].y - corners2[0].y);

        // Use the built-in Overlaps method
        return r1.Overlaps(r2);
    }
}
