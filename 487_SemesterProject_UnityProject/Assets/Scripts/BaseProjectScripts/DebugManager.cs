using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : Singleton<DebugManager> {

    [Header("Main")]
    public bool enableDebug;    // Should disable all other debug functions
    // have different booleans for different aspects of the game
    // they should be able to be changed here

    // For example
    // [Header("Enemy Controller")]
    // public bool debugRaycasts;
    // public bool debugPathfindingPath;
    [Header("Console Log")]
    public bool debugLogLogs;
    public bool debugLogWarnings;
    public bool debugLogErrors;

    [Header("LoadSceneManager")]
    public bool debugFinishedLoadingSceneName;
    public bool debugSceneNameWhenCalled;

    [Header("LocalDataManager")]
    public bool debugSerializationSuccessMessages;

    [Header("Extensions")]
    public bool debugMousePositionRay;

    [Header("FileSystem")]
    [Tooltip("Turn this on to upload and download files using the server. Try to only turn this on if the user is logged in.")]
    public bool useFileServer;
}
