using UnityEngine;
using UnityEngine.UI;

public class FadeTransition : MonoBehaviour
{
    public Image fadeImage;
    private float fadeDuration = 1f;

    public void FadeToBlack(System.Action onComplete)
    {
        StartCoroutine(Fade(0f, 1f, onComplete));
    }

    System.Collections.IEnumerator Fade(float startAlpha, float endAlpha, System.Action onComplete)
    {
        float time = 0;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, Mathf.Lerp(startAlpha, endAlpha, time / fadeDuration));
            yield return null;
        }
        onComplete?.Invoke();
    }
}