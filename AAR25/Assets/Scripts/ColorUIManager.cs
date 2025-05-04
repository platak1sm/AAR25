using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class ColorUIManager : MonoBehaviour
{
    public static ColorUIManager Instance { get; private set; }

    [SerializeField] private GameObject colorUIPanel; // ColorUI Canvas
    [SerializeField] private GameObject blackBorder; // Black/Border
    [SerializeField] private GameObject greenBorder; // Green/Border
    [SerializeField] private GameObject redBorder; // Red/Border

    private InputAction yButtonAction; // Y button (left controller)
    private InputAction xButtonAction; // X button (left controller)
    private enum DrawingColor { Black, Green, Red }
    private DrawingColor currentColor = DrawingColor.Black;
    private readonly Color[] colors = { Color.black, Color.green, Color.red };
    private readonly GameObject[] borders = new GameObject[3];
    private bool isDrawingPhase = false;
    private float lastInputTime = 0f;
    private const float INPUT_COOLDOWN = 1f; // Updated to 1 second wait time

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Setup Input Actions for OpenXR
        yButtonAction = new InputAction("YButton", InputActionType.Button, "<XRController>{LeftHand}/secondaryButton");
        xButtonAction = new InputAction("XButton", InputActionType.Button, "<XRController>{LeftHand}/primaryButton");
        yButtonAction.Enable();
        xButtonAction.Enable();
    }

    void Start()
    {
        // Validate references
        if (colorUIPanel == null) { Debug.LogError("colorUIPanel not assigned"); enabled = false; return; }
        if (blackBorder == null) { Debug.LogError("blackBorder not assigned"); enabled = false; return; }
        if (greenBorder == null) { Debug.LogError("greenBorder not assigned"); enabled = false; return; }
        if (redBorder == null) { Debug.LogError("redBorder not assigned"); enabled = false; return; }

        // Initialize borders array
        borders[(int)DrawingColor.Black] = blackBorder;
        borders[(int)DrawingColor.Green] = greenBorder;
        borders[(int)DrawingColor.Red] = redBorder;

        // Set initial state
        colorUIPanel.SetActive(false);
        foreach (GameObject border in borders)
        {
            border.SetActive(false);
        }

        // Set default color
        UpdateDrawingColor();

        // Subscribe to TimeManager events
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnDrawingPhaseStarted += () =>
            {
                isDrawingPhase = true;
                yButtonAction.Enable();
                xButtonAction.Enable();
                Debug.Log("ColorUIManager: Drawing phase started, inputs enabled");
            };
            TimeManager.Instance.OnDrawingPhaseEnded += () =>
            {
                isDrawingPhase = false;
                yButtonAction.Disable();
                xButtonAction.Disable();
                Debug.Log("ColorUIManager: Drawing phase ended, inputs disabled");
            };
            isDrawingPhase = !TimeManager.drawingCompleted; // Initial state
            if (!isDrawingPhase)
            {
                yButtonAction.Disable();
                xButtonAction.Disable();
            }
            Debug.Log($"ColorUIManager: Subscribed to TimeManager events, isDrawingPhase: {isDrawingPhase}");
        }
        else
        {
            Debug.LogWarning("TimeManager not found, assuming drawing phase is always active");
            isDrawingPhase = true;
        }
    }

    void Update()
    {
        if (!isDrawingPhase || TimeManager.drawingCompleted || Time.time < lastInputTime + INPUT_COOLDOWN)
            return;

        // Y button: Next color
        bool yPressed = yButtonAction.WasPressedThisFrame() || Input.GetKeyDown(KeyCode.Y);
        if (yPressed)
        {
            Debug.Log($"Y button pressed. Switching to next color from {currentColor}");
            SwitchColorNext();
            lastInputTime = Time.time;
        }
        else if (yButtonAction.IsPressed())
        {
            Debug.Log("Y button held but not triggered (WasPressedThisFrame not satisfied)");
        }

        // X button: Previous color
        bool xPressed = xButtonAction.WasPressedThisFrame() || Input.GetKeyDown(KeyCode.X);
        if (xPressed)
        {
            Debug.Log($"X button pressed. Switching to previous color from {currentColor}");
            SwitchColorPrevious();
            lastInputTime = Time.time;
        }
        else if (xButtonAction.IsPressed())
        {
            Debug.Log("X button held but not triggered (WasPressedThisFrame not satisfied)");
        }
    }

    private void SwitchColorPrevious()
    {
        currentColor = (DrawingColor)(((int)currentColor - 1 + 3) % 3); // Black -> Red -> Green -> Black
        UpdateDrawingColor();
        StartCoroutine(ShowColorFeedback());
    }

    private void SwitchColorNext()
    {
        currentColor = (DrawingColor)(((int)currentColor + 1) % 3); // Black -> Green -> Red -> Black
        UpdateDrawingColor();
        StartCoroutine(ShowColorFeedback());
    }

    private void UpdateDrawingColor()
    {
        if (MXInkStylusHandler.Instance != null)
        {
            MXInkStylusHandler.Instance.SetDrawingColor(colors[(int)currentColor]);
            Debug.Log($"Drawing color set to {currentColor}");
        }
        else
        {
            Debug.LogWarning("MXInkStylusHandler not found, cannot set drawing color");
        }
    }

    private IEnumerator ShowColorFeedback()
    {
        // Activate ColorUI and selected border
        colorUIPanel.SetActive(true);
        borders[(int)currentColor].SetActive(true);

        // Wait 1 second
        yield return new WaitForSeconds(1f);

        // Deactivate ColorUI and border
        colorUIPanel.SetActive(false);
        borders[(int)currentColor].SetActive(false);
    }
}


