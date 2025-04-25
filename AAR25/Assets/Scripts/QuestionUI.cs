using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.XR;


public class QuestionUI : MonoBehaviour
{
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI correctText;
    public TextMeshProUGUI incorrectText;
    public RawImage[] paintingImages; // 3 RawImage components for paintings
    public Button[] audioButtons; // 4 audio buttons
    public AudioClip[] audioClips; // 4 audio clips
    public GameObject paintingPanel; // PaintingPanel GameObject
    public GameObject audioPanel; // AudioPanel GameObject
    public GameObject leaderboardPanel; // LeaderboardPanel GameObject
    public TextMeshProUGUI leaderboardText; // Text to display leaderboard
    private AudioSource audioSource;

    private int currentPaintingIndex = 0;
    private int selectedPaintingIndex = -1;
    private int currentAudioIndex = 0;
    private int selectedAudioIndex = -1;
    private bool selectingPainting = true; // True: selecting painting, False: selecting audio
    private bool isPaintingCorrect = false;
    private bool isAudioCorrect = false;
    private int correctPaintingIndex = 0;
    private int correctAudioIndex = 0;
    
    
    private InputAction yButtonAction;
    private InputAction xButtonAction;

    void Awake()
    {
        // Setup Input Actions for OpenXR
        yButtonAction = new InputAction("YButton", InputActionType.Button, "<XRController>{LeftHand}/secondaryButton");
        xButtonAction = new InputAction("XButton", InputActionType.Button, "<XRController>{LeftHand}/primaryButton");
        yButtonAction.Enable();
        xButtonAction.Enable();
    }

    void Start()
    {
        // Initialize AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.volume = 1.0f;
        audioSource.spatialBlend = 0.0f; // 2D audio
        audioSource.playOnAwake = false;
        // DrawingPersistence.LoadDrawing(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, this); // Commented out: disable drawing loading

        // Validate paintingImages
        if (paintingImages == null || paintingImages.Length != 3)
        {
            Debug.LogError($"paintingImages array is invalid. Length: {(paintingImages == null ? 0 : paintingImages.Length)}, Expected: 3");
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
            if (paintingImages[i].texture == null)
            {
                Debug.LogWarning($"paintingImages[{i}] has no texture assigned");
            }
        }

        // Validate paintingPanel
        if (paintingPanel == null)
        {
            Debug.LogError("paintingPanel is not assigned");
            enabled = false;
            return;
        }

        // Validate audioClips
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

        // Get correct painting index
        if (PaintingSelector.Instance != null)
        {
            correctPaintingIndex = PaintingSelector.Instance.CurrentPaintingIndex;
            Debug.Log($"Correct painting index from PaintingSelector: {correctPaintingIndex}");
        }
        else
        {
            Debug.LogWarning("PaintingSelector not found. Using default correctPaintingIndex: 0");
        }

        // Get correct audio index
        if (AudioManager.Instance != null && AudioManager.Instance.sceneAudio != null)
        {
            correctAudioIndex = System.Array.IndexOf(audioClips, AudioManager.Instance.sceneAudio);
            if (correctAudioIndex == -1)
            {
                Debug.LogWarning("AudioManager.sceneAudio not found in audioClips. Using default correctAudioIndex: 0");
                correctAudioIndex = 0;
            }
            else
            {
                Debug.Log($"Correct audio index from AudioManager: {correctAudioIndex}");
            }
        }
        else
        {
            Debug.LogWarning("AudioManager or sceneAudio not found. Using default correctAudioIndex: 0");
        }
        // Initialize UI
        UpdatePaintingDisplay();
        resultText.text = "Select the painting that inspired the first user. Go through each painting using button Y and press button X when you want to select it.";
        Debug.Log($"Initialized QuestionUI. paintingImages.Length: {paintingImages.Length}, currentPaintingIndex: {currentPaintingIndex}, Left Controller Connected: false");
    }

