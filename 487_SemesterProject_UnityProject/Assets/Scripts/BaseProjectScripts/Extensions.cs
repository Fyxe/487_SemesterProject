using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This is a class for extending the different functions within C# and Unity. 
/// It was created to solve a few specfic issues and therefor is only in this one script.
/// If this becomes too large (1,000+ lines), this class will need to be separated into different
/// scripts, such as 'StringExtensions', 'GenericExtensions', etc.
/// </summary>
public static class Extensions {

    // ======================================================================================================
    // generic
    // ======================================================================================================

    /// <summary>
    /// Logs the current variable to the console. Useful in cases such as "if (var.Log()){}"
    /// </summary>
    /// <typeparam name="T">The type of the object you want to log.</typeparam>
    /// <param name="toLog">The object you want to log.</param>
    /// <param name="name">A preface to the log you're about to export.</param>
    /// <returns>Returns the object given.</returns>
    public static T Log<T>(this T toLog, string name = "")
    {
        if (DebugManager.instance.enableDebug && DebugManager.instance.debugLogLogs)
        {
            if (name == "")
            {
                Debug.Log(toLog.ToString());
            }
            else
            {
                Debug.Log(name + ": " + toLog.ToString());
            }
        }
        return toLog;
    }

    /// <summary>
    /// Logs the current variable to the console as a warning. Useful in cases such as "if (var.LogWarning()){}"
    /// </summary>
    /// <typeparam name="T">The type of the object you want to log.</typeparam>
    /// <param name="toLog">The object you want to log.</param>
    /// <param name="name">A preface to the log you're about to export.</param>
    /// <returns>Returns the object given.</returns>
    public static T LogWarning<T>(this T toLog, string name = "")
    {
        if (DebugManager.instance.enableDebug && DebugManager.instance.debugLogWarnings)
        {
            if (name == "")
            {
                Debug.LogWarning(toLog.ToString());
            }
            else
            {
                Debug.LogWarning(name + ": " + toLog.ToString());
            }
        }
        return toLog;
    }

    /// <summary>
    /// Logs the current variable to the console as an error. Useful in cases such as "if (var.LogError()){}"
    /// </summary>
    /// <typeparam name="T">The type of the object you want to log.</typeparam>
    /// <param name="toLog">The object you want to log.</param>
    /// <param name="name">A preface to the log you're about to export.</param>
    /// <returns>Returns the object given.</returns>
    public static T LogError<T>(this T toLog, string name = "")
    {
        if (DebugManager.instance.enableDebug && DebugManager.instance.debugLogErrors)
        {
            if (name == "")
            {
                Debug.LogError(toLog.ToString());
            }
            else
            {
                Debug.LogError(name + ": " + toLog.ToString());
            }
        }
        return toLog;
    }

    // ======================================================================================================
    // char
    // ======================================================================================================


    // ======================================================================================================
    // bool
    // ======================================================================================================


    // ======================================================================================================
    // int
    // ======================================================================================================

    public static bool IsEven(this int toCheck)
    {
        return ((toCheck % 2) == 0);
    }

    // ======================================================================================================
    // float
    // ======================================================================================================


    // ======================================================================================================
    // string
    // ======================================================================================================

    /// <summary>
    /// Converts a string into upper camel case.
    /// </summary>
    /// <param name="variableName">The string being converted.</param>
    /// <returns>A new string with the conversion applied.</returns>
    public static string ConvertToCamelCaseWithSpace(this string variableName)
    {
        string retString = variableName;
        retString = System.Char.ToUpper(retString[0]) + retString.Substring(1, retString.Length - 1);
        for (int i = 1; i < retString.Length; i++)
        {
            if ((System.Char.IsUpper(retString[i])) && System.Char.IsLower(retString[i - 1]))
            {
                retString = retString.Substring(0, i) + " " + retString.Substring(i, (retString.Length - i));
            }
        }
        return retString;
    }

    /// <summary>
    /// Removes the spaces from the given string.
    /// </summary>
    /// <param name="toRemove">The string to remove spaces from.</param>
    /// <returns>The original string with no spaces</returns>
    public static string RemoveSpaces(this string toRemove)
    {
        string retString = toRemove;
        toRemove.Replace(" ", "");
        return retString;
    }

    /// <summary
    /// Removes the file name from a file path
    /// </summary>
    /// <param name="toRemove">The string to remove spaces from.</param>
    /// <returns>file path</returns>
    public static string RemoveFilename(this string fullpath)
    {
        string retString = fullpath;
        int greatestSlashIndex = retString.LastIndexOf('\\');
        int lastForwardsSlashIndex = retString.LastIndexOf('/');
        if(greatestSlashIndex < lastForwardsSlashIndex)
        {
            greatestSlashIndex = lastForwardsSlashIndex;
        }
        if(greatestSlashIndex<0)
        {
            return fullpath;
        }

        retString = fullpath.Substring(0, greatestSlashIndex);
        return retString;
    }

