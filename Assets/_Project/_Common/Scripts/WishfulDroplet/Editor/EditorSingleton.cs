using UnityEngine;
using UnityEditor;


namespace WishfulDroplet.Editor {
	/// <summary>
	/// A Singleton manager for the editor.
	/// </summary>
    public static class EditorSingleton {
		/// <summary>
		/// Gets or creates a singleton and returns it.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
        public static T Get<T>() where T : ScriptableObject {
            T singleton = default(T);

			// We find if there's an asset of type "T"
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T).Name));
            foreach(var guid in guids) {
                singleton = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
                if(singleton) break;
            }

			// If there's no existing singleton then we create one
            if(!singleton) {
                string path = Utilities.GetOrCreateFolderPath(string.Format("{0}/{1}", Utilities.GetFolderPathWithName("WishfulDroplet"), "Editor/Resources"));
                singleton = Utilities.CreateScriptableObjectAsset<T>(path);
                Selection.activeObject = singleton;
            }

            return singleton;
        }
    }
}