using UnityEngine;
using UnityEditor;


namespace WishfulDroplet.Editor {
    public static class EditorSingleton {
        public static T Get<T>() where T : ScriptableObject {
            T singleton = default(T);

            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T).Name));
            foreach(var guid in guids) {
                singleton = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
                if(singleton) break;
            }

            if(!singleton) {
                string path = Utilities.GetOrCreateFolderPath(string.Format("{0}/{1}", Utilities.GetFolderPathWithName("WishfulDroplet"), "Editor/Resources"));
                singleton = Utilities.CreateScriptableObjectAsset<T>(path);
                Selection.activeObject = singleton;
            }

            return singleton;
        }
    }
}