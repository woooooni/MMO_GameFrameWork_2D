using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MultiplayersBuildAndRun
{
    [MenuItem("Tools/Run Multiplayer/Windows/2 players")]
    static void PerformWin64Build2()
    {
        PerformWin64Build(2);
    }

    [MenuItem("Tools/Run Multiplayer/Windows/3 players")]
    static void PerformWin64Build3()
    {
        PerformWin64Build(3);
    }

    [MenuItem("Tools/Run Multiplayer/Windows/4 players")]
    static void PerformWin64Build4()
    {
        PerformWin64Build(4);
    }

    [MenuItem("Tools/Run Multiplayer/MacOS/2Players")]
    static void PerformMacOSBuild2()
    {
        PerformMacOSBuild(2);
    }
    
    [MenuItem("Tools/Run Multiplayer/MacOS/3Players")]
    static void PerformMacOSBuild3()
    {
        PerformMacOSBuild(3);
    }
    
    [MenuItem("Tools/Run Multiplayer/MacOS/4Players")]
    static void PerformMacOSBuild4()
    {
        PerformMacOSBuild(4);
    }


    static void PerformWin64Build(int playerCount)
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);

        for(int i = 1; i<=playerCount; i++)
        {
            string location = "Builds/Win64/" + GetProjectName() + i.ToString() + "/" + GetProjectName() + i.ToString() + ".exe";
            BuildPipeline.BuildPlayer(GetScenePaths(), location,
                BuildTarget.StandaloneWindows64, BuildOptions.AutoRunPlayer);
        }
    }
    
    static void PerformMacOSBuild(int playerCount)
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX);

        for(int i = 1; i<=playerCount; i++)
        {
            string location = "Builds/MacOS/" + GetProjectName() + i.ToString() + "/" + GetProjectName() + i.ToString() + ".app";
            BuildPipeline.BuildPlayer(GetScenePaths(), location,
                BuildTarget.StandaloneOSX, BuildOptions.AutoRunPlayer);
        }
    }

    static string GetProjectName()
    {
        string[] s = Application.dataPath.Split('/');
        return s[s.Length - 2];
    }

    static string[] GetScenePaths()
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];

        for(int i=0; i< scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }
        return scenes;
    }
}