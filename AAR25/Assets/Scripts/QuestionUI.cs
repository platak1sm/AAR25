// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using System.Collections;
// using UnityEngine.InputSystem;
// using UnityEngine.SceneManagement;

// public class QuestionUI : MonoBehaviour
// {
//     public static QuestionUI Instance { get; private set; }
//     public TextMeshProUGUI resultText;
//     public TextMeshProUGUI correctText;
//     public TextMeshProUGUI incorrectText;
//     public RawImage[] paintingImages; // 12 images (1 display + 3 options per phase)
//     public GameObject[] paintingPanels; // One per phase (PaintingPanel1, PaintingPanel2, PaintingPanel3)
//     public GameObject[] audioPanels; // One per phase (AudioPanel1, AudioPanel2, AudioPanel3)
//     public Button[] audioButtons; // 12 buttons (4 per phase)
//     public AudioClip[] audioClips; // 4 clips for selection
//     public AudioClip correctAudioPhase1; 
//     public AudioClip correctAudioPhase2; 
//     public AudioClip correctAudioPhase3; 
//     public GameObject leaderboardPanel; 
//     public TextMeshProUGUI leaderboardText; 

//     private AudioSource audioSource;
//     private int currentPhase = 0; 
//     private int currentPaintingIndex = 0; 
//     private int selectedPaintingIndex = -1; 
//     private int currentAudioIndex = 0;
//     private int selectedAudioIndex = -1;
//     private bool selectingPainting = true;
//     private bool[] isPaintingCorrect = new bool[3]; 
//     private bool[] isAudioCorrect = new bool[3]; 
//     private int[] correctPaintingIndices = new int[3]; 
//     private int[] correctButtonIndices = new int[3]; 
//     private bool canInput = true; 

//     private InputAction yButtonAction;
//     private InputAction xButtonAction;

//     void Awake()
//     {
//         Instance = this;
//         yButtonAction = new InputAction("YButton", InputActionType.Button, "<XRController>{LeftHand}/secondaryButton");
//         xButtonAction = new InputAction("XButton", InputActionType.Button, "<XRController>{LeftHand}/primaryButton");
//         yButtonAction.Enable();
//         xButtonAction.Enable();
//     }

//     void Start()
//     {
//         gameObject.transform.GetChild(1).gameObject.SetActive(true);
//         gameObject.transform.GetChild(0).gameObject.SetActive(true);
//         audioSource = GetComponent<AudioSource>();
//         if (audioSource == null)
//         {
//             audioSource = gameObject.AddComponent<AudioSource>();
//         }
//         audioSource.volume = 1.0f;
//         audioSource.spatialBlend = 0.0f;
//         audioSource.playOnAwake = false;

//         if (paintingImages == null || paintingImages.Length != 12)
//         {
//             Debug.LogError($"paintingImages array is invalid. Length: {(paintingImages == null ? 0 : paintingImages.Length)}, Expected: 12");
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
//         }

//         if (paintingPanels == null || paintingPanels.Length != 3)
//         {
//             Debug.LogError($"paintingPanels array is invalid. Length: {(paintingPanels == null ? 0 : paintingPanels.Length)}, Expected: 3");
//             enabled = false;
//             return;
//         }
//         for (int i = 0; i < paintingPanels.Length; i++)
//         {
//             if (paintingPanels[i] == null)
//             {
//                 Debug.LogError($"paintingPanels[{i}] is null");
//                 enabled = false;
//                 return;
//             }
//         }

//         if (audioPanels == null || audioPanels.Length != 3)
//         {
//             Debug.LogError($"audioPanels array is invalid. Length: {(audioPanels == null ? 0 : audioPanels.Length)}, Expected: 3");
//             enabled = false;
//             return;
//         }
//         for (int i = 0; i < audioPanels.Length; i++)
//         {
//             if (audioPanels[i] == null)
//             {
//                 Debug.LogError($"audioPanels[{i}] is null");
//                 enabled = false;
//                 return;
//             }
//         }

//         if (audioButtons == null || audioButtons.Length != 12)
//         {
//             Debug.LogError($"audioButtons array is invalid. Length: {(audioButtons == null ? 0 : audioButtons.Length)}, Expected: 12");
//             enabled = false;
//             return;
//         }
//         for (int i = 0; i < audioButtons.Length; i++)
//         {
//             if (audioButtons[i] == null)
//             {
//                 Debug.LogError($"audioButtons[{i}] is null");
//                 enabled = false;
//                 return;
//             }
//         }

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

