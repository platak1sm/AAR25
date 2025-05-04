// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using System.Collections;
// using UnityEngine.InputSystem;

// public class QuestionUI : MonoBehaviour
// {
//     public TextMeshProUGUI resultText;
//     public TextMeshProUGUI correctText;
//     public TextMeshProUGUI incorrectText;
//     public RawImage[] paintingImages; // 3 RawImage components for paintings
//     public Button[] audioButtons; // 4 audio buttons
//     public AudioClip[] audioClips; // 4 audio clips
//     public GameObject paintingPanel; // PaintingPanel GameObject
//     public GameObject audioPanel; // AudioPanel GameObject
//     public GameObject leaderboardPanel; // LeaderboardPanel GameObject
//     public TextMeshProUGUI leaderboardText; // Text to display leaderboard
//     private AudioSource audioSource;

//     private int currentPaintingIndex = 0;
//     private int selectedPaintingIndex = -1;
//     private int currentAudioIndex = 0;
//     private int selectedAudioIndex = -1;
//     private bool selectingPainting = true; // True: selecting painting, False: selecting audio
//     private bool isPaintingCorrect = false;
//     private bool isAudioCorrect = false;
//     private int correctPaintingIndex = 0;
//     private int correctAudioIndex = 0;

//     private InputAction yButtonAction;
//     private InputAction xButtonAction;

//     void Awake()
//     {
//         // Setup Input Actions for OpenXR
//         yButtonAction = new InputAction("YButton", InputActionType.Button, "<XRController>{LeftHand}/secondaryButton");
//         xButtonAction = new InputAction("XButton", InputActionType.Button, "<XRController>{LeftHand}/primaryButton");
//         yButtonAction.Enable();
//         xButtonAction.Enable();
//     }

//     void Start()
//     {
//         // Initialize AudioSource
//         audioSource = GetComponent<AudioSource>();
//         if (audioSource == null)
//         {
//             audioSource = gameObject.AddComponent<AudioSource>();
//         }
//         audioSource.volume = 1.0f;
//         audioSource.spatialBlend = 0.0f; // 2D audio
//         audioSource.playOnAwake = false;

//         // Validate paintingImages
//         if (paintingImages == null || paintingImages.Length != 3)
//         {
//             Debug.LogError($"paintingImages array is invalid. Length: {(paintingImages == null ? 0 : paintingImages.Length)}, Expected: 3");
//             enabled = false;
//             return;
//         }
//         for (int i = 0; i < paintingImages.Length; i++)
//         {
//             if (paintingImages[i] == null)
//             {
//                 Debug.LogError($"paintingImages[{i}] is null");
//                 enabled = false;
//                 return;
//             }
//             if (paintingImages[i].texture == null)
//             {
//                 Debug.LogWarning($"paintingImages[{i}] has no texture assigned");
//             }
//         }

//         // Validate paintingPanel
//         if (paintingPanel == null)
//         {
//             Debug.LogError("paintingPanel is not assigned");
//             enabled = false;
//             return;
//         }

//         // Validate audioClips
//         if (audioClips == null || audioClips.Length != 4)
//         {
//             Debug.LogError($"audioClips array is invalid. Length: {(audioClips == null ? 0 : audioClips.Length)}, Expected: 4");
//             enabled = false;
//             return;
//         }
//         for (int i = 0; i < audioClips.Length; i++)
//         {
//             if (audioClips[i] == null)
//             {
//                 Debug.LogWarning($"audioClips[{i}] is null");
//             }
//         }

//         // Get correct painting index
//         if (PaintingSelector.Instance != null)
//         {
//             correctPaintingIndex = PaintingSelector.Instance.CurrentPaintingIndex;
//             Debug.Log($"Correct painting index from PaintingSelector: {correctPaintingIndex}");
//         }
//         else
//         {
//             Debug.LogWarning("PaintingSelector not found. Using default correctPaintingIndex: 0");
//         }

//         // Get correct audio index
//         if (AudioManager.Instance != null && AudioManager.Instance.sceneAudio != null)
//         {
//             correctAudioIndex = System.Array.IndexOf(audioClips, AudioManager.Instance.sceneAudio);
//             if (correctAudioIndex == -1)
//             {
//                 Debug.LogWarning("AudioManager.sceneAudio not found in audioClips. Using default correctAudioIndex: 0");
//                 correctAudioIndex = 0;
//             }
//             else
//             {
//                 Debug.Log($"Correct audio index from AudioManager: {correctAudioIndex}");
//             }
//         }
//         else
//         {
//             Debug.LogWarning("AudioManager or sceneAudio not found. Using default correctAudioIndex: 0");
//         }

