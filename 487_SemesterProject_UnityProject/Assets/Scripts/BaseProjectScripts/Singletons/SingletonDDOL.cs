using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonDDOL<T> : MonoBehaviour where T : MonoBehaviour
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
                        newInstanceObject.name = "SingletonDDOL[" + typeof(T).ToString() + "]";
                        m_instance = newInstanceObject.AddComponent<T>();

                        DontDestroyOnLoad(newInstanceObject);
                    }
                    else
                    {
                        DontDestroyOnLoad(m_instance);
                    }
                }
            }

            return m_instance;
        }
    }

    void Awake()
    {        
        lock (m_lock)
        {            
            if (m_instance == null)
            {                
                m_instance = FindObjectOfType<T>();

                if (m_instance == null)
                {
                    GameObject newInstance = new GameObject();
                    newInstance.name = "SingletonDDOL[" + typeof(T).ToString() + "]";
                    m_instance = newInstance.AddComponent<T>();

                    DontDestroyOnLoad(newInstance);
                }
                else
                {
                    DontDestroyOnLoad(m_instance);
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
        Debug.Log("asdasd" + instance.name);
    }
}
