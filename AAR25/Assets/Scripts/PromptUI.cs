using UnityEngine;
using TMPro;

public class PromptUI : MonoBehaviour
{
    public TextMeshProUGUI promptText;
    public GameObject questionUI;
    public GameObject prompt;

    void Start()
    {
       
    }

    private bool lastFlag = false;
    private float flagTrueTime = -1f;
    private bool actionTriggered = false;

    void Update()
    {
        // Detect change from false to true
        if (TimeManager.flag && !lastFlag)
        {
            flagTrueTime = Time.time;
            actionTriggered = false; // Reset the trigger
            Debug.Log("Flag turned true, timer started");
        }

        // If flag is still true and 10 seconds have passed, trigger the action once
        if (TimeManager.flag && !actionTriggered && Time.time - flagTrueTime >= 5f)
        {
            Debug.Log("10 seconds passed since flag turned true. Triggering action...");

            prompt.gameObject.SetActive(false);

            if (questionUI != null)
            {    
                questionUI.transform.GetChild(0).gameObject.SetActive(true); 
                questionUI.transform.GetChild(1).gameObject.SetActive(true);
                questionUI.transform.GetChild(4).gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError("QuestionUI is null.");
            }

            actionTriggered = true; // Prevent retriggering
        }

        lastFlag = TimeManager.flag; // Store flag state for next frame
    }
}   