    void Update()
    {

        // Skip input if coroutine is running (during feedback delay)
        if (correctText.gameObject.activeSelf || incorrectText.gameObject.activeSelf)
        {
            Debug.Log("Input skipped: Feedback text active");
            return;
        }

        // Y button (left controller) or debug key: Cycle options
        bool yPressed = yButtonAction.WasPressedThisFrame() || Input.GetKeyDown(KeyCode.Y);
        if (yPressed)
        {
            Debug.Log($"Y button pressed on right controller. selectingPainting: {selectingPainting}");
            if (selectingPainting)
            {
                currentPaintingIndex = (currentPaintingIndex + 1) % paintingImages.Length;
                UpdatePaintingDisplay();
                resultText.text = $"Painting {currentPaintingIndex + 1}. Confirm with X.";
                Debug.Log($"Cycled painting. currentPaintingIndex: {currentPaintingIndex}");
            }
            else
            {
                currentAudioIndex = (currentAudioIndex + 1) % audioButtons.Length;
                UpdateAudioDisplay();
                if (audioClips[currentAudioIndex] != null)
                {
                    audioSource.Stop();
                    audioSource.PlayOneShot(audioClips[currentAudioIndex]);
                    Debug.Log($"Playing audio: {audioClips[currentAudioIndex].name}");
                }
                else
                {
                    Debug.LogWarning($"audioClips[{currentAudioIndex}] is null, cannot play");
                }
                resultText.text = $"Audio {currentAudioIndex + 1}. Confirm with X.";
                Debug.Log($"Cycled audio. currentAudioIndex: {currentAudioIndex}");
            }
        }else if (yButtonAction.IsPressed())
        {
            Debug.Log("Y button held but not triggered (WasPressedThisFrame not satisfied)");
        }


        // X button (left controller) or debug key: Select option
        bool xPressed = xButtonAction.WasPressedThisFrame() || Input.GetKeyDown(KeyCode.X);
        if (xPressed)
        {
            Debug.Log("X button pressed on left controller");
            if (selectingPainting)
            {
                selectedPaintingIndex = currentPaintingIndex;
                isPaintingCorrect = selectedPaintingIndex == correctPaintingIndex;
                StartCoroutine(ShowPaintingFeedback());
                Debug.Log($"Selected painting {selectedPaintingIndex}. Correct: {isPaintingCorrect}");
            }
            else
            {
                selectedAudioIndex = currentAudioIndex;
                isAudioCorrect = selectedAudioIndex == correctAudioIndex;
                StartCoroutine(ShowAudioFeedbackAndScore());
                Debug.Log($"Selected audio {selectedAudioIndex}. Correct: {isAudioCorrect}");
            }
        }
    }



    void UpdatePaintingDisplay()
    {
        for (int i = 0; i < paintingImages.Length; i++)
        {
            bool isActive = i == currentPaintingIndex;
            //paintingImages[i].gameObject.SetActive(isActive);
            paintingPanel.transform.GetChild(i).gameObject.SetActive(isActive);
            Debug.Log($"PaintingImage{i + 1} active: {isActive}, Texture: {(paintingImages[i].texture != null ? paintingImages[i].texture.name : "None")}");
        }
    }

    void UpdateAudioDisplay()
    {
        for (int i = 0; i < audioButtons.Length; i++)
        {
            audioButtons[i].image.color = i == currentAudioIndex ? Color.yellow : Color.white;
        }
    }

    IEnumerator ShowPaintingFeedback()
    {
        // Show feedback for 3 seconds
        correctText.gameObject.SetActive(isPaintingCorrect);
        incorrectText.gameObject.SetActive(!isPaintingCorrect);
        yield return new WaitForSeconds(3f);
        correctText.gameObject.SetActive(false);
        incorrectText.gameObject.SetActive(false);

        // Transition to audio selection
        selectingPainting = false;
        paintingPanel.SetActive(false);
        audioPanel.SetActive(true);
        UpdateAudioDisplay();
        // Stop AudioManager audio to avoid conflicts
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopAudio();
        }
        if (audioClips[currentAudioIndex] != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(audioClips[currentAudioIndex]);
            Debug.Log($"Playing initial audio: {audioClips[currentAudioIndex].name}");
        }
        else
        {
            Debug.LogWarning($"audioClips[{currentAudioIndex}] is null, cannot play initial audio");
        }
        resultText.text = "Select the audio that inspired the first user. Go through each audio using button Y and press button X when you want to select it.";
    }

    IEnumerator ShowAudioFeedbackAndScore()
    {
        // Show feedback for 3 seconds
        correctText.gameObject.SetActive(isAudioCorrect);
        incorrectText.gameObject.SetActive(!isAudioCorrect);
        yield return new WaitForSeconds(3f);
        correctText.gameObject.SetActive(false);
        incorrectText.gameObject.SetActive(false);

        // Show score
        int score = (isPaintingCorrect ? 1 : 0) + (isAudioCorrect ? 1 : 0);
        resultText.text = $"Score: {score}/2";

        // Submit score to leaderboard
        string playerName = $"Team_{System.DateTime.Now.Ticks % 10000:D4}"; // e.g., Player_1234
        if (LeaderboardManager.Instance != null)
        {
            LeaderboardManager.Instance.SubmitScore(playerName, score, (success) =>
            {
                Debug.Log($"Score submission {(success ? "succeeded" : "failed")}: {playerName}, Score: {score}");
            });
        }
        else
        {
            Debug.LogWarning("LeaderboardManager not found, score not submitted");
        }

        // Show leaderboard after 3 seconds
        yield return new WaitForSeconds(3f);
        ShowLeaderboard();
        enabled = false;
    }

    private void ShowLeaderboard()
    {
        resultText.gameObject.SetActive(false); // Hide score text
        transform.GetChild(1).gameObject.SetActive(false); // Hide background
        audioPanel.SetActive(false); // Hide audio panel

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
}