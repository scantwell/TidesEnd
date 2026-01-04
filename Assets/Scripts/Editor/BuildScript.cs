using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System;
using System.IO;

/// <summary>
/// Automated build script for creating Windows and Linux builds.
/// Usage: Unity Editor > Tools > Build > Build All Platforms
/// Or run via command line for CI/CD pipelines.
/// </summary>
public class BuildScript : MonoBehaviour
{
    private static readonly string BuildOutputPath = "Builds";
    private static readonly string GameName = "TidesEnd";

    [MenuItem("Tools/Build/Build Windows (x64)")]
    public static void BuildWindows()
    {
        BuildPlatform(BuildTarget.StandaloneWindows64, "Windows");
    }

    [MenuItem("Tools/Build/Build Linux (x64)")]
    public static void BuildLinux()
    {
        BuildPlatform(BuildTarget.StandaloneLinux64, "Linux");
    }

    [MenuItem("Tools/Build/Build All Platforms")]
    public static void BuildAll()
    {
        Debug.Log("=== Starting Multi-Platform Build ===");

        bool windowsSuccess = BuildPlatform(BuildTarget.StandaloneWindows64, "Windows");
        bool linuxSuccess = BuildPlatform(BuildTarget.StandaloneLinux64, "Linux");

        Debug.Log("\n=== Build Summary ===");
        Debug.Log($"Windows: {(windowsSuccess ? "SUCCESS" : "FAILED")}");
        Debug.Log($"Linux: {(linuxSuccess ? "SUCCESS" : "FAILED")}");

        if (windowsSuccess && linuxSuccess)
        {
            Debug.Log($"\n✓ All builds completed successfully!");
            Debug.Log($"Build location: {Path.GetFullPath(BuildOutputPath)}");

            // Open builds folder
            EditorUtility.RevealInFinder(BuildOutputPath);
        }
        else
        {
            Debug.LogError("One or more builds failed. Check console for details.");
        }
    }