//         if (correctAudioPhase1 == null || correctAudioPhase2 == null || correctAudioPhase3 == null)
//         {
//             Debug.LogError("One or more correct audio clips for phases are not assigned!");
//             enabled = false;
//             return;
//         }

//         if (TimeManager.Instance != null)
//         {
//             correctPaintingIndices[0] = 2; 
//             correctPaintingIndices[1] = 5; 
//             correctPaintingIndices[2] = 10; 
//             Debug.Log($"QuestionUI: Set correctPaintingIndices: {correctPaintingIndices[0]}, {correctPaintingIndices[1]}, {correctPaintingIndices[2]}");
//         }
//         else
//         {
//             Debug.LogError("TimeManager.Instance is null!");
//             enabled = false;
//             return;
//         }

//         AudioClip[] correctClips = new AudioClip[] { correctAudioPhase1, correctAudioPhase2, correctAudioPhase3 };
//         for (int phase = 0; phase < 3; phase++)
//         {
//             correctButtonIndices[phase] = Random.Range(0, 4); // Store the correct button index
//             for (int i = 0; i < 4; i++)
//             {
//                 int buttonIndex = phase * 4 + i;
//                 if (i == correctButtonIndices[phase])
//                 {
//                     int capturedIndex = i;
//                     audioButtons[buttonIndex].onClick.AddListener(() => { PlayAudio(correctClips[phase]); currentAudioIndex = capturedIndex; UpdateAudioDisplay(); });
//                 }
//                 else
//                 {
//                     AudioClip incorrectClip;
//                     do
//                     {
//                         incorrectClip = audioClips[Random.Range(0, audioClips.Length)];
//                     } while (incorrectClip == correctClips[phase]);
//                     int capturedIndex = i;
//                     audioButtons[buttonIndex].onClick.AddListener(() => { PlayAudio(incorrectClip); currentAudioIndex = capturedIndex; UpdateAudioDisplay(); });
//                 }
//             }
//         }

//         UpdatePaintingDisplay();
//         resultText.text = "Select the painting for Phase 1. Use button Y to cycle and button X to select.";
//     }

//     void Update()
//     {
//         if (!gameObject.activeSelf || !TimeManager.drawingCompleted || !canInput)
//             return;

//         if (selectingPainting)
//         {
//             bool yPressed = yButtonAction.WasPressedThisFrame() || Input.GetKeyDown(KeyCode.Y);
//             if (yPressed)
//             {
//                 currentPaintingIndex = (currentPaintingIndex + 1) % 3;
//                 UpdatePaintingDisplay();
//                 resultText.text = $"Painting for Phase {currentPhase + 1}. Confirm with X.";
//             }

//             bool xPressed = xButtonAction.WasPressedThisFrame() || Input.GetKeyDown(KeyCode.X);
//             if (xPressed)
//             {
//                 selectedPaintingIndex = currentPhase * 4 + currentPaintingIndex + 1;
//                 isPaintingCorrect[currentPhase] = selectedPaintingIndex == correctPaintingIndices[currentPhase];
//                 StartCoroutine(ShowPaintingFeedback());
//                 canInput = false;
//             }
//         }
//         else
//         {
//             bool yPressed = yButtonAction.WasPressedThisFrame() || Input.GetKeyDown(KeyCode.Y);
//             if (yPressed)
//             {
//                 currentAudioIndex = (currentAudioIndex + 1) % 4;
//                 UpdateAudioDisplay();
//                 if (audioClips[currentAudioIndex] != null)
//                 {
//                     audioSource.Stop();
//                     audioSource.PlayOneShot(audioClips[currentAudioIndex]);
//                 }
//                 resultText.text = $"Audio for Phase {currentPhase + 1}. Confirm with X.";
//             }

//             bool xPressed = xButtonAction.WasPressedThisFrame() || Input.GetKeyDown(KeyCode.X);
//             if (xPressed)
//             {
//                 audioSource.Stop();
//                 selectedAudioIndex = currentAudioIndex;
//                 AudioClip[] correctClips = new AudioClip[] { correctAudioPhase1, correctAudioPhase2, correctAudioPhase3 };
//                 isAudioCorrect[currentPhase] = audioClips[selectedAudioIndex] == correctClips[currentPhase];
//                 StartCoroutine(ShowAudioFeedbackAndScore());
//                 canInput = false;
//             }
//         }
//     }

