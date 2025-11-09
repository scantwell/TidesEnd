using UnityEngine;

namespace TidesEnd.Weapons
{
    /// <summary>
    /// Visual debug system for hybrid aiming.
    /// Shows 3D spheres and lines to visualize where bullets spawn and where they're aimed.
    /// Enable this to debug aiming issues in Game view (doesn't require Scene view).
    /// </summary>
    public class WeaponDebugVisualizer : MonoBehaviour
    {
        [Header("Debug Settings")]
        [SerializeField] private bool enableVisualization = false;
        [SerializeField] private float visualDuration = 2f;

        [Header("Visual Settings")]
        [SerializeField] private float sphereSize = 0.1f;
        [SerializeField] private float lineWidth = 0.02f;

        [Header("Colors")]
        [SerializeField] private Color cameraRayColor = Color.red;
        [SerializeField] private Color aimPointColor = Color.yellow;
        [SerializeField] private Color muzzleColor = Color.cyan;
        [SerializeField] private Color trajectoryColor = Color.green;

        private static WeaponDebugVisualizer instance;

        public static WeaponDebugVisualizer Instance => instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Debug.LogWarning($"Multiple WeaponDebugVisualizers detected! Destroying duplicate on {gameObject.name}");
                Destroy(this);
            }
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        /// <summary>
        /// Visualize a complete shot from hybrid aiming system
        /// </summary>
        public void VisualizeShotHybrid(Vector3 cameraPos, Vector3 aimPoint, Vector3 muzzlePos, Vector3 projectileDirection, float range)
        {
            if (!enableVisualization) return;

            // 1. Camera to aim point (what the player is looking at)
            DrawLine(cameraPos, aimPoint, cameraRayColor, visualDuration);
            DrawSphere(aimPoint, aimPointColor, visualDuration, sphereSize);

            // 2. Muzzle position (where bullet spawns)
            DrawSphere(muzzlePos, muzzleColor, visualDuration, sphereSize * 1.5f);

            // 3. Projectile trajectory (muzzle to aim point)
            DrawLine(muzzlePos, aimPoint, trajectoryColor, visualDuration);

            // 4. Extended trajectory (show where bullet will travel)
            Vector3 extendedEnd = muzzlePos + (projectileDirection * range);
            DrawLine(muzzlePos, extendedEnd, trajectoryColor * 0.5f, visualDuration);
        }

        /// <summary>
        /// Visualize camera raycast result
        /// </summary>
        public void VisualizeAimRay(Vector3 origin, Vector3 hitPoint, bool didHit)
        {
            if (!enableVisualization) return;

            Color color = didHit ? Color.red : Color.yellow;
            DrawLine(origin, hitPoint, color, visualDuration);

            if (didHit)
            {
                DrawSphere(hitPoint, aimPointColor, visualDuration, sphereSize);
            }
        }

        /// <summary>
        /// Visualize projectile spawn
        /// </summary>
        public void VisualizeProjectileSpawn(Vector3 position, Vector3 direction, float length)
        {
            if (!enableVisualization) return;

            DrawSphere(position, muzzleColor, visualDuration, sphereSize);
            DrawLine(position, position + direction * length, trajectoryColor, visualDuration);
        }

        /// <summary>
        /// Draw a 3D sphere that's visible in Game view
        /// </summary>
        private void DrawSphere(Vector3 position, Color color, float duration, float size)
        {
            // Create temporary sphere
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = "DebugSphere";
            sphere.transform.position = position;
            sphere.transform.localScale = Vector3.one * size;

            // Remove collider
            Collider col = sphere.GetComponent<Collider>();
            if (col != null) Destroy(col);

            // Set color
            Renderer renderer = sphere.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
                mat.color = color;
                renderer.material = mat;
            }

            // Auto-destroy
            Destroy(sphere, duration);
        }

        /// <summary>
        /// Draw a 3D line that's visible in Game view
        /// </summary>
        private void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
        {
            GameObject lineObj = new GameObject("DebugLine");
            LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();

            // Configure line renderer
            lineRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
            lineRenderer.useWorldSpace = true;

            // Auto-destroy
            Destroy(lineObj, duration);
        }

        /// <summary>
        /// Toggle visualization on/off at runtime
        /// </summary>
        public void ToggleVisualization()
        {
            enableVisualization = !enableVisualization;
            Debug.Log($"Weapon Debug Visualization: {(enableVisualization ? "ENABLED" : "DISABLED")}");
        }

        /// <summary>
        /// Enable visualization
        /// </summary>
        public void Enable()
        {
            enableVisualization = true;
        }

        /// <summary>
        /// Disable visualization
        /// </summary>
        public void Disable()
        {
            enableVisualization = false;
        }

        public bool IsEnabled => enableVisualization;
    }
}
