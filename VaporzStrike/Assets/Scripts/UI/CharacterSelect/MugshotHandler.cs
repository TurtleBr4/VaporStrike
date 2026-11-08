using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class MugshotHandler : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public RectTransform element;
    private Selectable selectable;
    public int charID = 0;
    public int selectedCursor = 0;
    public Image mugshot;
    public PlayerData playerData;

    public TV displayOnTV;

    public bool isStage = false;

    private EventTrigger EventTrigger;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //element = GameObject.Find("CursorP1").GetComponent<RectTransform>();
        selectable = GetComponent<Selectable>();
        playerData = GameManager.instance.GetPlayerByID(charID);
        if (!playerData)
        {
            gameObject.SetActive(false);
            return;
        }
        if (!playerData.mugshot.Equals(null) && !isStage)
        {

            mugshot.sprite = playerData.mugshot;

            GetComponentInChildren<TextMeshProUGUI>().gameObject.SetActive(false);
        }
        else if (isStage) { if(GetComponent<Image>().sprite != null) GetComponentInChildren<TextMeshProUGUI>().gameObject.SetActive(false); }
        else { mugshot.sprite = null; GetComponentInChildren<TextMeshProUGUI>().gameObject.SetActive(true); }

        EventTrigger = gameObject.GetComponent<EventTrigger>();
    }

    // Update is called once per frame
    void Update()
    {

        //if (AreRectsOverlapping(GetComponent<RectTransform>(), element)) { EventSystem.current.SetSelectedGameObject(this.gameObject); }


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
        Debug.Log("Updating TV");
        if (displayOnTV != null && GetComponent<Image>().sprite != null) { displayOnTV.SetScreen(GetComponent<Image>().sprite, new Color(0.5f, 0.5f, 0.5f)); }
        if (!selectable.interactable || isStage) return;
        if (CharacterSelect.instance.selectMode == CharacterSelect.SelectMode.Character)
        {
            Debug.Log(this.gameObject.name + " was selected");
            GameManager.instance.pickPlayer(selectedCursor, charID);
        }

    }
    public void UpdateTV()
    {

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
