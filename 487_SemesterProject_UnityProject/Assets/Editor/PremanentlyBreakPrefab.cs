using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class PermanentlyBreakPrefab
{

    /// <summary>
    /// From:
    /// http://forum.unity3d.com/threads/breaking-connection-from-gameobject-to-prefab-for-good.82883/
    /// Aquisition 09/03/2018
    /// </summary>
    [MenuItem("Utilities/GameObject/Permanently Break Prefab")]
    public static void ExecuteOnSelectedObject()
    {
        GameObject[] selected = Selection.gameObjects;
        Selection.activeGameObject = null;

        bool dirtyScene = false;

        foreach (var gameObject in selected)
        {
            var prefabType = PrefabUtility.GetPrefabType(gameObject);

            //Don't do the thing for PrefabType.None (not a prefab), Prefab (prefab asset) or ModelPrefab (model asset)
            if (prefabType != PrefabType.PrefabInstance &&
                prefabType != PrefabType.DisconnectedPrefabInstance &&
                prefabType != PrefabType.ModelPrefabInstance &&
                prefabType != PrefabType.DisconnectedModelPrefabInstance &&
                prefabType != PrefabType.MissingPrefabInstance)
            {
                continue;
            }

            dirtyScene = true;
            PrefabUtility.DisconnectPrefabInstance(gameObject);
            Object prefab = PrefabUtility.CreateEmptyPrefab("Assets/dummy.prefab");
            PrefabUtility.ReplacePrefab(gameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);
            PrefabUtility.DisconnectPrefabInstance(gameObject);
            AssetDatabase.DeleteAsset("Assets/dummy.prefab");
        }


        if (dirtyScene)
        {
            var sceneCount = EditorSceneManager.sceneCount;
            for (int i = 0; i < sceneCount; i++)
            {
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetSceneAt(i));
            }
        }

        Selection.objects = selected;
    }    
    
    public static void ExecuteOnGivenObject(GameObject[] selected)
    {                
        bool dirtyScene = false;

        foreach (var gameObject in selected)
        {
            PrefabType prefabType = PrefabUtility.GetPrefabType(gameObject);

            //Don't do the thing for PrefabType.None (not a prefab), Prefab (prefab asset) or ModelPrefab (model asset)
            if (prefabType != PrefabType.PrefabInstance &&
                prefabType != PrefabType.DisconnectedPrefabInstance &&
                prefabType != PrefabType.ModelPrefabInstance &&
                prefabType != PrefabType.DisconnectedModelPrefabInstance &&
                prefabType != PrefabType.MissingPrefabInstance)
            {
                continue;
            }

            dirtyScene = true;
            PrefabUtility.DisconnectPrefabInstance(gameObject);
            Object prefab = PrefabUtility.CreateEmptyPrefab("Assets/dummy.prefab");
            PrefabUtility.ReplacePrefab(gameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);
            PrefabUtility.DisconnectPrefabInstance(gameObject);
            AssetDatabase.DeleteAsset("Assets/dummy.prefab");
        }


        if (dirtyScene)
        {
            var sceneCount = EditorSceneManager.sceneCount;
            for (int i = 0; i < sceneCount; i++)
            {
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetSceneAt(i));
            }
        }
    }

}