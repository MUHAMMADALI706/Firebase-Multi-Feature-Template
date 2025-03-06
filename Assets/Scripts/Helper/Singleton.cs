using UnityEngine;
using System.Collections;

public class GenericSingletonClass<T> : MonoBehaviour where T : Component
{
    private static T mInstance;
    public static T Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = FindObjectOfType<T>();
                if (mInstance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    mInstance = obj.AddComponent<T>();
                }
            }
            return mInstance;
        }
    }

    public virtual void Awake()
    {
        mInstance = this as T;
    }
}