//         // Initialize UI
//         UpdatePaintingDisplay();
//         resultText.text = "Select the painting that inspired the first user. Go through each painting using button Y and press button X when you want to select it.";
//         Debug.Log($"Initialized QuestionUI. paintingImages.Length: {paintingImages.Length}, currentPaintingIndex: {currentPaintingIndex}");

//         // Subscribe to TimeManager events to control input activation
//         if (TimeManager.Instance != null)
//         {
//             TimeManager.Instance.OnDrawingPhaseStarted += () =>
//             {
//                 yButtonAction.Disable();
//                 xButtonAction.Disable();
//                 Debug.Log("QuestionUI: Drawing phase started, inputs disabled");
//             };
//             TimeManager.Instance.OnDrawingPhaseEnded += () =>
//             {
//                 yButtonAction.Enable();
//                 xButtonAction.Enable();
//                 Debug.Log("QuestionUI: Drawing phase ended, inputs enabled");
//             };
//             // Set initial input state
//             if (!TimeManager.drawingCompleted)
//             {
//                 yButtonAction.Disable();
//                 xButtonAction.Disable();
//             }
//         }
//         else
//         {
//             Debug.LogWarning("TimeManager not found, QuestionUI inputs may be active during drawing phase");
//         }
//     }

//     void Update()
//     {
//         // Only process inputs if QuestionUI is active and drawing phase is complete
//         if (!gameObject.activeSelf || !TimeManager.drawingCompleted)
//         {
//             return;
//         }

//         // Skip input if coroutine is running (during feedback delay)
//         if (correctText.gameObject.activeSelf || incorrectText.gameObject.activeSelf)
//         {
//             Debug.Log("Input skipped: Feedback text active");
//             return;
//         }

//         // Y button: Cycle options
//         bool yPressed = yButtonAction.WasPressedThisFrame() || Input.GetKeyDown(KeyCode.Y);
//         if (yPressed)
//         {
//             Debug.Log($"Y button pressed. selectingPainting: {selectingPainting}");
//             if (selectingPainting)
//             {
//                 currentPaintingIndex = (currentPaintingIndex + 1) % paintingImages.Length;
//                 UpdatePaintingDisplay();
//                 resultText.text = $"Painting {currentPaintingIndex + 1}. Confirm with X.";
//                 Debug.Log($"Cycled painting. currentPaintingIndex: {currentPaintingIndex}");
//             }
//             else
//             {
//                 currentAudioIndex = (currentAudioIndex + 1) % audioButtons.Length;
//                 UpdateAudioDisplay();
//                 if (audioClips[currentAudioIndex] != null)
//                 {
//                     audioSource.Stop();
//                     audioSource.PlayOneShot(audioClips[currentAudioIndex]);
//                     Debug.Log($"Playing audio: {audioClips[currentAudioIndex].name}");
//                 }
//                 else
//                 {
//                     Debug.LogWarning($"audioClips[{currentAudioIndex}] is null, cannot play");
//                 }
//                 resultText.text = $"Audio {currentAudioIndex + 1}. Confirm with X.";
//                 Debug.Log($"Cycled audio. currentAudioIndex: {currentAudioIndex}");
//             }
//         }
//         else if (yButtonAction.IsPressed())
//         {
//             Debug.Log("Y button held but not triggered (WasPressedThisFrame not satisfied)");
//         }

//         // X button: Select option
//         bool xPressed = xButtonAction.WasPressedThisFrame() || Input.GetKeyDown(KeyCode.X);
//         if (xPressed)
//         {
//             Debug.Log("X button pressed");
//             if (selectingPainting)
//             {
//                 selectedPaintingIndex = currentPaintingIndex;
//                 isPaintingCorrect = selectedPaintingIndex == correctPaintingIndex;
//                 StartCoroutine(ShowPaintingFeedback());
//                 Debug.Log($"Selected painting {selectedPaintingIndex}. Correct: {isPaintingCorrect}");
//             }
//             else
//             {
//                 selectedAudioIndex = currentAudioIndex;
//                 isAudioCorrect = selectedAudioIndex == correctAudioIndex;
//                 StartCoroutine(ShowAudioFeedbackAndScore());
//                 Debug.Log($"Selected audio {selectedAudioIndex}. Correct: {isAudioCorrect}");
//             }
//         }
//         else if (xButtonAction.IsPressed())
//         {
//             Debug.Log("X button held but not triggered (WasPressedThisFrame not satisfied)");
//         }
//     }

