using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class LevelPreview : MonoBehaviour
{
    public TextMeshProUGUI selectedLevelText;
    public Image bg;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(CharacterSelect.instance.selectedLevel != null && CharacterSelect.instance.selectedLevel != "")
        {
            selectedLevelText.text = "SelectedLevel: " + CharacterSelect.instance.selectedLevel;
            bg.enabled = true;
        }
        else
        {
            selectedLevelText.text = "";
            bg.enabled = false;
        }

    }
}
