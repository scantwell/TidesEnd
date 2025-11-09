using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

/// <summary>
/// Debug helper for diagnosing NetworkTransform synchronization issues.
/// Attach this to any networked object to visualize network state and interpolation.
/// </summary>
public class NetworkTransformDebugger : NetworkBehaviour
{
    [Header("Debug Settings")]
    [SerializeField] private bool showDebugGUI = true;
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private bool logPositionUpdates = false;
    [SerializeField] private bool logRotationUpdates = false;

    [Header("Visual Settings")]
    [SerializeField] private Color localPlayerColor = Color.green;
    [SerializeField] private Color remotePlayerColor = Color.red;
    [SerializeField] private float gizmoSize = 0.5f;

    private NetworkTransform networkTransform;
    private Vector3 lastPosition;
    private Quaternion lastRotation;
    private float lastUpdateTime;
    private int updateCount;
    private float averageUpdateRate;

    // Performance tracking
    private float[] updateDeltas = new float[60]; // Track last 60 updates
    private int deltaIndex = 0;

    private void Awake()
    {
        networkTransform = GetComponent<NetworkTransform>();
        if (networkTransform == null)
        {
            Debug.LogError($"[NetworkTransformDebugger] No NetworkTransform found on {gameObject.name}");
            enabled = false;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        lastPosition = transform.position;
        lastRotation = transform.rotation;
        lastUpdateTime = Time.time;
    }

    private void Update()
    {
        if (!IsSpawned) return;

        // Track position changes
        if (Vector3.Distance(transform.position, lastPosition) > 0.001f)
        {
            float delta = Time.time - lastUpdateTime;
            updateDeltas[deltaIndex] = delta;
            deltaIndex = (deltaIndex + 1) % updateDeltas.Length;

            if (logPositionUpdates)
            {
                float distance = Vector3.Distance(transform.position, lastPosition);
                Debug.Log($"[{(IsOwner ? "Owner" : "Remote")}] Position Update - Distance: {distance:F3}m, Delta: {delta:F3}s, Velocity: {(distance/delta):F2}m/s");
            }

            lastPosition = transform.position;
            lastUpdateTime = Time.time;
            updateCount++;
        }

        // Track rotation changes
        if (Quaternion.Angle(transform.rotation, lastRotation) > 0.1f)
        {
            if (logRotationUpdates)
            {
                float angle = Quaternion.Angle(transform.rotation, lastRotation);
                Debug.Log($"[{(IsOwner ? "Owner" : "Remote")}] Rotation Update - Angle: {angle:F2}°");
            }

            lastRotation = transform.rotation;
        }

        // Calculate average update rate
        if (updateCount > 0 && updateCount % 10 == 0)
        {
            CalculateAverageUpdateRate();
        }
    }

    private void CalculateAverageUpdateRate()
    {
        float sum = 0f;
        int validSamples = 0;

        for (int i = 0; i < updateDeltas.Length; i++)
        {
            if (updateDeltas[i] > 0)
            {
                sum += updateDeltas[i];
                validSamples++;
            }
        }

        if (validSamples > 0)
        {
            averageUpdateRate = 1f / (sum / validSamples); // Updates per second
        }
    }

    private void OnGUI()
    {
        if (!showDebugGUI || !IsSpawned) return;

        GUIStyle style = new GUIStyle(GUI.skin.box);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = 12;
        style.normal.textColor = IsOwner ? localPlayerColor : remotePlayerColor;

        string playerType = IsOwner ? "LOCAL PLAYER" : "REMOTE PLAYER";
        string serverState = IsServer ? " (SERVER)" : " (CLIENT)";

        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2.5f);
        screenPos.y = Screen.height - screenPos.y; // Flip Y for GUI

        if (screenPos.z > 0) // Only show if in front of camera
        {
            GUILayout.BeginArea(new Rect(screenPos.x - 100, screenPos.y - 80, 200, 160));
            GUILayout.Box($"=== {playerType}{serverState} ===\n" +
                         $"ClientID: {OwnerClientId}\n" +
                         $"Position: {transform.position.ToString("F2")}\n" +
                         $"Rotation: {transform.rotation.eulerAngles.y:F1}°\n" +
                         $"Update Rate: {averageUpdateRate:F1} Hz\n" +
                         $"Updates: {updateCount}\n" +
                         $"Interpolating: {!IsOwner}",
                         style, GUILayout.Width(200));
            GUILayout.EndArea();
        }

        // Global debug info in corner
        DrawGlobalDebugInfo();
    }

