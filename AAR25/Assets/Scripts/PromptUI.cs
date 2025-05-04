using UnityEngine;
using TMPro;

public class PromptUI : MonoBehaviour
{
    public TextMeshProUGUI promptText;

    void Start()
    {
        if (promptText != null)
        {
            promptText.text = "Drawing Phases Complete! Now Answer the Questions.";
        }
        
    }
}