using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.UIElements.Experimental;
using TMPro;

public class MugshotHandler : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public RectTransform element;
    private Selectable selectable;
    public int charID = 0;
    public int selectedCursor = 0;
    public Image mugshot;
    public PlayerData playerData;

    public bool isStage = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //element = GameObject.Find("CursorP1").GetComponent<RectTransform>();
        selectable = GetComponent<Selectable>();
        playerData = GameManager.instance.GetPlayerByID(charID);
    }

    // Update is called once per frame
    void Update()
    {

        //if (AreRectsOverlapping(GetComponent<RectTransform>(), element)) { EventSystem.current.SetSelectedGameObject(this.gameObject); }
        if (!playerData)
        {
            gameObject.SetActive(false);
        }
        if (!playerData.mugshot.Equals(null) && !isStage)
        {

            mugshot.sprite = playerData.mugshot;

           GetComponentInChildren<TextMeshProUGUI>().gameObject.SetActive(false);
        }
        else if (isStage) { }
        else { mugshot.sprite = null; GetComponentInChildren<TextMeshProUGUI>().gameObject.SetActive(true); }


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

    public void OnSelect(BaseEventData eventData)
    {
        if (!selectable.interactable) return;
        if (CharacterSelect.instance.selectMode == CharacterSelect.SelectMode.Character)
        {
            Debug.Log(this.gameObject.name + " was selected");
            GameManager.instance.pickPlayer(selectedCursor, charID);
        }

    }
    public void SelectChar(int cursorID)
    {
        selectedCursor = cursorID;
    }
    public void OnDeselect(BaseEventData eventData)
    {
        Debug.Log(this.gameObject.name + " was deselected");
    }
}
