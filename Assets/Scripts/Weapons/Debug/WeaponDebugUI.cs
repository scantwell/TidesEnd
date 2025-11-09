using UnityEngine;
using UnityEngine.InputSystem;

namespace TidesEnd.Weapons
{
    /// <summary>
    /// On-screen UI debug panel for weapon aiming system.
    /// Shows real-time info about camera position, aim point, muzzle position, and distances.
    /// </summary>
    public class WeaponDebugUI : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool showPanel = false;
        [SerializeField] private KeyCode toggleKey = KeyCode.F3;

        [Header("UI Settings")]
        [SerializeField] private int fontSize = 18;
        [SerializeField] private Color textColor = Color.white;
        [SerializeField] private Color backgroundColor = new Color(0, 0, 0, 0.7f);

        private GUIStyle labelStyle;
        private GUIStyle boxStyle;
        private Rect panelRect;

        // Debug data
        private Vector3 lastCameraPos;
        private Vector3 lastAimPoint;
        private Vector3 lastMuzzlePos;
        private Vector3 lastProjectileDir;
        private float lastDistance;
        private bool lastRaycastHit;
        private string lastWeaponName = "None";

        private void OnEnable()
        {
            panelRect = new Rect(10, 10, 400, 300);
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;
            // Toggle panel with F3
            if (keyboard.f3Key.isPressed)
            {
                showPanel = !showPanel;
                Debug.Log($"Weapon Debug UI: {(showPanel ? "ENABLED" : "DISABLED")} (Press {toggleKey} to toggle)");
            }
        }

        private void OnGUI()
        {
            if (!showPanel) return;

            // Initialize styles
            if (labelStyle == null)
            {
                labelStyle = new GUIStyle(GUI.skin.label);
                labelStyle.fontSize = fontSize;
                labelStyle.normal.textColor = textColor;
                labelStyle.padding = new RectOffset(10, 10, 5, 5);

                boxStyle = new GUIStyle(GUI.skin.box);
                boxStyle.normal.background = MakeTex(2, 2, backgroundColor);
            }

            // Draw panel
            GUILayout.BeginArea(panelRect, boxStyle);
            GUILayout.Label("<b>WEAPON DEBUG PANEL</b>", labelStyle);
            GUILayout.Space(10);

            GUILayout.Label($"<b>Weapon:</b> {lastWeaponName}", labelStyle);
            GUILayout.Space(5);

            GUILayout.Label($"<b>Camera Position:</b> {FormatVector(lastCameraPos)}", labelStyle);
            GUILayout.Label($"<b>Muzzle Position:</b> {FormatVector(lastMuzzlePos)}", labelStyle);
            GUILayout.Label($"<b>Aim Point:</b> {FormatVector(lastAimPoint)}", labelStyle);
            GUILayout.Space(5);

            GUILayout.Label($"<b>Raycast Hit:</b> {(lastRaycastHit ? "YES" : "NO")}", labelStyle);
            GUILayout.Label($"<b>Distance to Target:</b> {lastDistance:F2}m", labelStyle);
            GUILayout.Space(5);

            GUILayout.Label($"<b>Projectile Direction:</b> {FormatVector(lastProjectileDir)}", labelStyle);
            GUILayout.Space(10);

            GUILayout.Label($"<color=yellow>Camera → Aim Point</color>", labelStyle);
            GUILayout.Label($"<color=cyan>Muzzle (spawn here)</color>", labelStyle);
            GUILayout.Label($"<color=lime>Muzzle → Aim Point (trajectory)</color>", labelStyle);
            GUILayout.Space(10);

            GUILayout.Label($"Press <b>{toggleKey}</b> to hide", labelStyle);

            GUILayout.EndArea();
        }

        /// <summary>
        /// Update debug data (called by Weapon.cs)
        /// </summary>
        public void UpdateDebugData(
            string weaponName,
            Vector3 cameraPos,
            Vector3 aimPoint,
            Vector3 muzzlePos,
            Vector3 projectileDir,
            bool raycastHit)
        {
            lastWeaponName = weaponName;
            lastCameraPos = cameraPos;
            lastAimPoint = aimPoint;
            lastMuzzlePos = muzzlePos;
            lastProjectileDir = projectileDir;
            lastRaycastHit = raycastHit;
            lastDistance = Vector3.Distance(cameraPos, aimPoint);
        }

        private string FormatVector(Vector3 v)
        {
            return $"({v.x:F2}, {v.y:F2}, {v.z:F2})";
        }

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        /// <summary>
        /// Toggle the debug panel on/off
        /// </summary>
        public void TogglePanel()
        {
            showPanel = !showPanel;
        }

        /// <summary>
        /// Enable the debug panel
        /// </summary>
        public void Show()
        {
            showPanel = true;
        }

        /// <summary>
        /// Disable the debug panel
        /// </summary>
        public void Hide()
        {
            showPanel = false;
        }

        public bool IsVisible => showPanel;
    }
}