    // ======================================================================================================
    // IList
    // ======================================================================================================

    /// <summary>
    /// Returns a random value from the list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="toGetFrom">The list being provided.</param>
    /// <returns>A random element in the list.</returns>
    public static T GetRandomValue<T>(this IList<T> toGetFrom)
    {
        if (toGetFrom.Count == 0)
        {
            Debug.LogError("Cannot get anything from an empty list");
        }
        if (toGetFrom == null)
        {
            Debug.LogError("Cannot get anything from a null list");
        }
        return toGetFrom[Random.Range(0,toGetFrom.Count)];
    }

    /// <summary>
    /// Removes a random value from the list and returns it.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="toGetFrom">The list being provided.</param>
    /// <returns>The item removed from the list.</returns>
    public static T RemoveRandomValue<T>(this IList<T> toGetFrom)
    {
        if (toGetFrom.Count == 0)
        {
            Debug.LogError("Cannot get anything from an empty list");
        }
        if (toGetFrom == null)
        {
            Debug.LogError("Cannot get anything from a null list");
        }
        int index = Random.Range(0, toGetFrom.Count);
        T retT = toGetFrom[index];
        toGetFrom.Remove(retT);
        return retT;
    }

    /// <summary>
    /// An in-place shuffling of a list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="toShuffle">The list being provided.</param>
    public static void Shuffle<T>(this IList<T> toShuffle)
    {
        if (toShuffle == null)
        {
            Debug.LogError("Cannot shuffle a null list");
        }
        int max = toShuffle.Count - 1;
        for (int i = max; i > 0; i--)
        {
            int firstIndex = Random.Range(0,max + 1);
            T value = toShuffle[firstIndex];
            toShuffle[firstIndex] = toShuffle[i];
            toShuffle[i] = value;
        }
    }

    public static T GetLast<T>(this IList<T> toGetFrom)
    {
        if (toGetFrom.Count == 0)
        {
            return default(T);
        }
        else
        {
            return toGetFrom[toGetFrom.Count - 1];
        }
    }

    public static T GetFirst<T>(this IList<T> toGetFrom)
    {
        if (toGetFrom.Count == 0)
        {
            return default(T);
        }
        else
        {
            return toGetFrom[0];
        }
    }

    public static T RemoveLast<T>(this IList<T> toGetFrom)
    {
        return RemoveAndGetAt(toGetFrom, toGetFrom.Count - 1);
    }

    public static T RemoveFirst<T>(this IList<T> toGetFrom)
    {
        return RemoveAndGetAt(toGetFrom,0);
    }

    public static T RemoveAndGetAt<T>(this IList<T> toGetFrom, int index)
    {
        if (toGetFrom.Count == 0)
        {
            Debug.Log("asdasd");
            return default(T);
        }
        else
        {
            T temp = toGetFrom[index];
            toGetFrom.RemoveAt(index);
            return temp;
        }
    }

    // ======================================================================================================
    // Transform
    // ======================================================================================================

    /// <summary>
    /// Iterates through all children of the given parent and destroys them.
    /// </summary>
    /// <param name="parent">The parent whose children will be destroyed.</param>
    public static void DestroyChildren(this Transform parent)
    {
        if (parent == null)
        {
            Debug.LogError("Cannot destroy children of a null parent");
        }
        foreach (Transform i in parent)
        {
            Object.Destroy(i.gameObject);
        }        
    }

    /// <summary>
    /// Sets a transform's local position to zero, local rotation to the identity rotation, and the local scale to one.
    /// </summary>
    /// <param name="toReset">The transform to be reset.</param>
    public static void Reset(this Transform toReset)
    {
        toReset.localPosition = Vector3.zero;
        toReset.localRotation = Quaternion.identity;
        toReset.localScale = Vector3.one;
    }

    // ======================================================================================================
    // Vector3
    // ======================================================================================================

    /// <summary>
    /// Checks two vectors to see if their values are within certain boundaries of eachother.
    /// </summary>
    /// <param name="toCheck">The A vector being compared.</param>
    /// <param name="toCompareTo">The B vector being compared.</param>
    /// <param name="maxRange">The maximum range the vectors can vary and still be true.</param>
    /// <returns>If Vector A and B are within the max range of eachother.</returns>
    public static bool IsApproximately(this Vector3 toCheck, Vector3 toCompareTo, float maxRange)
    {
        bool xValues = Mathf.Abs(toCheck.x - toCompareTo.x) <= maxRange;
        bool yValues = Mathf.Abs(toCheck.y - toCompareTo.y) <= maxRange;
        bool zValues = Mathf.Abs(toCheck.z - toCompareTo.z) <= maxRange;
        return xValues && yValues && zValues;
    }