//     public void UpdatePaintingDisplay()
//     {
//         for (int i = 0; i < paintingPanels.Length; i++)
//         {
//             bool isActive = i == currentPhase;
//             paintingPanels[i].SetActive(isActive);
//             if (isActive)
//             {
//                 for (int j = 0; j < 4; j++)
//                 {
//                     if (j == 0)
//                     {
//                         paintingImages[i * 4 + j].gameObject.SetActive(false);
//                     }
//                     else
//                     {
//                     paintingImages[i * 4 + j].gameObject.SetActive(j == 0 || j - 1 == currentPaintingIndex);
//                     }
//                 }
//             }
//         }
//     }

//     void UpdateAudioDisplay()
//     {
//         for (int i = 0; i < audioPanels.Length; i++)
//         {
//             audioPanels[i].SetActive(i == currentPhase);
//             if (i == currentPhase)
//             {
//                 int startIndex = i * 4;
//                 for (int j = 0; j < 4; j++)
//                 {
//                     audioButtons[startIndex + j].image.color = j == currentAudioIndex ? Color.yellow : Color.white;
//                 }
//             }
//         }
//     }
//     void PlayAudio(AudioClip clip)
//     {
//         if (clip != null)
//         {
//             audioSource.Stop();
//             audioSource.PlayOneShot(clip);
//         }
//     }

//     IEnumerator ShowPaintingFeedback()
//     {
//         correctText.gameObject.SetActive(isPaintingCorrect[currentPhase]);
//         incorrectText.gameObject.SetActive(!isPaintingCorrect[currentPhase]);
//         yield return new WaitForSeconds(3f); // 3-second feedback display
//         correctText.gameObject.SetActive(false);
//         incorrectText.gameObject.SetActive(false);

//         selectingPainting = false;
//         paintingPanels[currentPhase].SetActive(false);
//         audioPanels[currentPhase].SetActive(true);
//         currentAudioIndex = correctButtonIndices[currentPhase];
//         UpdateAudioDisplay();
//         if (audioClips[currentAudioIndex] != null)
//         {
//             audioSource.Stop();
//             audioSource.PlayOneShot(audioClips[currentAudioIndex]);
//         }
//         resultText.text = $"Select the audio for Phase {currentPhase + 1}. Use button Y to cycle and button X to select.";
//         yield return new WaitForSeconds(3f); // 3-second no-input period
//         canInput = true;
//     }

//     IEnumerator ShowAudioFeedbackAndScore()
//     {
//         correctText.gameObject.SetActive(isAudioCorrect[currentPhase]);
//         incorrectText.gameObject.SetActive(!isAudioCorrect[currentPhase]);
//         yield return new WaitForSeconds(3f); // 3-second feedback display
//         correctText.gameObject.SetActive(false);
//         incorrectText.gameObject.SetActive(false);

//         int phaseScore = (isPaintingCorrect[currentPhase] ? 1 : 0) + (isAudioCorrect[currentPhase] ? 1 : 0);
//         resultText.text = $"Score for Phase {currentPhase + 1}: {phaseScore}/2";

//         yield return new WaitForSeconds(3f); // 3-second no-input period
//         canInput = true;

//         if (currentPhase == 2) // Last phase
//         {
//             yield return new WaitForSeconds(5f); // 5-second delay before score scene
//             int totalCorrect = (isPaintingCorrect[0] ? 1 : 0) + (isAudioCorrect[0] ? 1 : 0) +
//                              (isPaintingCorrect[1] ? 1 : 0) + (isAudioCorrect[1] ? 1 : 0) +
//                              (isPaintingCorrect[2] ? 1 : 0) + (isAudioCorrect[2] ? 1 : 0);
//             float levelMultiplier = TimeManager.difficultyPoints; // Use difficultyPoints from TimeManager
//             float totalScore = totalCorrect * levelMultiplier;
//             int maxScore = 6 * (int)levelMultiplier; // 6 possible correct answers (2 per phase)
//             Debug.Log($"Transitioning to ScoreScene with totalScore: {totalScore}, maxScore: {maxScore}");
//             SceneManager.LoadScene("ScoreScene"); // Transition to score scene
//             PlayerPrefs.SetFloat("TotalScore", totalScore);
//             PlayerPrefs.SetInt("MaxScore", maxScore);
//         }
//         else
//         {
//             NextPhase();
//         }
//     }

