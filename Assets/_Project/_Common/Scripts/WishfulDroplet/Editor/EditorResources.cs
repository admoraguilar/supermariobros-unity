using UnityEngine;
using UnityEditor;


namespace WishfulDroplet.Editor {
    [CreateAssetMenu(menuName = "WishfulDroplet/Editor/EditorResources")]
    public class EditorResources : ScriptableObject {
		[MenuItem("Tools/WishfulDropet/Settings/Editor Resources")]
		public static void ShowEditorResources() {
			EditorResources editorResources = EditorSingleton.Get<EditorResources>();
			Selection.activeObject = editorResources;
		}

		public GUISkin inspectorNoteSkin;
    }
}
