using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class DifficultySelector : MonoBehaviour
{
    public Button[] difficultyButtons;
    public TextMeshProUGUI instructionText;
    private int currentDifficultyIndex = 0;
    private InputAction yButtonAction;
    private InputAction xButtonAction;

    void Awake()
    {
        yButtonAction = new InputAction("YButton", InputActionType.Button, "<XRController>{LeftHand}/secondaryButton");
        xButtonAction = new InputAction("XButton", InputActionType.Button, "<XRController>{LeftHand}/primaryButton");
        yButtonAction.Enable();
        xButtonAction.Enable();
    }

    void Start()
    {
        if (difficultyButtons == null || difficultyButtons.Length != 3)
        {
            Debug.LogError($"difficultyButtons array is invalid. Length: {(difficultyButtons == null ? 0 : difficultyButtons.Length)}, Expected: 3");
            enabled = false;
            return;
        }
        for (int i = 0; i < difficultyButtons.Length; i++)
        {
            if (difficultyButtons[i] == null)
            {
                Debug.LogError($"difficultyButtons[{i}] is null");
                enabled = false;
                return;
            }
        }

        if (instructionText == null)
        {
            Debug.LogError("instructionText is not assigned");
            enabled = false;
            return;
        }

        UpdateDifficultyDisplay();
        instructionText.text = "Select the difficulty level. Use button Y to cycle through options and button X to confirm.";
    }

    void Update()
    {
        bool yPressed = yButtonAction != null && yButtonAction.WasPressedThisFrame() || Input.GetKeyDown(KeyCode.Y);
        if (yPressed)
        {
            currentDifficultyIndex = (currentDifficultyIndex + 1) % difficultyButtons.Length;
            UpdateDifficultyDisplay();
            instructionText.text = $"Difficulty: {GetDifficultyName(currentDifficultyIndex)}. Confirm with X.";
        }

        bool xPressed = xButtonAction != null && xButtonAction.WasPressedThisFrame() || Input.GetKeyDown(KeyCode.X);
        if (xPressed)
        {
            SetDifficultyAndProceed();
        }
    }

    void UpdateDifficultyDisplay()
    {
        for (int i = 0; i < difficultyButtons.Length; i++)
        {
            difficultyButtons[i].image.color = i == currentDifficultyIndex ? Color.yellow : Color.white;
        }
    }

    string GetDifficultyName(int index)
    {
        switch (index)
        {
            case 0: return "Easy";
            case 1: return "Medium";
            case 2: return "Difficult";
            default: return "Unknown";
        }
    }

    void SetDifficultyAndProceed()
    {
        switch (currentDifficultyIndex)
        {
            case 0: // Easy
                TimeManager.selectedTime = 30f; // 1.5 minutes
                break;
            case 1: // Medium
                TimeManager.selectedTime = 20f; // 1 minute
                break;
            case 2: // Difficult
                TimeManager.selectedTime = 10f; // 0.5 minutes
                break;
        }
        Debug.Log($"DifficultySelector: Set TimeManager.selectedTime to {TimeManager.selectedTime} seconds");

        SceneController.Instance.StartMainScene(); // Updated from StartDrawingPhase
    }

    void OnDestroy()
    {
        if (yButtonAction != null)
        {
            yButtonAction.Disable();
            yButtonAction.Dispose();
        }
        if (xButtonAction != null)
        {
            xButtonAction.Disable();
            xButtonAction.Dispose();
        }
    }
}