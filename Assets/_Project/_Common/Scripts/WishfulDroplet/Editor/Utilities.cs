using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;


namespace WishfulDroplet.Editor {
    public static class ShortcutItems {
        [MenuItem("Tools/WishfulDropet/Settings/Editor Resources")]
        public static void ShowEditorResources() {
            EditorResources editorResources = EditorSingleton.Get<EditorResources>();
            Selection.activeObject = editorResources;
        }
    }

    public static class Utilities {
        public static T CreateScriptableObjectAsset<T>(string path) where T : ScriptableObject {
            T so = ScriptableObject.CreateInstance<T>();

            AssetDatabase.CreateAsset(so, string.Format("{0}/{1}.asset", path, typeof(T).Name));
            AssetDatabase.SaveAssets();

            Selection.activeObject = so;

            return so;
        }

        public static string GetFolderPathWithName(string folderName, bool isRelative = true) {
            string path = "";
            string[] fullPaths = Directory.GetDirectories(string.Format(Application.dataPath), string.Format("*{0}", folderName), SearchOption.AllDirectories);

            if(fullPaths.Length > 0) {
                string fullPath = fullPaths.Length > 0 ? fullPaths[0] : "";
                if(isRelative) path = MakePathRelative(fullPath);
            }

            return path;
        }

        public static string GetOrCreateFolderPath(string folderPath, bool isRelative = true) {
            string path = "";

            path = Directory.CreateDirectory(folderPath).FullName;
            if(isRelative) path = MakePathRelative(path);

            return path;
        }

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

        public static string GetProjectWindowSelectedPath(string fallbackPath = "Assets") {
            string path = fallbackPath;
                
            foreach(var obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets)) {
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

