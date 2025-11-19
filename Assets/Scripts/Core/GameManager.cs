using UnityEngine;

namespace TidesEnd.Core
{
    /// <summary>
    /// Central coordinator for game systems. Manages high-level game flow.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Performance Settings")]
        [SerializeField] private int targetFrameRate = 144;
        [SerializeField] private bool useVSync = false;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Set consistent frame rate for predictable weapon behavior
            Application.targetFrameRate = targetFrameRate;
            QualitySettings.vSyncCount = useVSync ? 1 : 0;

            Debug.Log($"GameManager: Target FPS = {targetFrameRate}, VSync = {useVSync}");
        }

        void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void QuitGame()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}
