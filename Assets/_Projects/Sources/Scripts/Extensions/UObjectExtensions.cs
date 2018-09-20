using UnityEngine;


public static class UObjectExtensions {
    public static T Spawn<T>(this Object uObject, T toSpawn, Transform parent = null, bool dontDestroy = false) where T : UnityEngine.Object {
        T obj = Object.Instantiate(toSpawn);
        obj.name = toSpawn.name;
        if(dontDestroy && parent == null) Object.DontDestroyOnLoad(obj);

        if(parent) {
            Transform objT = null;

            GameObject go = obj as GameObject;
            if(go) objT = go.GetComponent<Transform>();

            Behaviour bh = obj as Behaviour;
            if(bh) objT = bh.GetComponent<Transform>();

            objT.SetParent(parent, false);
        }

        return obj;
    }

    public static T SpawnIfNoExistingChildren<T>(this Object uObject, string objectName, Transform parent) where T : Component {
        T obj = null;

        for(int i = 0; i < parent.childCount; i++) {
            GameObject go = parent.GetChild(i).gameObject;
            if(go.name == objectName) {
                obj = go.GetComponent<T>();
                if(obj == null) obj = go.AddComponent<T>();
                break;
            }
        }

        if(obj == null) {
            obj = Spawn(uObject, new GameObject(objectName), parent, false).AddComponent<T>();
        }

        return obj;
    }
}