//----------WHITE AS ERASER--------------

// using UnityEngine;
// using UnityEngine.InputSystem;
// using System.Collections;

// public class ColorUIManager : MonoBehaviour
// {
//     public static ColorUIManager Instance { get; private set; }

//     [SerializeField] private GameObject colorUIPanel; // ColorUI Canvas
//     [SerializeField] private GameObject blackBorder; // Black/Border
//     [SerializeField] private GameObject greenBorder; // Green/Border
//     [SerializeField] private GameObject redBorder; // Red/Border
//     [SerializeField] private GameObject eraserBorder; // Eraser/Border (new)

//     private InputAction yButtonAction; // Y button (left controller)
//     private InputAction xButtonAction; // X button (left controller)
//     private enum DrawingMode { Black, Green, Red, Eraser }
//     private DrawingMode currentMode = DrawingMode.Black;
//     private readonly Color[] colors = { Color.black, Color.green, Color.red, Color.white }; // Transparent for eraser
//     private readonly GameObject[] borders = new GameObject[4];
//     private bool isDrawingPhase = false;
//     private float lastInputTime = 0f;
//     private const float INPUT_COOLDOWN = 1f; // 1-second wait time
//     private bool isEraserActive = false;

//     void Awake()
//     {
//         if (Instance == null)
//         {
//             Instance = this;
//             DontDestroyOnLoad(gameObject);
//         }
//         else
//         {
//             Destroy(gameObject);
//         }

//         // Setup Input Actions for OpenXR
//         yButtonAction = new InputAction("YButton", InputActionType.Button, "<XRController>{LeftHand}/secondaryButton");
//         xButtonAction = new InputAction("XButton", InputActionType.Button, "<XRController>{LeftHand}/primaryButton");
//         yButtonAction.Enable();
//         xButtonAction.Enable();
//     }

//     void Start()
//     {
//         // Validate references
//         if (colorUIPanel == null) { Debug.LogError("colorUIPanel not assigned"); enabled = false; return; }
//         if (blackBorder == null) { Debug.LogError("blackBorder not assigned"); enabled = false; return; }
//         if (greenBorder == null) { Debug.LogError("greenBorder not assigned"); enabled = false; return; }
//         if (redBorder == null) { Debug.LogError("redBorder not assigned"); enabled = false; return; }
//         if (eraserBorder == null) { Debug.LogError("eraserBorder not assigned"); enabled = false; return; }

//         // Initialize borders array
//         borders[(int)DrawingMode.Black] = blackBorder;
//         borders[(int)DrawingMode.Green] = greenBorder;
//         borders[(int)DrawingMode.Red] = redBorder;
//         borders[(int)DrawingMode.Eraser] = eraserBorder;

//         // Set initial state
//         colorUIPanel.SetActive(false);
//         foreach (GameObject border in borders)
//         {
//             border.SetActive(false);
//         }