    // ======================================================================================================
    // Vector2
    // ======================================================================================================

    /// <summary>
    /// Checks two vectors to see if their values are within certain boundaries of eachother.
    /// </summary>
    /// <param name="toCheck">The A vector being compared.</param>
    /// <param name="toCompareTo">The B vector being compared.</param>
    /// <param name="maxRange">The maximum range the vectors can vary and still be true.</param>
    /// <returns>If Vector A and B are within the max range of eachother.</returns>
    public static bool IsApproximately(this Vector2 toCheck, Vector2 toCompareTo, float maxRange)
    {
        bool xValues = Mathf.Abs(toCheck.x - toCompareTo.x) <= maxRange;
        bool yValues = Mathf.Abs(toCheck.y - toCompareTo.y) <= maxRange;
        return xValues && yValues;
    }

    // ======================================================================================================
    // GameObject
    // ======================================================================================================

    /// <summary>
    /// Sets the layer of a gameobject and recursivly all of it's children.
    /// </summary>
    /// <param name="toSet">The gameobject to set the layer of.</param>
    /// <param name="newLayer">The new layer that all objects will be set to.</param>
    public static void SetLayer(this GameObject toSet, int newLayer)
    {
        toSet.layer = newLayer;
        foreach (Transform i in toSet.transform)
        {
            SetLayer(i.gameObject, newLayer);
        }
    }

    /// <summary>
    /// Sets the layer of a gameobject and recursivly all of it's children.
    /// </summary>
    /// <param name="toSet">The gameobject to set the layer of.</param>
    /// <param name="newLayer">The new layer that all objects will be set to.</param>
    public static void SetLayer(this GameObject toSet, string newLayerName)
    {
        int newLayer = 0;
        try
        {
            newLayer = LayerMask.NameToLayer(newLayerName);
        }
        catch
        {
            return;
        }

        toSet.layer = newLayer;
        foreach (Transform i in toSet.transform)
        {
            SetLayer(i.gameObject, newLayer);
        }
    }

    public static void ReplaceLayer(this GameObject toSet, int newLayer, int layerToReplace)
    {
        if (toSet.layer == layerToReplace)
        {
            toSet.layer = newLayer;
        }
        foreach (Transform i in toSet.transform)
        {
            ReplaceLayer(i.gameObject, newLayer, layerToReplace);
        }
    }

    /// <summary>
    /// Attempts to get the component provided, and if it fails, adds a new component and returns it.
    /// </summary>
    /// <typeparam name="T">A component.</typeparam>
    /// <param name="toGetFrom">The gameobject to attempt the Get and Add calls on.</param>
    /// <returns>A component that was found or the new one created.</returns>
    public static Component GetOrAddComponent<T>(this GameObject toGetFrom) where T : Component
    {
        Component retComp = toGetFrom.GetComponent<T>();
        if (retComp == null)
        {
            retComp = toGetFrom.AddComponent<T>();
        }
        return retComp;
    }

    // ======================================================================================================
    // ToggleGroup
    // ======================================================================================================

    // From: https://forum.unity.com/threads/how-to-get-reference-of-all-toggles-from-togglegroup.463534/
    private static System.Reflection.FieldInfo _toggleListMember;

    /// <summary>
    /// Gets the list of toggles. Do NOT add to the list, only read from it.
    /// </summary>
    /// <param name="grp"></param>
    /// <returns></returns>
    public static IList<Toggle> GetAllToggles(this ToggleGroup grp)
    {
        if (_toggleListMember == null)
        {
            _toggleListMember = typeof(ToggleGroup).GetField("m_Toggles", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (_toggleListMember == null)
                throw new System.Exception("UnityEngine.UI.ToggleGroup source code must have changed in latest version and is no longer compatible with this version of code.");
        }
        return _toggleListMember.GetValue(grp) as IList<Toggle>;
    }

    /// <summary>
    /// Returns the count of all toggles in the toggle group.
    /// </summary>
    /// <param name="grp">The toggle group affected.</param>
    /// <returns>Returns the count of all toggles in the toggle group.</returns>
    public static int Count(this ToggleGroup grp)
    {
        return GetAllToggles(grp).Count;
    }

    /// <summary>
    /// Returns a toggle at the given index.
    /// </summary>
    /// <param name="grp">The toggle group affected.</param>
    /// <param name="index">The index that is trying to be retrieved.</param>
    /// <returns>Returns a toggle at the given index.</returns>
    public static Toggle Get(this ToggleGroup grp, int index)
    {
        return GetAllToggles(grp)[index];
    }

}
