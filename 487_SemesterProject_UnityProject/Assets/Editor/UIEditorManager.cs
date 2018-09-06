using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public static class UIEditorManager
{

    #region UI
    [MenuItem("GameObject/UI/LiveLab/Image")]
    static void InstanitiateImage()
    {
        InstantiateObject("ObjectImage", "UI");
    }

    [MenuItem("GameObject/UI/LiveLab/ImageFill")]
    static void InstanitiateImageFill()
    {
        InstantiateObject("ObjectImageFill", "UI");
    }

    [MenuItem("GameObject/UI/LiveLab/Button")]
    static void InstanitiateButton()
    {
        InstantiateObject("ObjectButton", "UI");
    }

    [MenuItem("GameObject/UI/LiveLab/Toggle")]
    static void InstanitiateToggle()
    {
        InstantiateObject("ObjectToggle", "UI");
    }

    [MenuItem("GameObject/UI/LiveLab/InputField")]
    static void InstanitiateInputField()
    {
        InstantiateObject("ObjectInputField", "UI");
    }

    [MenuItem("GameObject/UI/LiveLab/Dropdown")]
    static void InstanitiateDropdown()
    {
        InstantiateObject("ObjectDropdown", "UI");
    }

    [MenuItem("GameObject/UI/LiveLab/TabContainer")]
    static void InstanitiateTabContainer()
    {
        InstantiateObject("ObjectTabs", "UI");
    }

    [MenuItem("GameObject/UI/LiveLab/ScrollRectGrid")]
    static void InstanitiateScrollRectGrid()
    {
        InstantiateObject("ScrollRectGrid", "UI");
    }

    [MenuItem("GameObject/UI/LiveLab/ScrollRectVertical")]
    static void InstanitiateScrollRectVertical()
    {
        InstantiateObject("ScrollRectVertical", "UI");
    }

    [MenuItem("GameObject/UI/LiveLab/ScrollRectHorizontal")]
    static void InstanitiateScrollRectHorizontal()
    {
        InstantiateObject("ScrollRectHorizontal", "UI");
    }
    #endregion

    #region 3D
    [MenuItem("GameObject/3D Object/LiveLab/Player")]
    static void InstanitiatePlayer()
    {
        InstantiateObject("Player", "3D");
    }
    #endregion

    /// <summary>
    /// Instantiates a UI object in the "Assets/Prefabs/UI/" folder, parents it to the first object in the current selection, then breaks the prefab instance.
    /// </summary>
    /// <param name="objectName">The name of the prefab in the folder. (Don't include the .prefab extension)</param>
    static void InstantiateObject(string objectName, string path)
    {        
        GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/LiveLabPrefabs/" + path + "/" + objectName + ".prefab", typeof(GameObject));
        GameObject spawnedObjectPrefab = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        if (Selection.gameObjects.Length > 0)
        {
            spawnedObjectPrefab.transform.SetParent(Selection.gameObjects[0].transform, false);
        }        
        PermanentlyBreakPrefab.ExecuteOnGivenObject(new GameObject[] { spawnedObjectPrefab });
    }
}
