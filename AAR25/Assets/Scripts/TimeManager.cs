using UnityEngine;
using UnityEngine.UI;
using System;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }
    public static float selectedTime = 5f; // Set by DifficultySelector, default to 60
    public static float difficultyPoints = 1.5f;
    private float timeRemaining;
    //public GameObject paintingUI; // Optional placeholder, can be removed
    public AudioManager audioManager;
    public GameObject promptUI;
    public GameObject questionUI;
    public static bool drawingCompleted = false;
    private bool timerRunning;
    public static bool flag = false;

    public GameObject[] paintingInsp = new GameObject[3]; // paintinginsp1, paintinginsp2, paintinginsp3
    public AudioClip[] audioClips = new AudioClip[3]; // audio1, audio2, audio3
    private int currentPhase = 0; // 0, 1, 2 for the three phases

    public event Action OnDrawingPhaseStarted;
    public event Action OnDrawingPhaseEnded;

    public int CurrentPhase => currentPhase; // Public property to access currentPhase

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
        // Fallback to start the first phase if no GameManager is present
        if (!timerRunning)
        {
            RestartTimer();
            Debug.Log("TimeManager: Starting Phase 1 automatically (no GameManager detected)");
        }
    }

    public void RestartTimer()
    {
        timeRemaining = selectedTime;
        timerRunning = true;
        drawingCompleted = false;
        flag = false;
        OnDrawingPhaseStarted?.Invoke();
        Debug.Log($"TimeManager: Drawing phase started for Phase {currentPhase + 1}, timeRemaining: {timeRemaining}");

        // paintingUI is optional; remove if not used
        //if (paintingUI != null) paintingUI.SetActive(true);
        if (promptUI != null) promptUI.SetActive(false);
        if (questionUI != null) questionUI.SetActive(false);
        if (paintingInsp[currentPhase] != null)
        {
            paintingInsp[currentPhase].SetActive(true);
            Debug.Log($"TimeManager: Activated PaintingInsp{currentPhase + 1}");
        }
        else
        {
            Debug.LogError($"TimeManager: paintingInsp[{currentPhase}] is not assigned!");
        }
        if (audioManager != null && audioClips[currentPhase] != null)
        {
            audioManager.PlayAudio(audioClips[currentPhase]);
            Debug.Log($"TimeManager: Playing audio clip {audioClips[currentPhase].name} for Phase {currentPhase + 1}");
        }
        else
        {
            Debug.LogError("TimeManager: AudioManager or audioClips[currentPhase] is not assigned!");
        }
    }

    void Update()
    {
        if (timerRunning)
        {
            timeRemaining -= Time.deltaTime;
            Debug.Log($"TimeManager: Time remaining for Phase {currentPhase + 1}: {timeRemaining}");
            if (timeRemaining <= 0)
            {
                timerRunning = false;
                drawingCompleted = true;
                OnDrawingPhaseEnded?.Invoke();
                Debug.Log($"TimeManager: Drawing phase ended for Phase {currentPhase + 1}");
                //StoreDrawing();
                NextPhase();
            }
        }
    }

    // void StoreDrawing()
    // {
    //     if (MXInkStylusHandler.Instance != null)
    //     {
    //         MXInkStylusHandler.Instance.SaveDrawing(); // Trigger save from stylus handler
    //         Debug.Log($"TimeManager: Saved drawing for Phase {currentPhase + 1}");
    //     }
    //     //if (paintingUI != null) paintingUI.SetActive(false); // Optional
    // }

    void NextPhase()
    {
        if (currentPhase < 2)
        {
            if (paintingInsp[currentPhase] != null)
            {
                paintingInsp[currentPhase].SetActive(false);
                Debug.Log($"TimeManager: Deactivated PaintingInsp{currentPhase + 1}");
            }
            if (audioManager != null && audioClips[currentPhase] != null)
            {
                audioManager.StopAudio();
                Debug.Log($"TimeManager: Stopped audio for Phase {currentPhase + 1}");
            }
            currentPhase++;
            RestartTimer();
        }
        else
        {
            if (paintingInsp[2] != null)
            {
                paintingInsp[2].SetActive(false);
                Debug.Log("TimeManager: Deactivated PaintingInsp3");
            }
            if (audioManager != null && audioClips[2] != null)
            {
                audioManager.StopAudio();
                Debug.Log("TimeManager: Stopped audio for Phase 3");
            }
            StartCoroutine(ShowPromptThenQuestions());
        }
    }

    System.Collections.IEnumerator ShowPromptThenQuestions()
    {
        if (promptUI != null)
        {
            promptUI.SetActive(true);
            Debug.Log("TimeManager: Activated PromptUI after Phase 3");
            yield return new WaitForSeconds(3f);
            promptUI.SetActive(false);
            Debug.Log("TimeManager: Deactivated PromptUI");
        }
        else
        {
            Debug.LogError("TimeManager: PromptUI is not assigned!");
        }

        if (questionUI != null)
        {
            questionUI.SetActive(true);
            Debug.Log("TimeManager: Activated QuestionUI");
            // Ensure the first painting and audio panels are active
            QuestionUI questionUIScript = questionUI.GetComponent<QuestionUI>();
            if (questionUIScript != null)
            {
                questionUIScript.UpdatePaintingDisplay();
                Debug.Log("TimeManager: Called UpdatePaintingDisplay on QuestionUI");
            }
        }
        else
        {
            Debug.LogError("TimeManager: QuestionUI is not assigned!");
        }
    }
}