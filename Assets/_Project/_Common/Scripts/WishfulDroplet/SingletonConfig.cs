using UnityEngine;
using System;
using System.Collections.Generic;


namespace WishfulDroplet {
    [CreateAssetMenu(menuName = "SingletonConfig")]
    public class SingletonConfig : ScriptableObject {
        public SingletonSet[] SingletonSets;

        [Serializable]
        public class SingletonSet {
            public string label;
            public GameObject[] gameObjectSingletons = new GameObject[0];
            public ScriptableObject[] scriptableObjectSingletons = new ScriptableObject[0];
            public bool isEnabled = true;
        }
    }


    public static class Singleton {
        private static List<object> singletons = new List<object>();
        private static Dictionary<Type, object> cachedSingletons = new Dictionary<Type, object>();


        public static T Get<T>() where T : class {
            object value = null;

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
            SingletonConfig[] configs = Resources.LoadAll<SingletonConfig>("");
            SingletonConfig config = configs.Length > 0 ? configs[0] : null;

            if(!config) {
                Debug.Log("No singletons loaded.");
                return;
            }

            foreach(var singletonSet in config.SingletonSets) {
                if(!singletonSet.isEnabled) continue;

                foreach(var singleton in singletonSet.gameObjectSingletons) {
                    if(!singleton) continue;

                    GameObject go = singleton as GameObject;
                    if(go) {
                        go = UnityEngine.Object.Instantiate(go);
                        go.name = singleton.name;
                        UnityEngine.Object.DontDestroyOnLoad(go);

                        singletons.AddRange(go.GetComponents<MonoBehaviour>());
                    }
                }

                foreach(var singleton in singletonSet.scriptableObjectSingletons) {
                    if(!singleton) continue;

                    ScriptableObject so = singleton as ScriptableObject;
                    if(so) {
                        singletons.Add(so);
                    }
                }
            }
        }
    }
}