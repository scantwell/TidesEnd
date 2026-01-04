// SimpleFogAnimation.cs
using UnityEngine;
using UnityEngine.UI;

public class SimpleFogAnimation : MonoBehaviour
{
    [SerializeField] private float fadeSpeed = 0.5f;
    [SerializeField] private float minAlpha = 0.1f;
    [SerializeField] private float maxAlpha = 0.3f;
    
    private CanvasGroup canvasGroup;
    private float targetAlpha;
    
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        targetAlpha = maxAlpha;
    }
    
    void Update()
    {
        // Fade in and out
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
        
        // Switch direction when close to target
        if (Mathf.Abs(canvasGroup.alpha - targetAlpha) < 0.05f)
        {
            targetAlpha = (targetAlpha == maxAlpha) ? minAlpha : maxAlpha;
        }
    }
}