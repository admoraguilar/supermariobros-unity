using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;


namespace WishfulDroplet.Editor {
    public static class Utilities {
		/// <summary>
		/// Makes a scriptable object asset at path. Path should be relative to Unity's folder structure.
		/// </summary>
		/// <typeparam name="T">The type of Scriptable Object you wanna create.</typeparam>
		/// <param name="path">The path to create the Scriptable Object, should be relative Unity's folder structure.</param>
		/// <returns></returns>
		public static T CreateScriptableObjectAsset<T>(string path) where T : ScriptableObject {
            T so = ScriptableObject.CreateInstance<T>();

            AssetDatabase.CreateAsset(so, string.Format("{0}/{1}.asset", path, typeof(T).Name));
            AssetDatabase.SaveAssets();

            Selection.activeObject = so;

            return so;
        }

		/// <summary>
		/// Returns the first folder path given a folder name.
		/// Useful if you want some assets created on a single special folder.
		/// </summary>
		/// <param name="folderName">The name of the folder.</param>
		/// <param name="isRelative">Makes path relative to Unity's folder structure.</param>
		/// <returns></returns>
		public static string GetFolderPathWithName(string folderName, bool isRelative = true) {
            string path = "";
            string[] fullPaths = Directory.GetDirectories(string.Format(Application.dataPath), string.Format("*{0}", folderName), SearchOption.AllDirectories);

            if(fullPaths.Length > 0) {
                string fullPath = fullPaths.Length > 0 ? fullPaths[0] : "";
                if(isRelative) path = MakePathRelative(fullPath);
            }

            return path;
        }

		/// <summary>
		/// Gets or creates a folder path.
		/// </summary>
		/// <param name="folderPath"></param>
		/// <param name="isRelative">Makes path relative to Unity's folder structure.</param>
		/// <returns></returns>
		public static string GetOrCreateFolderPath(string folderPath, bool isRelative = true) {
            string path = "";

            path = Directory.CreateDirectory(folderPath).FullName;
            if(isRelative) path = MakePathRelative(path);

            return path;
        }

		/// <summary>
		/// Makes the path relative to Unity's folder structure.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
        public static string MakePathRelative(string path) {
            string relativePath = "";

            if(!string.IsNullOrEmpty(path)) {
                List<string> splitPath = new List<string>(path.Split('\\', '/'));
                while(splitPath[0] != "Assets") {
                    splitPath.RemoveAt(0);
                }

                for(int i = 0; i < splitPath.Count; i++) {
                    if(i == splitPath.Count - 1) relativePath += splitPath[i];
                    else relativePath += splitPath[i] + "/";
                }
            }

            return relativePath;
        }

		/// <summary>
		/// Gets the selected path in the project window.
		/// </summary>
		/// <param name="fallbackPath">Path used if there's no selected path in project window, should be relative to Unity's folder structure.</param>
		/// <returns></returns>
        public static string GetProjectWindowSelectedPath(string fallbackPath = "Assets") {
            string path = fallbackPath;
                
            foreach(Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets)) {
                path = AssetDatabase.GetAssetPath(obj);
                if(!string.IsNullOrEmpty(path) && File.Exists(path)) {
                    path = Path.GetDirectoryName(path);
                    break;
                }
            }

            return path;
        }
    }
}

