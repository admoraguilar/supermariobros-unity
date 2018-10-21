using UnityEngine;
using UnityEditor;


namespace WishfulDroplet {
    namespace Editor {
        [CustomPropertyDrawer(typeof(InspectorNoteAttribute))]
        public class InspectorNotePropertyDrawer : DecoratorDrawer {
            public override float GetHeight() {
                EditorResources editorResource = EditorSingleton.Get<EditorResources>();
                GUISkin inspectorNoteStyle = editorResource.inspectorNoteSkin;
                InspectorNoteAttribute noteAttribute = attribute as InspectorNoteAttribute;

                float headerHeight = inspectorNoteStyle.customStyles[0].CalcHeight(new GUIContent(noteAttribute.header), EditorGUIUtility.currentViewWidth - 33f) +
                                     inspectorNoteStyle.customStyles[0].fontSize;
                float messageHeight = inspectorNoteStyle.customStyles[1].CalcHeight(new GUIContent(noteAttribute.message), EditorGUIUtility.currentViewWidth - 33f) +
                                     inspectorNoteStyle.customStyles[1].fontSize;

                return ((string.IsNullOrEmpty(noteAttribute.message) ? 0 : 0 + messageHeight) + headerHeight);
            }

            public override void OnGUI(Rect position) {
                EditorResources editorResource = EditorSingleton.Get<EditorResources>();
                GUISkin inspectorNoteStyle = editorResource.inspectorNoteSkin;
                InspectorNoteAttribute noteAttribute = attribute as InspectorNoteAttribute;

                // Background box
                Rect backgroundBoxPos = position;
                backgroundBoxPos.height -= 5f;
                backgroundBoxPos.y += 5f;
                GUI.Box(backgroundBoxPos, "", inspectorNoteStyle.customStyles[2]);

                float headerHeight = inspectorNoteStyle.customStyles[0].CalcHeight(new GUIContent(noteAttribute.header), position.width);
                //float messageHeight = inspectorNoteStyle.customStyles[1].CalcHeight(new GUIContent(note.Message), position.width); // not used

                // our header is always present
                Rect posLabel = position;
                posLabel.y += 13;
                posLabel.x += 5f;
                posLabel.height += 13;
                EditorGUI.LabelField(posLabel, noteAttribute.header, inspectorNoteStyle.customStyles[0]);

                // do we have a message too?
                if(!string.IsNullOrEmpty(noteAttribute.message)) {
                    //Color color = GUI.color;
                    //Color faded = color;
                    //faded.a = 0.5f;

                    Rect posExplain = posLabel;
                    posExplain.y += headerHeight;
                    posExplain.width -= 13f;

                    //GUI.color = faded;
                    EditorGUI.LabelField(posExplain, noteAttribute.message, inspectorNoteStyle.customStyles[1]);
                    //GUI.color = color;
                }
     
                //Rect posLine = position;
                //posLine.y += (string.IsNullOrEmpty(note.message) ? 17 : 17 + messageHeight) + headerHeight;
                //posLine.x += 0;
                //posLine.height = 2;
                //GUI.Box(posLine, "");

                // Draw the field
                base.OnGUI(position);
            }
        }
    }
}
