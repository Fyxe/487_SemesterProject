using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    static object m_lock = new object();

    protected static T m_instance;
    public static T instance
    {
        get
        {
            lock (m_lock)
            {
                if (m_instance == null)
                {
                    m_instance = FindObjectOfType<T>();

                    if (m_instance == null)
                    {
                        GameObject newInstanceObject = new GameObject();
                        newInstanceObject.name = "Singleton[" + typeof(T).ToString() + "]";
                        m_instance = newInstanceObject.AddComponent<T>();
                    }
                }
            }

            return m_instance;
        }
    }

    private void Awake()
    {
        lock (m_lock)
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<T>();

                if (m_instance == null)
                {
                    GameObject newInstance = new GameObject();
                    newInstance.name = "Singleton[" + typeof(T).ToString() + "]";
                    m_instance = newInstance.AddComponent<T>();
                }
            }
        }

        Initialize();
    }

    /// <summary>
    /// Called in Awake() after caching/creating the singleton instance. Implement this instead of implementing Awake().
    /// </summary>
    protected virtual void Initialize()
    {

    }
}
