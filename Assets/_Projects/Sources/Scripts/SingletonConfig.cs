using UnityEngine;
using System;
using System.Collections.Generic;


[CreateAssetMenu(menuName = "SingletonConfig")]
public class SingletonConfig : ScriptableObject {
    public UnityEngine.Object[] Singleton;
}


public static class SingletonController {
    private static List<UnityEngine.Object> singletons = new List<UnityEngine.Object>();
    private static Dictionary<Type, UnityEngine.Object> cachedSingletons = new Dictionary<Type, UnityEngine.Object>();


    public static T Get<T>() where T : UnityEngine.Object {
        UnityEngine.Object value = null;

        // We cache the singleton so that the next time
        // we get the same type we won't do a linear search
        // anymore
        if(!cachedSingletons.TryGetValue(typeof(T), out value)) {
            foreach(var singleton in singletons) {
                if(typeof(T).IsAssignableFrom(singleton.GetType())) {
                    value = singleton;
                    cachedSingletons[typeof(T)] = singleton;
                    break;
                }
            }
        }

        return (T)value;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void RunOnStartup() {
        SingletonConfig config = Resources.LoadAll<SingletonConfig>("")[0];
        foreach(var singleton in config.Singleton) {
            if(!singleton) continue;

            GameObject go = singleton as GameObject;
            if(go) {
                // Instantiate the object and put it on 
                // DontDestroyOnLoad scene
                go = UnityEngine.Object.Instantiate(go);
                go.name = singleton.name;
                UnityEngine.Object.DontDestroyOnLoad(go);

                // Get the monobehaviours of this object as singletons
                singletons.AddRange(go.GetComponents<MonoBehaviour>());
            }
        }
    }
}
