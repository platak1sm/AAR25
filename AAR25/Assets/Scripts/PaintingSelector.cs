using UnityEngine;
using UnityEngine.UI;

public class PaintingSelector : MonoBehaviour
{
    public static PaintingSelector Instance { get; private set; }
    public int CurrentPaintingIndex { get; private set; }
    public RawImage paintingImage; // RawImage in PaintingUI
    public Texture[] paintingTextures; // Same textures as QuestionUI.paintingImages

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Validate
        if (paintingImage == null)
        {
            Debug.LogError("PaintingSelector: paintingImage not assigned");
            return;
        }
        if (paintingTextures == null || paintingTextures.Length < 3)
        {
            Debug.LogError($"PaintingSelector: paintingTextures invalid. Length: {(paintingTextures == null ? 0 : paintingTextures.Length)}, Expected: >=3");
            return;
        }

        // Set painting (e.g., random or specific)
        CurrentPaintingIndex = Random.Range(0, paintingTextures.Length); // Example: random
        if (paintingTextures.Length > CurrentPaintingIndex)
        {
            paintingImage.texture = paintingTextures[CurrentPaintingIndex];
            Debug.Log($"PaintingSelector set painting index: {CurrentPaintingIndex}, Texture: {paintingTextures[CurrentPaintingIndex].name}");
        }
        else
        {
            Debug.LogError("PaintingSelector: CurrentPaintingIndex out of range");
        }
    }
}