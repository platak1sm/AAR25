using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }

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
        SceneManager.LoadScene("StartScene");
    }

    public void StartMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void EndGame()
    {
        Debug.Log("SceneController: Transitioning to EndScene");
        //SceneManager.LoadScene("EndScene");
    }
}