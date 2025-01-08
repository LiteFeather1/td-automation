using System;
using UnityEngine;
using UnityEditor;

public static class ShortCuts
{
    [MenuItem("Helpers/Screenshot", priority = 1)]
    public static void Screenshot()
    {
        ScreenCapture.CaptureScreenshot($"{Application.dataPath}/Screenshots/Screenshot_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.png");
    }
}