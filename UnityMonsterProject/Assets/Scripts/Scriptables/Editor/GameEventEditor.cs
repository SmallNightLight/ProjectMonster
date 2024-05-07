using UnityEditor;
using UnityEngine;
using ScriptableArchitecture.Core;

namespace ScriptableArchitecture.EditorScript
{
    [CustomEditor(typeof(GameEventBase), true)]
    public class GameEventEditor : Editor
    {
        private GameEventBase _target;
        private bool _showDebugEvent;
        private bool _showListeners;
        private bool _showStackTrace;
        private Vector2 _scrollStacktrace;
       
        private void OnEnable()
        {
            _target = (GameEventBase)target;
        }

        public override void OnInspectorGUI()
        {
            //Draw debug options
            EditorGUILayout.Space();
            serializedObject.Update();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;

            _showDebugEvent = EditorGUILayout.Foldout(_showDebugEvent, "Debug Event", true, new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold });

            if (_showDebugEvent)
            {
                serializedObject.ApplyModifiedProperties();
                EditorGUILayout.Space();

                if (GUILayout.Button("Raise Event"))
                    _target.Raise();
            }

            EditorGUILayout.EndVertical();

            GameEventEditorHelper.DrawListeners(target as IGameEvent, ref _showListeners);
            GameEventEditorHelper.DrawStackTrace(_target.GetStackTraces()[0], ref _showStackTrace, ref _scrollStacktrace);
        }
    }
}