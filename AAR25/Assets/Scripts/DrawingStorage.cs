using UnityEngine;

public class DrawingStorage : MonoBehaviour
{
    public static DrawingStorage Instance { get; private set; }
    public Texture2D[] drawings = new Texture2D[3]; // Store drawings for Phase 1, Phase 2, Phase 3

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
}