//     void UpdatePaintingDisplay()
//     {
//         for (int i = 0; i < paintingImages.Length; i++)
//         {
//             bool isActive = i == currentPaintingIndex;
//             paintingPanel.transform.GetChild(i).gameObject.SetActive(isActive);
//             Debug.Log($"PaintingImage{i + 1} active: {isActive}, Texture: {(paintingImages[i].texture != null ? paintingImages[i].texture.name : "None")}");
//         }
//     }

//     void UpdateAudioDisplay()
//     {
//         for (int i = 0; i < audioButtons.Length; i++)
//         {
//             audioButtons[i].image.color = i == currentAudioIndex ? Color.yellow : Color.white;
//         }
//     }

//     IEnumerator ShowPaintingFeedback()
//     {
//         // Show feedback for 3 seconds
//         correctText.gameObject.SetActive(isPaintingCorrect);
//         incorrectText.gameObject.SetActive(!isPaintingCorrect);
//         yield return new WaitForSeconds(3f);
//         correctText.gameObject.SetActive(false);
//         incorrectText.gameObject.SetActive(false);

//         // Transition to audio selection
//         selectingPainting = false;
//         paintingPanel.SetActive(false);
//         audioPanel.SetActive(true);
//         UpdateAudioDisplay();
//         // Stop AudioManager audio to avoid conflicts
//         if (AudioManager.Instance != null)
//         {
//             AudioManager.Instance.StopAudio();
//         }
//         if (audioClips[currentAudioIndex] != null)
//         {
//             audioSource.Stop();
//             audioSource.PlayOneShot(audioClips[currentAudioIndex]);
//             Debug.Log($"Playing initial audio: {audioClips[currentAudioIndex].name}");
//         }
//         else
//         {
//             Debug.LogWarning($"audioClips[{currentAudioIndex}] is null, cannot play initial audio");
//         }
//         resultText.text = "Select the audio that inspired the first user. Go through each audio using button Y and press button X when you want to select it.";
//     }

//     IEnumerator ShowAudioFeedbackAndScore()
//     {
//         // Show feedback for 3 seconds
//         correctText.gameObject.SetActive(isAudioCorrect);
//         incorrectText.gameObject.SetActive(!isAudioCorrect);
//         yield return new WaitForSeconds(3f);
//         correctText.gameObject.SetActive(false);
//         incorrectText.gameObject.SetActive(false);

//         // Show score
//         int score = (isPaintingCorrect ? 1 : 0) + (isAudioCorrect ? 1 : 0);
//         resultText.text = $"Score: {score}/2";

//         // Submit score to leaderboard
//         string playerName = $"Team_{System.DateTime.Now.Ticks % 10000:D4}"; // e.g., Team_1234
//         if (LeaderboardManager.Instance != null)
//         {
//             LeaderboardManager.Instance.SubmitScore(playerName, score, (success) =>
//             {
//                 Debug.Log($"Score submission {(success ? "succeeded" : "failed")}: {playerName}, Score: {score}");
//             });
//         }
//         else
//         {
//             Debug.LogWarning("LeaderboardManager not found, score not submitted");
//         }

//         // Show leaderboard after 3 seconds
//         yield return new WaitForSeconds(3f);
//         ShowLeaderboard();
//         enabled = false;
//     }

//     private void ShowLeaderboard()
//     {
//         resultText.gameObject.SetActive(false); // Hide score text
//         transform.GetChild(1).gameObject.SetActive(false); // Hide background
//         audioPanel.SetActive(false); // Hide audio panel

//         if (LeaderboardManager.Instance == null)
//         {
//             Debug.LogWarning("LeaderboardManager not found, cannot show leaderboard");
//             return;
//         }

//         LeaderboardManager.Instance.GetTopScores((entries) =>
//         {
//             System.Text.StringBuilder leaderboardDisplay = new System.Text.StringBuilder();
//             leaderboardDisplay.AppendLine("<size=36>AR Leaderboard</size>");
//             leaderboardDisplay.AppendLine("<size=24>Rank | Team | Score</size>");
//             leaderboardDisplay.AppendLine("-----------------------");

//             for (int i = 0; i < entries.Count; i++)
//             {
//                 leaderboardDisplay.AppendLine($"{i + 1}. {entries[i].name} | {entries[i].score} points");
//             }