//         // Set default color
//         UpdateDrawingMode();

//         // Subscribe to TimeManager events
//         if (TimeManager.Instance != null)
//         {
//             TimeManager.Instance.OnDrawingPhaseStarted += () =>
//             {
//                 isDrawingPhase = true;
//                 yButtonAction.Enable();
//                 xButtonAction.Enable();
//                 Debug.Log("ColorUIManager: Drawing phase started, inputs enabled");
//             };
//             TimeManager.Instance.OnDrawingPhaseEnded += () =>
//             {
//                 isDrawingPhase = false;
//                 yButtonAction.Disable();
//                 xButtonAction.Disable();
//                 Debug.Log("ColorUIManager: Drawing phase ended, inputs disabled");
//             };
//             isDrawingPhase = !TimeManager.drawingCompleted; // Initial state
//             if (!isDrawingPhase)
//             {
//                 yButtonAction.Disable();
//                 xButtonAction.Disable();
//             }
//             Debug.Log($"ColorUIManager: Subscribed to TimeManager events, isDrawingPhase: {isDrawingPhase}");
//         }
//         else
//         {
//             Debug.LogWarning("TimeManager not found, assuming drawing phase is always active");
//             isDrawingPhase = true;
//         }
//     }

//     void Update()
//     {
//         if (!isDrawingPhase || TimeManager.drawingCompleted || Time.time < lastInputTime + INPUT_COOLDOWN)
//             return;

//         // Y button: Toggle eraser
//         bool yPressed = yButtonAction.WasPressedThisFrame() || Input.GetKeyDown(KeyCode.Y);
//         if (yPressed)
//         {
//             Debug.Log($"Y button pressed. Toggling eraser from {currentMode}");
//             ToggleEraser();
//             lastInputTime = Time.time;
//         }
//         else if (yButtonAction.IsPressed())
//         {
//             Debug.Log("Y button held but not triggered (WasPressedThisFrame not satisfied)");
//         }

//         // X button: Cycle colors
//         bool xPressed = xButtonAction.WasPressedThisFrame() || Input.GetKeyDown(KeyCode.X);
//         if (xPressed)
//         {
//             Debug.Log($"X button pressed. Switching to next color from {currentMode}");
//             SwitchColorNext();
//             lastInputTime = Time.time;
//         }
//         else if (xButtonAction.IsPressed())
//         {
//             Debug.Log("X button held but not triggered (WasPressedThisFrame not satisfied)");
//         }
//     }

//     private void SwitchColorNext()
//     {
//         if (!isEraserActive)
//         {
//             currentMode = (DrawingMode)(((int)currentMode + 1) % 3); // Cycle Black, Green, Red
//         }
//         else
//         {
//             currentMode = DrawingMode.Black; // Default to Black when exiting eraser mode
//             isEraserActive = false;
//         }
//         UpdateDrawingMode();
//         StartCoroutine(ShowColorFeedback());
//     }

//     private void ToggleEraser()
//     {
//         isEraserActive = !isEraserActive;
//         currentMode = isEraserActive ? DrawingMode.Eraser : DrawingMode.Black; // Default back to Black when exiting eraser
//         UpdateDrawingMode();
//         StartCoroutine(ShowColorFeedback());
//     }

//     private void UpdateDrawingMode()
//     {
//         if (MXInkStylusHandler.Instance != null)
//         {
//             MXInkStylusHandler.Instance.SetDrawingColor(colors[(int)currentMode]);
//             Debug.Log($"Drawing mode set to {currentMode}");
//         }
//         else
//         {
//             Debug.LogWarning("MXInkStylusHandler not found, cannot set drawing mode");
//         }
//     }

//     private IEnumerator ShowColorFeedback()
//     {
//         // Activate ColorUI and selected border
//         colorUIPanel.SetActive(true);
//         foreach (GameObject border in borders)
//         {
//             border.SetActive(false);
//         }
//         borders[(int)currentMode].SetActive(true);

//         // Wait 1 second
//         yield return new WaitForSeconds(1f);

//         // Deactivate ColorUI and border
//         colorUIPanel.SetActive(false);
//         borders[(int)currentMode].SetActive(false);
//     }
// }