    private static bool BuildPlatform(BuildTarget target, string platformName)
    {
        Debug.Log($"\n=== Building {platformName} ===");

        // Check if build support is installed
        if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Standalone, target))
        {
            Debug.LogError($"✗ {platformName} build target is NOT supported!");
            Debug.LogError($"  Please install {platformName} Build Support via Unity Hub:");
            Debug.LogError($"  1. Open Unity Hub");
            Debug.LogError($"  2. Go to 'Installs'");
            Debug.LogError($"  3. Click the gear icon on your Unity version (6000.2.8f1)");
            Debug.LogError($"  4. Select 'Add Modules'");
            Debug.LogError($"  5. Check '{platformName} Build Support (Mono)'");
            Debug.LogError($"  6. Click 'Install'");
            return false;
        }

        Debug.Log($"✓ {platformName} build support is installed");

        // Determine file extension
        string extension = target == BuildTarget.StandaloneWindows64 ? ".exe" : "";

        // Build path
        string buildPath = Path.Combine(BuildOutputPath, platformName, $"{GameName}{extension}");

        // Ensure directory exists
        string directoryPath = Path.GetDirectoryName(buildPath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Get scenes in build
        string[] scenes = GetScenePaths();
        if (scenes.Length == 0)
        {
            Debug.LogError($"No scenes found in build settings! Add scenes via File > Build Settings.");
            return false;
        }

        Debug.Log($"Building to: {buildPath}");
        Debug.Log($"Scenes included: {string.Join(", ", scenes)}");

        // Configure build options
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = buildPath,
            target = target,
            options = BuildOptions.None // Change to BuildOptions.Development for debug builds
        };

        // Execute build
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            // Verify files actually exist
            bool filesExist = File.Exists(buildPath) || Directory.Exists(Path.Combine(directoryPath, $"{GameName}_Data"));

            if (!filesExist)
            {
                Debug.LogError($"✗ {platformName} build reported success but no files were created!");
                Debug.LogError($"  Expected at: {buildPath}");
                Debug.LogError($"  This usually means the build module isn't properly installed.");
                return false;
            }

            Debug.Log($"✓ {platformName} build succeeded!");
            Debug.Log($"  Size: {FormatBytes(summary.totalSize)}");
            Debug.Log($"  Time: {summary.totalTime.TotalSeconds:F1}s");
            Debug.Log($"  Output: {summary.outputPath}");

            // Copy steam_appid.txt to build directory (required for Facepunch Steamworks)
            CopySteamAppIdFile(directoryPath);

            // Set execute permissions on Linux builds
            if (target == BuildTarget.StandaloneLinux64)
            {
                SetLinuxExecutablePermissions(buildPath);
                CopySteamLibraryForLinux(directoryPath);
                CreateLinuxReadme(directoryPath);
            }

            // List created files
            if (Directory.Exists(directoryPath))
            {
                var files = Directory.GetFiles(directoryPath, "*", SearchOption.TopDirectoryOnly);
                var dirs = Directory.GetDirectories(directoryPath, "*", SearchOption.TopDirectoryOnly);
                Debug.Log($"  Files created: {files.Length} files, {dirs.Length} folders");
            }

            return true;
        }
        else
        {
            Debug.LogError($"✗ {platformName} build failed!");
            Debug.LogError($"  Result: {summary.result}");

            // Log errors
            foreach (var step in report.steps)
            {
                foreach (var message in step.messages)
                {
                    if (message.type == LogType.Error || message.type == LogType.Exception)
                    {
                        Debug.LogError($"  {message.content}");
                    }
                }
            }
            return false;
        }
    }

    private static string[] GetScenePaths()
    {
        var scenes = new string[EditorBuildSettings.scenes.Length];
        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }
        return scenes;
    }

    private static void CopySteamAppIdFile(string buildDirectory)
    {
        // Find steam_appid.txt in project root
        string projectRoot = Path.GetDirectoryName(Application.dataPath);
        string sourceFile = Path.Combine(projectRoot, "steam_appid.txt");
        string destFile = Path.Combine(buildDirectory, "steam_appid.txt");

        if (File.Exists(sourceFile))
        {
            try
            {
                File.Copy(sourceFile, destFile, overwrite: true);
                Debug.Log($"  ✓ Copied steam_appid.txt to build directory");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"  ⚠ Failed to copy steam_appid.txt: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning($"  ⚠ steam_appid.txt not found at: {sourceFile}");
            Debug.LogWarning($"  Steam integration may not work without this file!");
        }
    }

    private static void CopySteamLibraryForLinux(string buildDirectory)
    {
        // Find libsteam_api.so in the Plugins folder
        string pluginsDir = Path.Combine(buildDirectory, $"{GameName}_Data", "Plugins");
        string sourceLib = Path.Combine(pluginsDir, "libsteam_api.so");
        string destLib = Path.Combine(buildDirectory, "libsteam_api.so");

        if (File.Exists(sourceLib))
        {
            try
            {
                File.Copy(sourceLib, destLib, overwrite: true);
                Debug.Log($"  ✓ Copied libsteam_api.so to executable directory");
                Debug.Log($"    From: {pluginsDir}");
                Debug.Log($"    To: {buildDirectory}");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"  ⚠ Failed to copy libsteam_api.so: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning($"  ⚠ libsteam_api.so not found at: {sourceLib}");
            Debug.LogWarning($"  Steam integration may not work on Linux!");
        }
    }

    private static void SetLinuxExecutablePermissions(string executablePath)
    {
        // On Windows, we can't directly set Unix permissions
        // Try to use WSL/Git Bash if available, otherwise provide instructions

        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            try
            {
                // Try using WSL to set permissions
                var processInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "wsl",
                    Arguments = $"chmod +x \"{executablePath.Replace("\\", "/")}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                var process = System.Diagnostics.Process.Start(processInfo);
                process.WaitForExit(5000);

                if (process.ExitCode == 0)
                {
                    Debug.Log($"  ✓ Set execute permissions on Linux binary using WSL");
                    return;
                }
            }
            catch
            {
                // WSL not available, that's okay
            }

            // If WSL didn't work, create a helper script
            CreateLinuxPermissionsScript(Path.GetDirectoryName(executablePath), Path.GetFileName(executablePath));
        }
    }

    private static void CreateLinuxPermissionsScript(string buildDirectory, string executableName)
    {
        // Create a shell script that users can run on Linux to fix permissions
        string fixPermScript = Path.Combine(buildDirectory, "fix_permissions.sh");
        string fixPermContent = $"#!/bin/bash\nchmod +x {executableName}\nchmod +x launch.sh\necho \"Execute permissions set\"\n";

        // Create a launch script that sets up Steam environment
        string launchScript = Path.Combine(buildDirectory, "launch.sh");
        string launchContent = $@"#!/bin/bash
# TidesEnd Linux Launch Script
# This script sets up the Steam environment and launches the game

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e ""${{GREEN}}TidesEnd - Linux Launcher${{NC}}""
echo ""==============================""

# Check if Steam is running
if ! pgrep -x ""steam"" > /dev/null; then
    echo -e ""${{YELLOW}}WARNING: Steam is not running!${{NC}}""
    echo ""Please start Steam before launching the game.""
    echo """"
    read -p ""Start Steam now? (y/n) "" -n 1 -r
    echo
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        steam &
        echo ""Waiting for Steam to start...""
        sleep 5
    else
        echo -e ""${{RED}}Cannot connect to Steam without Steam client running.${{NC}}""
        exit 1
    fi
fi

# Set execute permission if needed
if [ ! -x ""{executableName}"" ]; then
    echo ""Setting execute permission on {executableName}...""
    chmod +x ""{executableName}""
fi

# Set library path to find Steam libraries and game libraries
# Add current directory first (where we copied libsteam_api.so)
export LD_LIBRARY_PATH=""$(pwd):$LD_LIBRARY_PATH""

# Also add the Plugins directory as fallback
if [ -d ""{GameName}_Data/Plugins"" ]; then
    export LD_LIBRARY_PATH=""$(pwd)/{GameName}_Data/Plugins:$LD_LIBRARY_PATH""
fi

# Add Steam runtime paths
if [ -d ""$HOME/.steam/steam/ubuntu12_32/steam-runtime/lib/x86_64-linux-gnu"" ]; then
    export LD_LIBRARY_PATH=""$HOME/.steam/steam/ubuntu12_32/steam-runtime/lib/x86_64-linux-gnu:$LD_LIBRARY_PATH""
fi

if [ -d ""$HOME/.local/share/Steam/ubuntu12_32/steam-runtime/lib/x86_64-linux-gnu"" ]; then
    export LD_LIBRARY_PATH=""$HOME/.local/share/Steam/ubuntu12_32/steam-runtime/lib/x86_64-linux-gnu:$LD_LIBRARY_PATH""
fi

echo ""Library search path configured""
echo ""LD_LIBRARY_PATH: $LD_LIBRARY_PATH""

# Check for steam_appid.txt
if [ ! -f ""steam_appid.txt"" ]; then
    echo -e ""${{RED}}ERROR: steam_appid.txt not found!${{NC}}""
    echo ""Creating steam_appid.txt with AppID 480 (Spacewar - testing)""
    echo ""480"" > steam_appid.txt
fi

echo -e ""${{GREEN}}Launching TidesEnd...${{NC}}""
echo """"

# Launch the game
./""{executableName}"" ""$@""
";

        try
        {
            File.WriteAllText(fixPermScript, fixPermContent);
            File.WriteAllText(launchScript, launchContent);

            Debug.LogWarning($"  ⚠ Could not set execute permissions from Windows");
            Debug.Log($"  ✓ Created launch.sh - Linux users should run this instead of the exe");
            Debug.Log($"  ✓ Created fix_permissions.sh - sets permissions if needed");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"  ⚠ Could not create launch scripts: {e.Message}");
            Debug.LogWarning($"  → User must manually run: chmod +x {executableName}");
        }
    }

    private static void CreateLinuxReadme(string buildDirectory)
    {
        string readmePath = Path.Combine(buildDirectory, "README_LINUX.txt");
        string readmeContent = @"TidesEnd - Linux Installation Guide
=====================================

QUICK START:
-----------
1. Make sure Steam is running
2. Run: bash launch.sh

That's it! The launch.sh script handles everything automatically.


MANUAL LAUNCH (if launch.sh doesn't work):
------------------------------------------
1. Make sure Steam is running
2. Set execute permission: chmod +x TidesEnd
3. Launch the game: ./TidesEnd


TROUBLESHOOTING:
---------------

Problem: ""Permission denied"" error
Solution: Run: bash fix_permissions.sh
          Or manually: chmod +x TidesEnd

Problem: Game doesn't connect to Steam
Solutions:
  - Make sure Steam client is running BEFORE launching the game
  - Check that steam_appid.txt exists in the game folder
  - Try launching with: steam steam://run/480//$(pwd)/TidesEnd
  - Check Steam logs: ~/.local/share/Steam/logs/

Problem: Missing libraries error
Solution: Install Steam Runtime libraries:
  Ubuntu/Debian: sudo apt install steam
  The game uses Steam's runtime libraries

Problem: ""Steam not showing game""
Solution: This is normal! AppID 480 is ""Spacewar"" (Steam's test app)
          Your friends will see you playing ""Spacewar""


MULTIPLAYER CONNECTION:
----------------------
This game uses Steam P2P networking.

To play with friends:
1. Both players must have Steam running
2. Host starts the game and creates a session
3. Client connects through the in-game menu
4. You should appear as playing ""Spacewar"" on Steam


SYSTEM REQUIREMENTS:
-------------------
- Steam client installed and running
- 64-bit Linux distribution
- Graphics drivers supporting OpenGL/Vulkan


For support or issues, check the game's documentation.
";

        try
        {
            File.WriteAllText(readmePath, readmeContent);
            Debug.Log($"  ✓ Created README_LINUX.txt with installation instructions");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"  ⚠ Could not create README: {e.Message}");
        }
    }

    private static string FormatBytes(ulong bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:F2} {sizes[order]}";
    }

    // Command-line build support for CI/CD
    public static void CommandLineBuildAll()
    {
        BuildAll();

        // Exit with appropriate code
        bool success = Directory.Exists(Path.Combine(BuildOutputPath, "Windows")) &&
                      Directory.Exists(Path.Combine(BuildOutputPath, "Linux"));

        EditorApplication.Exit(success ? 0 : 1);
    }
}