//             leaderboardText.text = leaderboardDisplay.ToString();
//             leaderboardPanel.transform.GetChild(0).gameObject.SetActive(true);
//             resultText.gameObject.SetActive(false); // Hide score text
//             Debug.Log($"Displayed AR leaderboard with {entries.Count} entries");
//         });
//     }
// }

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class QuestionUI : MonoBehaviour
{
    public static QuestionUI Instance { get; private set; }
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI correctText;
    public TextMeshProUGUI incorrectText;
    public RawImage[] paintingImages; // 12 images (1 display + 3 options per phase)
    public GameObject[] paintingPanels; // One per phase (PaintingPanel1, PaintingPanel2, PaintingPanel3)
    public GameObject[] audioPanels; // One per phase (AudioPanel1, AudioPanel2, AudioPanel3)
    public Button[] audioButtons; // 12 buttons (4 per phase)
    public AudioClip[] audioClips; // 4 clips for selection
    public AudioClip correctAudioPhase1; // Correct audio for Phase 1
    public AudioClip correctAudioPhase2; // Correct audio for Phase 2
    public AudioClip correctAudioPhase3; // Correct audio for Phase 3
    private AudioSource audioSource;

    private int currentPhase = 0; // 0, 1, 2 for the three drawing phases
    private int currentPaintingIndex = 0; // 0, 1, 2 for the 3 options within the phase
    private int selectedPaintingIndex = -1; // 0 to 11, adjusted for display
    private int currentAudioIndex = 0;
    private int selectedAudioIndex = -1;
    private bool selectingPainting = true;
    private bool isPaintingCorrect = false;
    private bool isAudioCorrect = false;
    private int[] correctPaintingIndices = new int[3]; // One correct option index per phase

    private InputAction yButtonAction;
    private InputAction xButtonAction;

    void Awake()
    {
        Instance = this;
        yButtonAction = new InputAction("YButton", InputActionType.Button, "<XRController>{LeftHand}/secondaryButton");
        xButtonAction = new InputAction("XButton", InputActionType.Button, "<XRController>{LeftHand}/primaryButton");
        yButtonAction.Enable();
        xButtonAction.Enable();
    }

    void Start()
    {
        gameObject.transform.GetChild(1).gameObject.SetActive(true);
        gameObject.transform.GetChild(0).gameObject.SetActive(true);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.volume = 1.0f;
        audioSource.spatialBlend = 0.0f;
        audioSource.playOnAwake = false;

        if (paintingImages == null || paintingImages.Length != 12)
        {
            Debug.LogError($"paintingImages array is invalid. Length: {(paintingImages == null ? 0 : paintingImages.Length)}, Expected: 12");
            enabled = false;
            return;
        }
        for (int i = 0; i < paintingImages.Length; i++)
        {
            if (paintingImages[i] == null)
            {
                Debug.LogError($"paintingImages[{i}] is null");
                enabled = false;
                return;
            }
        }

        if (paintingPanels == null || paintingPanels.Length != 3)
        {
            Debug.LogError($"paintingPanels array is invalid. Length: {(paintingPanels == null ? 0 : paintingPanels.Length)}, Expected: 3");
            enabled = false;
            return;
        }
        for (int i = 0; i < paintingPanels.Length; i++)
        {
            if (paintingPanels[i] == null)
            {
                Debug.LogError($"paintingPanels[{i}] is null");
                enabled = false;
                return;
            }
        }

        if (audioPanels == null || audioPanels.Length != 3)
        {
            Debug.LogError($"audioPanels array is invalid. Length: {(audioPanels == null ? 0 : audioPanels.Length)}, Expected: 3");
            enabled = false;
            return;
        }
        for (int i = 0; i < audioPanels.Length; i++)
        {
            if (audioPanels[i] == null)
            {
                Debug.LogError($"audioPanels[{i}] is null");
                enabled = false;
                return;
            }
        }

        if (audioButtons == null || audioButtons.Length != 12)
        {
            Debug.LogError($"audioButtons array is invalid. Length: {(audioButtons == null ? 0 : audioButtons.Length)}, Expected: 12");
            enabled = false;
            return;
        }
        for (int i = 0; i < audioButtons.Length; i++)
        {
            if (audioButtons[i] == null)
            {
                Debug.LogError($"audioButtons[{i}] is null");
                enabled = false;
                return;
            }
        }

        if (audioClips == null || audioClips.Length != 4)
        {
            Debug.LogError($"audioClips array is invalid. Length: {(audioClips == null ? 0 : audioClips.Length)}, Expected: 4");
            enabled = false;
            return;
        }
        for (int i = 0; i < audioClips.Length; i++)
        {
            if (audioClips[i] == null)
            {
                Debug.LogWarning($"audioClips[{i}] is null");
            }
        }

        // Validate correct audio clips for each phase
        if (correctAudioPhase1 == null || correctAudioPhase2 == null || correctAudioPhase3 == null)
        {
            Debug.LogError("One or more correct audio clips for phases are not assigned!");
            enabled = false;
            return;
        }

        // Set correct painting indices based on TimeManager.paintingInsp
        if (TimeManager.Instance != null)
        {
            // Assume the correct painting is the first option (index 1, 5, 9) for each phase,
            // corresponding to paintingInsp[0], paintingInsp[1], paintingInsp[2]
            correctPaintingIndices[0] = 1; // First option in Phase 1 (slot 1)
            correctPaintingIndices[1] = 5; // First option in Phase 2 (slot 5)
            correctPaintingIndices[2] = 9; // First option in Phase 3 (slot 9)
            Debug.Log($"QuestionUI: Set correctPaintingIndices: {correctPaintingIndices[0]}, {correctPaintingIndices[1]}, {correctPaintingIndices[2]}");
        }
        else
        {
            Debug.LogError("TimeManager.Instance is null!");
            enabled = false;
            return;
        }

        if (DrawingStorage.Instance != null)
        {
            for (int i = 0; i < 3; i++)
            {
                if (DrawingStorage.Instance.drawings[i] != null)
                {
                    paintingImages[i * 4].texture = DrawingStorage.Instance.drawings[i]; // Load drawing into display
                    paintingImages[i * 4].gameObject.SetActive(false); // Initially inactive
                    Debug.Log($"QuestionUI: Loaded drawing {i + 1} into paintingImages[{i * 4}]");
                }
                for (int j = 1; j < 4; j++) // Load options
                {
                    paintingImages[i * 4 + j].gameObject.SetActive(false); // Initially inactive
                }
            }
        }

        // Assign audio clips to buttons using predefined correct clips
        AudioClip[] correctClips = new AudioClip[] { correctAudioPhase1, correctAudioPhase2, correctAudioPhase3 };
        for (int phase = 0; phase < 3; phase++)
        {
            int correctButtonIndex = Random.Range(0, 4); // Randomly assign the correct audio to one button
            for (int i = 0; i < 4; i++)
            {
                int buttonIndex = phase * 4 + i; // Buttons 0-3 for Phase 0, 4-7 for Phase 1, 8-11 for Phase 2
                if (i == correctButtonIndex)
                {
                    audioButtons[buttonIndex].onClick.AddListener(() => PlayAudio(correctClips[phase]));
                }
                else
                {
                    // Assign a random incorrect audio clip, ensuring no duplicates
                    AudioClip incorrectClip;
                    do
                    {
                        incorrectClip = audioClips[Random.Range(0, audioClips.Length)];
                    } while (incorrectClip == correctClips[phase]);
                    audioButtons[buttonIndex].onClick.AddListener(() => PlayAudio(incorrectClip));
                }
            }
        }

        UpdatePaintingDisplay();
        resultText.text = "Select the painting for Phase 1. Use button Y to cycle and button X to select.";
    }

    void Update()
    {
        if (!gameObject.activeSelf || !TimeManager.drawingCompleted)
        {
            return;
        }

        if (correctText.gameObject.activeSelf || incorrectText.gameObject.activeSelf)
        {
            return;
        }

        if (selectingPainting)
        {
            bool yPressed = yButtonAction != null && yButtonAction.WasPressedThisFrame() || Input.GetKeyDown(KeyCode.Y);
            if (yPressed)
            {
                currentPaintingIndex = (currentPaintingIndex + 1) % 3; // Cycle 0, 1, 2 within phase
                UpdatePaintingDisplay();
                resultText.text = $"Painting for Phase {currentPhase + 1}. Confirm with X.";
            }

            bool xPressed = xButtonAction != null && xButtonAction.WasPressedThisFrame() || Input.GetKeyDown(KeyCode.X);
            if (xPressed)
            {
                selectedPaintingIndex = currentPhase * 4 + currentPaintingIndex + 1; // 1-3, 5-7, 9-11
                isPaintingCorrect = selectedPaintingIndex == correctPaintingIndices[currentPhase];
                StartCoroutine(ShowPaintingFeedback());
            }
        }
        else
        {
            bool yPressed = yButtonAction != null && yButtonAction.WasPressedThisFrame() || Input.GetKeyDown(KeyCode.Y);
            if (yPressed)
            {
                currentAudioIndex = (currentAudioIndex + 1) % 4;
                UpdateAudioDisplay();
                if (audioClips[currentAudioIndex] != null)
                {
                    audioSource.Stop();
                    audioSource.PlayOneShot(audioClips[currentAudioIndex]);
                }
                resultText.text = $"Audio for Phase {currentPhase + 1}. Confirm with X.";
            }

            bool xPressed = xButtonAction != null && xButtonAction.WasPressedThisFrame() || Input.GetKeyDown(KeyCode.X);
            if (xPressed)
            {
                selectedAudioIndex = currentAudioIndex;
                AudioClip[] correctClips = new AudioClip[] { correctAudioPhase1, correctAudioPhase2, correctAudioPhase3 };
                isAudioCorrect = audioClips[selectedAudioIndex] == correctClips[currentPhase]; // Correct if selected audio matches phase audio
                StartCoroutine(ShowAudioFeedbackAndScore());
                audioSource.Stop();
            }
        }
    }

    public void UpdatePaintingDisplay()
    {
        for (int i = 0; i < paintingPanels.Length; i++)
        {
            bool isActive = i == currentPhase;
            paintingPanels[i].SetActive(isActive);
            if (isActive)
            {
                for (int j = 0; j < 4; j++) // 4 images: 1 display + 3 options
                {
                    if (j == 0)
                    {
                        paintingImages[i * 4 + j].gameObject.SetActive(false);
                    }
                    else
                    {
                        paintingImages[i * 4 + j].gameObject.SetActive(j == 0 || j - 1 == currentPaintingIndex);
                    }
                }
            }
        }
    }

    void UpdateAudioDisplay()
    {
        for (int i = 0; i < audioPanels.Length; i++)
        {
            audioPanels[i].SetActive(i == currentPhase);
            if (i == currentPhase)
            {
                int startIndex = i * 4;
                for (int j = 0; j < 4; j++)
                {
                    audioButtons[startIndex + j].image.color = j == currentAudioIndex ? Color.yellow : Color.white;
                }
            }
        }
    }

    void PlayAudio(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(clip);
        }
    }

    IEnumerator ShowPaintingFeedback()
    {
        correctText.gameObject.SetActive(isPaintingCorrect);
        incorrectText.gameObject.SetActive(!isPaintingCorrect);
        yield return new WaitForSeconds(3f);
        correctText.gameObject.SetActive(false);
        incorrectText.gameObject.SetActive(false);

        selectingPainting = false;
        paintingPanels[currentPhase].SetActive(false);
        audioPanels[currentPhase].SetActive(true);
        UpdateAudioDisplay();
        if (audioClips[currentAudioIndex] != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(audioClips[currentAudioIndex]);
        }
        resultText.text = $"Select the audio for Phase {currentPhase + 1}. Use button Y to cycle and button X to select.";
    }

    IEnumerator ShowAudioFeedbackAndScore()
    {
        correctText.gameObject.SetActive(isAudioCorrect);
        incorrectText.gameObject.SetActive(!isAudioCorrect);
        yield return new WaitForSeconds(3f);
        correctText.gameObject.SetActive(false);
        incorrectText.gameObject.SetActive(false);

        int score = (isPaintingCorrect ? 1 : 0) + (isAudioCorrect ? 1 : 0);
        resultText.text = $"Score for Phase {currentPhase + 1}: {score}/2";

        yield return new WaitForSeconds(3f);
        NextPhase();
    }

    void NextPhase()
    {
        for (int i = currentPhase * 4; i < (currentPhase + 1) * 4; i++)
        {
            paintingImages[i].gameObject.SetActive(false);
        }
        audioPanels[currentPhase].SetActive(false);
        currentPhase++;
        if (currentPhase < 3)
        {
            selectingPainting = true;
            currentPaintingIndex = 0; // Reset to first option
            UpdatePaintingDisplay();
            resultText.text = $"Select the painting for Phase {currentPhase + 1}. Use button Y to cycle and button X to select.";
        }
        else
        {
            SceneController.Instance.EndGame();
        }
    }
}