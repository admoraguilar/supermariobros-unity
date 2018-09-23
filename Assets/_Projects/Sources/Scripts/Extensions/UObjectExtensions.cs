using UnityEngine;
using System.Collections.Generic;

public static class UObjectExtensions {
    public static T AddComponentAsChildObject<T>(this Object uObject, Transform parent, string path) where T : Component {
        T obj = null;

        GameObject childObject = FindDeepChild(uObject, path, TransformExtensions.SearchType.BreadthFirst);
        if(!childObject) {
            childObject = MakeChildObject(uObject, parent, path);
        }

        obj = childObject.GetComponent<T>();
        if(!obj) {
            obj = childObject.AddComponent<T>();
        }

        return obj;
    }

    public static GameObject MakeChildObject(this Object uObject, Transform parent, string path) {
        string[] paths = path.Split('/');
        GameObject obj = null;
        Transform objTr = parent;

        for(int i = 0; i < paths.Length; i++) {
            string p = paths[i];
            obj = null;

            for(int a = 0; a < objTr.childCount; a++) {
                GameObject go = objTr.GetChild(a).gameObject;
                if(go.name == p) {
                    obj = go;
                    break;
                }
            }

            if(!obj) {
                obj = new GameObject(p);
                obj.GetComponent<Transform>().SetParent(objTr, false);
            }

            if(obj) {
                objTr = obj.GetComponent<Transform>();
            }
        }

        return obj;
    }

    public static GameObject FindDeepChild(this Object uObject, string path, TransformExtensions.SearchType searchType) {
        GameObject go = null;

        go = uObject as GameObject;
        if(!go) {
            go = (uObject as Behaviour).gameObject;
        }

        Transform childTr = go.GetComponent<Transform>().FindDeepChild(path, searchType);
        GameObject childGo = null;
        if(childTr) {
            childGo = childTr.gameObject;
        }

        return childGo;
    }

    public static T AddOrGetComponent<T>(this Object uObject, int index = 0) where T : Component {
        GameObject go = null;

        go = uObject as GameObject;
        if(!go) {
            go = (uObject as Behaviour).gameObject;
        }

        List<T> components = new List<T>(go.GetComponents<T>());

        while(index >= components.Count) {
            components.Add(go.AddComponent<T>());
        }
        //if(index >= components.Count) {
        //    go.AddComponent<T>();
        //}

        return components[index];
    }

    public static T Spawn<T>(this Object uObject, T toSpawn, Transform parent = null, bool isDontDestroy = false) where T : UnityEngine.Object {
        T obj = Object.Instantiate(toSpawn);
        obj.name = toSpawn.name;
        if(isDontDestroy && parent == null) Object.DontDestroyOnLoad(obj);

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
}