    private void DrawGlobalDebugInfo()
    {
        GUIStyle cornerStyle = new GUIStyle(GUI.skin.box);
        cornerStyle.alignment = TextAnchor.UpperLeft;
        cornerStyle.fontSize = 14;
        cornerStyle.normal.textColor = Color.white;

        GUILayout.BeginArea(new Rect(10, 10, 300, 250));

        string networkState = NetworkManager.Singleton.IsServer ? "SERVER" : "CLIENT";
        if (NetworkManager.Singleton.IsHost) networkState = "HOST";

        GUILayout.Box($"=== NETWORK DEBUG ===\n" +
                     $"Mode: {networkState}\n" +
                     $"Connected: {NetworkManager.Singleton.IsConnectedClient}\n" +
                     $"RTT: {GetRTT():F0}ms\n" +
                     $"FPS: {(1f / Time.deltaTime):F0}\n" +
                     $"\n=== NetworkTransform Settings ===\n" +
                     $"Pos Interp: {networkTransform.PositionMaxInterpolationTime:F2}s\n" +
                     $"Rot Interp: {networkTransform.RotationMaxInterpolationTime:F2}s\n" +
                     $"Quaternion Sync: {networkTransform.UseQuaternionSynchronization}\n" +
                     $"Interpolate: {networkTransform.Interpolate}",
                     cornerStyle, GUILayout.Width(300));

        GUILayout.EndArea();
    }

    private float GetRTT()
    {
        if (NetworkManager.Singleton == null) return 0f;

        // Get RTT from NetworkManager
        // This is an approximation - Netcode 2.6 doesn't expose RTT directly
        // You can use the NetworkTime system for more accurate measurements
        return Time.deltaTime * 1000f * 2f; // Rough estimate
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos || !Application.isPlaying || !IsSpawned) return;

        // Draw position sphere
        Gizmos.color = IsOwner ? localPlayerColor : remotePlayerColor;
        Gizmos.DrawWireSphere(transform.position, gizmoSize);

        // Draw velocity direction
        Vector3 velocity = (transform.position - lastPosition) / Time.deltaTime;
        if (velocity.magnitude > 0.1f)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, velocity.normalized * 2f);
        }

        // Draw forward direction
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position + Vector3.up * 1f, transform.forward * 1.5f);
    }

    // Console commands for runtime debugging
    [ContextMenu("Log Current State")]
    public void LogCurrentState()
    {
        Debug.Log($"=== NetworkTransform State ===\n" +
                 $"GameObject: {gameObject.name}\n" +
                 $"IsOwner: {IsOwner}\n" +
                 $"IsServer: {IsServer}\n" +
                 $"Position: {transform.position}\n" +
                 $"Rotation: {transform.rotation.eulerAngles}\n" +
                 $"Update Rate: {averageUpdateRate:F1} Hz\n" +
                 $"Total Updates: {updateCount}\n" +
                 $"PositionMaxInterpolationTime: {networkTransform.PositionMaxInterpolationTime}\n" +
                 $"RotationMaxInterpolationTime: {networkTransform.RotationMaxInterpolationTime}\n" +
                 $"UseQuaternionSynchronization: {networkTransform.UseQuaternionSynchronization}\n" +
                 $"Interpolate: {networkTransform.Interpolate}");
    }

    [ContextMenu("Reset Statistics")]
    public void ResetStatistics()
    {
        updateCount = 0;
        averageUpdateRate = 0f;
        deltaIndex = 0;
        for (int i = 0; i < updateDeltas.Length; i++)
        {
            updateDeltas[i] = 0f;
        }
        Debug.Log("Statistics reset");
    }

    [ContextMenu("Toggle Position Logging")]
    public void TogglePositionLogging()
    {
        logPositionUpdates = !logPositionUpdates;
        Debug.Log($"Position logging: {(logPositionUpdates ? "ON" : "OFF")}");
    }

    [ContextMenu("Toggle Rotation Logging")]
    public void ToggleRotationLogging()
    {
        logRotationUpdates = !logRotationUpdates;
        Debug.Log($"Rotation logging: {(logRotationUpdates ? "ON" : "OFF")}");
    }
}