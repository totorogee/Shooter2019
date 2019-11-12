using UnityEngine;

public class PrefabSingleton<T> : MonoBehaviour where T : MonoBehaviour{

    private static T _instance;

    private static object _lock = new object();

    private static bool created = false;

    //Error code: Never used
#pragma warning disable 219
    protected virtual void Awake()
    {
        T t = PrefabSingleton<T>.Instance;
    }
#pragma warning restore 219


    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                Debug.Log("[Singleton] Instance '" + typeof(T) +
                "' already destroyed on application quit." +
                " Won't create again.");
                return _instance;
            }

            lock (_lock)
            {
                if (_instance == null && !created)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        Debug.LogError("[Singleton] Something went really wrong " +
                        " - there should never be more than 1 singleton!" +
                        " Reopenning the scene might fix it.");
                        return _instance;
                    }

                    if (_instance == null)
                    {
                        created = true;

                        GameObject go = Resources.Load<GameObject>("Singleton/" + typeof(T).Name);

                        if (go != null)
                        {
                            _instance = Instantiate<GameObject>(go).GetComponent<T>();
                        }else{

                            GameObject singleton = new GameObject();
                            _instance = singleton.AddComponent<T>();
                        }

                        //use base class type as object name for UnitySendMessage callback
                        _instance.gameObject.name = "~" + typeof(T).ToString();

                        Debug.Log("[Singleton] An instance of " + typeof(T) +
                                  " is needed in the scene, so '" + _instance.gameObject.name +
                                  "' was created with DontDestroyOnLoad.");
                    }
                    else
                    {
                        ////use base class type as object name for UnitySendMessage callback
                        //_instance.gameObject.name = "~" + typeof(T).ToString();

                        //Debug.Log("[Singleton] Using instance already created: " +
                        //_instance.gameObject.name);
                    }


                    DontDestroyOnLoad(_instance.transform.root.gameObject);
                }

                return _instance;
            }
        }
    }

    private static bool applicationIsQuitting = false;

    /// <summary>
    /// When Unity quits, it destroys objects in a random order.
    /// In principle, a Singleton is only destroyed when application quits.
    /// If any script calls Instance after it have been destroyed, 
    ///   it will create a buggy ghost object that will stay on the Editor scene
    ///   even after stopping playing the Application. Really bad!
    /// So, this was made to be sure we're not creating that buggy ghost object.
    /// </summary>
    public virtual void OnDestroy()
    {
        applicationIsQuitting = true;
    }
}
