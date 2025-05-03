// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.SceneManagement;

// public class TimerManager : MonoBehaviour
// {
//     public float easyTime = 90f;
//     public float mediumTime = 60f;
//     public float hardTime = 30f;
//     public static float difficultyPoints = 1.5f;
//     private float timeRemaining;
//     public GameObject paintingUI;
//     public AudioManager audioManager;
//     public Button nextButton;
//     public Button previousButton;
//     private bool timerRunning;
//     public static bool drawingCompleted = false;

//     void Start()
//     {
//         timeRemaining = mediumTime;
//         difficultyPoints = 1.5f;

//         if (GameManager.Instance.IsSecondUser())
//         {
//             timerRunning = false;
//             paintingUI.SetActive(false);
//             audioManager.StopAudio();
//             nextButton.gameObject.SetActive(true);
//             previousButton.gameObject.SetActive(true);
//             var persistentCanvas = CanvasPlacement.GetCanvas();
//             if (persistentCanvas != null) persistentCanvas.SetActive(true);
//         }
//         else
//         {
//             timerRunning = true;
//             nextButton.gameObject.SetActive(false);
//             previousButton.gameObject.SetActive(false);
//         }

//         nextButton.onClick.AddListener(GameManager.Instance.NextScene);
//         previousButton.onClick.AddListener(GameManager.Instance.PreviousScene);
//     }

//     void Update()
//     {
//         if (timerRunning)
//         {
//             timeRemaining -= Time.deltaTime;
//             if (timeRemaining <= 0)
//             {
//                 timerRunning = false;
//                 drawingCompleted = true;
//                 EndScene();
//             }
//         }
//     }

//     void EndScene()
//     {
//         FindObjectOfType<FadeTransition>().FadeToBlack(() =>
//         {
//             GameManager.Instance.NextScene();
//         });
//     }
// }

// using UnityEngine;

// public class TimeManager : MonoBehaviour
// {
//     public static float selectedTime = 20f; // 1.5 minutes
//     public static float difficultyPoints = 1.5f;
//     private float timeRemaining;
//     public GameObject paintingUI;
//     public AudioManager audioManager;
//     public GameObject promptUI;
//     public static bool drawingCompleted = false;
//     private bool timerRunning;
//     public static bool flag = false;

//     void Start()
//     {
//         timeRemaining = selectedTime;
//         timerRunning = true;
//     }

//     void Update()
//     {
//         if (timerRunning)
//         {
//             timeRemaining -= Time.deltaTime;
//             if (timeRemaining <= 0)
//             {
//                 timerRunning = false;
//                 drawingCompleted = true;
//                 EndScene();
//             }
//         }
//     }

//     void EndScene()
//     {
//         paintingUI.SetActive(false);
//         audioManager.StopAudio();
//         if (promptUI != null)
//         {
//             promptUI.SetActive(true);
//             if (promptUI.transform.childCount > 1)
//             {
//                 promptUI.transform.GetChild(0).gameObject.SetActive(true);
//                 promptUI.transform.GetChild(1).gameObject.SetActive(true);
//                 flag=true;
//             }
//             else
//             {
//                 Debug.LogError($"PromptUI has {promptUI.transform.childCount} children. Need at least 2.");
//             }
//         }
//         else
//         {
//             Debug.LogError("Failed to instantiate PromptUI.");
//         }
//     }
// }

using UnityEngine;
using System;
using System.Collections;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }
    public static float selectedTime = 20f; // Drawing duration
    public static float difficultyPoints = 1.5f;
    private float timeRemaining;
    public GameObject paintingUI;
    public AudioManager audioManager;
    public GameObject promptUI;
    public static bool drawingCompleted = false;
    private bool timerRunning;
    public static bool flag = false;

    // Events for drawing phase
    public event Action OnDrawingPhaseStarted;
    public event Action OnDrawingPhaseEnded;

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
    }

    void Start()
    {
        timeRemaining = selectedTime;
        timerRunning = true;
        drawingCompleted = false;
        flag = false;
        OnDrawingPhaseStarted?.Invoke();
        Debug.Log("TimeManager: Drawing phase started");
    }

    void Update()
    {
        if (timerRunning)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0)
            {
                timerRunning = false;
                drawingCompleted = true;
                OnDrawingPhaseEnded?.Invoke();
                Debug.Log("TimeManager: Drawing phase ended");
                EndScene();
            }
        }
    }

    void EndScene()
    {
        paintingUI.SetActive(false);
        audioManager.StopAudio();
        if (promptUI != null)
        {
            promptUI.SetActive(true);
            if (promptUI.transform.childCount > 1)
            {
                promptUI.transform.GetChild(0).gameObject.SetActive(true);
                promptUI.transform.GetChild(1).gameObject.SetActive(true);
                flag = true;
                Debug.Log("TimeManager: Activated promptUI for question selection");
            }
            else
            {
                Debug.LogError($"TimeManager: PromptUI has {promptUI.transform.childCount} children. Need at least 2.");
            }
        }
        else
        {
            Debug.LogError("TimeManager: Failed to instantiate PromptUI.");
        }
    }
}