using UnityEngine;
using ScriptableArchitecture.Core;
using UnityEditor;

namespace ScriptableArchitecture.EditorScript
{
    [CustomEditor(typeof(Receiver), true)]
    public class ReceiverEditor : Editor
    {
        private SerializedProperty _emittersProperty;
        private SerializedProperty _activeEmitterProperty;

        private void OnEnable()
        {
            _emittersProperty = serializedObject.FindProperty("_emitters");
            _activeEmitterProperty = serializedObject.FindProperty("_activeEmitter");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            GUILayout.Label("Active Emitters");
            EditorGUI.indentLevel++;

            for (int i = 0; i < _emittersProperty.arraySize; i++)
            {
                SerializedProperty emitterProperty = _emittersProperty.GetArrayElementAtIndex(i);
                Emitter emitter = emitterProperty.objectReferenceValue as Emitter;

                if (emitter != null)
                {
                    GUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField(emitter.name, GUILayout.Width(150));

                    int priority = new SerializedObject(emitter).FindProperty("_priority").intValue;

                    GUILayout.Label(priority.ToString());
                    GUILayout.EndHorizontal();
                }
            }

            EditorGUI.indentLevel--;

            GUILayout.Space(15);
            EditorGUILayout.LabelField("Current Emitter", GUILayout.Width(150));

            if (_activeEmitterProperty.objectReferenceValue)
            {
                EditorGUI.indentLevel++;
                GUILayout.BeginHorizontal();

                float indent = EditorGUI.indentLevel * 15;
                GUILayout.Space(indent);

                int priority = new SerializedObject(_activeEmitterProperty.objectReferenceValue).FindProperty("_priority").intValue;

                GUILayout.Label((_activeEmitterProperty.objectReferenceValue as Emitter).name, GUILayout.Width(150 - indent));
                GUILayout.Label(priority.ToString());
                
                GUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}