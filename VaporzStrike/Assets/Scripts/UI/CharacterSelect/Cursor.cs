using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class Cursor : MonoBehaviour
{
    PlayerInput playerControls;
    public Vector2 dir;
    public float speed = 1f;
    public static int cursorID = 0;
    public RectTransform element;
    public GraphicRaycaster _graphicRaycaster;
    [SerializeField] private int _id = 0;
    [SerializeField] private RectTransform r;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        _id = cursorID++;
        //playerControls.DeactivateInput();
        _graphicRaycaster = GameObject.Find("Canvas").GetComponentInChildren<GraphicRaycaster>();
        r = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        r.anchoredPosition += dir * speed;
        var pointer = new PointerEventData(EventSystem.current) { position = r.anchoredPosition };
       
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        _graphicRaycaster.Raycast(pointer, raycastResults);
        Debug.Log(raycastResults.Count);
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
