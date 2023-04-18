#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuildQuestion : MonoBehaviour
{
    static readonly string endPathEditor = "/Editor Client.exe";
    static readonly string endPathEditorServer = "/Editor Server.exe";
    [MenuItem("Build/Debug Client")]
    public static void BuildWindow()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = GetScenes();
        buildPlayerOptions.locationPathName =  EditorUtility.SaveFolderPanel("Choose Location of Built Applications", "Builds", "");
        if (buildPlayerOptions.locationPathName == string.Empty) return;
        buildPlayerOptions.locationPathName += endPathEditor;
        buildPlayerOptions.target = BuildTarget.StandaloneWindows;
        buildPlayerOptions.options = BuildOptions.Development;

        EditorSceneManager.SaveOpenScenes();
        EditorSceneManager.OpenScene("Assets/Scenes/Lobby.unity", OpenSceneMode.Single);
        FindObjectOfType<UserClient>(true)?.gameObject?.SetActive(true);
        FindObjectOfType<MainServer>(true)?.gameObject?.SetActive(false);
        EditorSceneManager.SaveOpenScenes();

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            UnityEngine.Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            UnityEngine.Debug.Log("Build failed");
        }
    }

    [MenuItem("Build/Debug Server")]
    public static void BuildWindowServer()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = GetScenes();
        buildPlayerOptions.locationPathName = EditorUtility.SaveFolderPanel("Choose Location of Built Applications", "Builds", "");
        if (buildPlayerOptions.locationPathName == string.Empty) return;
        buildPlayerOptions.locationPathName += endPathEditorServer;
        buildPlayerOptions.target = BuildTarget.StandaloneWindows;
        buildPlayerOptions.options = BuildOptions.Development;

        EditorSceneManager.SaveOpenScenes();
        EditorSceneManager.OpenScene("Assets/Scenes/Lobby.unity", OpenSceneMode.Single);
        FindObjectOfType<MainServer>(true)?.gameObject?.SetActive(true);
        FindObjectOfType<UserClient>(true)?.gameObject?.SetActive(false);
        EditorSceneManager.SaveOpenScenes();

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            UnityEngine.Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            UnityEngine.Debug.Log("Build failed");
        }
    }


    static string[] GetScenes()
    {
        List<string> scenes = new List<string>();
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
                scenes.Add(scene.path);
        }
        return scenes.ToArray();
    }
}
#endif