//     void NextPhase()
//     {
//         for (int i = currentPhase * 4; i < (currentPhase + 1) * 4; i++)
//         {
//             paintingImages[i].gameObject.SetActive(false);
//         }
//         audioPanels[currentPhase].SetActive(false);
//         currentPhase++;
//         if (currentPhase < 3)
//         {
//             selectingPainting = true;
//             currentPaintingIndex = 0;
//             UpdatePaintingDisplay();
//             resultText.text = $"Select the painting for Phase {currentPhase + 1}. Use button Y to cycle and button X to select.";
//         }
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
    public AudioClip[] audioClips; // 4 audio clips
    public GameObject lastAudioPanel;
    public GameObject leaderboardPanel; 
    public TextMeshProUGUI leaderboardText; 
    public AudioClip correctAudioPhase1; 
    public AudioClip correctAudioPhase2; 
    public AudioClip correctAudioPhase3; 
    private AudioSource audioSource;

    private int currentPhase = 0; // 0, 1, 2 for the three drawing phases
    private int currentPaintingIndex = 0; 
    private int selectedPaintingIndex = -1; 
    private int currentAudioIndex = 0;
    private int selectedAudioIndex = -1;
    private bool selectingPainting = true;
    private bool[] isPaintingCorrect = new bool[3]; 
    private bool[] isAudioCorrect = new bool[3];
    private int[] correctPaintingIndices = new int[3]; 
    private int[] correctButtonIndices = new int[3]; 
    private bool canInput = true; 
    private int totalScore = 0; 

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

        if (correctAudioPhase1 == null || correctAudioPhase2 == null || correctAudioPhase3 == null)
        {
            Debug.LogError("One or more correct audio clips for phases are not assigned!");
            enabled = false;
            return;
        }

        if (TimeManager.Instance != null)
        {
            correctPaintingIndices[0] = 2; 
            correctPaintingIndices[1] = 5; 
            correctPaintingIndices[2] = 10; 
            Debug.Log($"QuestionUI: Set correctPaintingIndices: {correctPaintingIndices[0]}, {correctPaintingIndices[1]}, {correctPaintingIndices[2]}");
        }
        else
        {
            Debug.LogError("TimeManager.Instance is null!");
            enabled = false;
            return;
        }

        AudioClip[] correctClips = new AudioClip[] { correctAudioPhase1, correctAudioPhase2, correctAudioPhase3 };
        for (int phase = 0; phase < 3; phase++)
        {
            correctButtonIndices[phase] = Random.Range(0, 4); 
            for (int i = 0; i < 4; i++)
            {
                int buttonIndex = phase * 4 + i;
                if (i == correctButtonIndices[phase])
                {
                    int capturedIndex = i;
                    audioButtons[buttonIndex].onClick.AddListener(() => { PlayAudio(correctClips[phase]); currentAudioIndex = capturedIndex; UpdateAudioDisplay(); });
                }
                else
                {
                    AudioClip incorrectClip;
                    do
                    {
                        incorrectClip = audioClips[Random.Range(0, audioClips.Length)];
                    } while (incorrectClip == correctClips[phase]);
                    int capturedIndex = i;
                    audioButtons[buttonIndex].onClick.AddListener(() => { PlayAudio(incorrectClip); currentAudioIndex = capturedIndex; UpdateAudioDisplay(); });
                }
            }
        }

        UpdatePaintingDisplay();
        resultText.text = "Select the painting for Phase 1. Use button Y to cycle and button X to select.";
    }

    void Update()
    {
        if (!gameObject.activeSelf || !TimeManager.drawingCompleted || !canInput)
            return;

        if (selectingPainting)
        {
            bool yPressed = yButtonAction.WasPressedThisFrame() || Input.GetKeyDown(KeyCode.Y);
            if (yPressed)
            {
                currentPaintingIndex = (currentPaintingIndex + 1) % 3;
                UpdatePaintingDisplay();
                resultText.text = $"Painting for Phase {currentPhase + 1}. Confirm with X.";
            }

            bool xPressed = xButtonAction.WasPressedThisFrame() || Input.GetKeyDown(KeyCode.X);
            if (xPressed)
            {
                selectedPaintingIndex = currentPhase * 4 + currentPaintingIndex + 1;
                isPaintingCorrect[currentPhase] = selectedPaintingIndex == correctPaintingIndices[currentPhase];
                StartCoroutine(ShowPaintingFeedback());
                canInput = false;
            }
        }
        else
        {
            bool yPressed = yButtonAction.WasPressedThisFrame() || Input.GetKeyDown(KeyCode.Y);
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

            bool xPressed = xButtonAction.WasPressedThisFrame() || Input.GetKeyDown(KeyCode.X);
            if (xPressed)
            {
                audioSource.Stop();
                selectedAudioIndex = currentAudioIndex;
                AudioClip[] correctClips = new AudioClip[] { correctAudioPhase1, correctAudioPhase2, correctAudioPhase3 };
                isAudioCorrect[currentPhase] = audioClips[selectedAudioIndex] == correctClips[currentPhase];
                StartCoroutine(ShowAudioFeedbackAndScore());
                canInput = false;
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
                for (int j = 0; j < 4; j++)
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
        correctText.gameObject.SetActive(isPaintingCorrect[currentPhase]);
        incorrectText.gameObject.SetActive(!isPaintingCorrect[currentPhase]);
        yield return new WaitForSeconds(3f); 
        correctText.gameObject.SetActive(false);
        incorrectText.gameObject.SetActive(false);

        selectingPainting = false;
        paintingPanels[currentPhase].SetActive(false);
        audioPanels[currentPhase].SetActive(true);
        currentAudioIndex = correctButtonIndices[currentPhase];
        UpdateAudioDisplay();
        if (audioClips[currentAudioIndex] != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(audioClips[currentAudioIndex]);
        }
        resultText.text = $"Select the audio for Phase {currentPhase + 1}. Use button Y to cycle and button X to select.";
        yield return new WaitForSeconds(3f); 
        canInput = true;
    }

    IEnumerator ShowAudioFeedbackAndScore()
    {
        correctText.gameObject.SetActive(isAudioCorrect[currentPhase]);
        incorrectText.gameObject.SetActive(!isAudioCorrect[currentPhase]);
        yield return new WaitForSeconds(3f); 
        correctText.gameObject.SetActive(false);
        incorrectText.gameObject.SetActive(false);

        int phaseScore = (isPaintingCorrect[currentPhase] ? 1 : 0) + (isAudioCorrect[currentPhase] ? 1 : 0);
        totalScore += phaseScore; 
        resultText.text = $"Score for Phase {currentPhase + 1}: {phaseScore}/2";

        yield return new WaitForSeconds(3f); 
        canInput = true;

        if (currentPhase == 2) // Last phase
        {
            resultText.text = $"Total Score: {totalScore}/6";
            yield return new WaitForSeconds(3f);

            // Submit total score to leaderboard
            if (LeaderboardManager.Instance != null)
            {
                LeaderboardManager.Instance.GetTopScores((entries) =>
                {
                    int entryCount = entries.Count;
                    string playerName = $"Team{entryCount + 1}"; // e.g., Team2 if 1 entry exists
                    int finalScore = (int)(totalScore * TimeManager.difficultyPoints); 
                    LeaderboardManager.Instance.SubmitScore(playerName, finalScore, (success) =>
                    {
                        Debug.Log($"Score submission {(success ? "succeeded" : "failed")}: {playerName}, Score: {finalScore}");
                    });
                });
            }
            else
            {
                Debug.LogWarning("LeaderboardManager not found, score not submitted");
                string playerName = $"Team1"; 

                int finalScore = (int)(totalScore * TimeManager.difficultyPoints);
                LeaderboardManager.Instance.SubmitScore(playerName, finalScore, (success) =>
                {
                    Debug.Log($"Score submission {(success ? "succeeded" : "failed")}: {playerName}, Score: {finalScore}");
                });
            }

        
            yield return new WaitForSeconds(3f);
            ShowLeaderboard();
            enabled = false;
        }
        else
        {
            NextPhase();
        }
    }

    private void ShowLeaderboard()
    {
        resultText.gameObject.SetActive(false); 
        transform.GetChild(1).gameObject.SetActive(false); 
        lastAudioPanel.SetActive(false); 

        if (LeaderboardManager.Instance == null)
        {
            Debug.LogWarning("LeaderboardManager not found, cannot show leaderboard");
            return;
        }

        LeaderboardManager.Instance.GetTopScores((entries) =>
        {
            System.Text.StringBuilder leaderboardDisplay = new System.Text.StringBuilder();
            leaderboardDisplay.AppendLine("<size=36>AR Leaderboard</size>");
            leaderboardDisplay.AppendLine("<size=24>Rank | Team | Score</size>");
            leaderboardDisplay.AppendLine("-----------------------");

            for (int i = 0; i < entries.Count; i++)
            {
                leaderboardDisplay.AppendLine($"{i + 1}. {entries[i].name} | {entries[i].score} points");
            }

            leaderboardText.text = leaderboardDisplay.ToString();
            leaderboardPanel.transform.GetChild(0).gameObject.SetActive(true);
            resultText.gameObject.SetActive(false); // Hide score text
            Debug.Log($"Displayed AR leaderboard with {entries.Count} entries");
        });
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
            currentPaintingIndex = 0;
            UpdatePaintingDisplay();
            resultText.text = $"Select the painting for Phase {currentPhase + 1}. Use button Y to cycle and button X to select.";
        }
    }
}