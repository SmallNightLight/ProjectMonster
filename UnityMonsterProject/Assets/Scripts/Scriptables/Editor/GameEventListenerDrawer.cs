using ScriptableArchitecture.Core;
using UnityEditor;
using UnityEngine;

namespace ScriptableArchitecture.EditorScript
{
    [CustomEditor(typeof(GameEventListenerBase), true)]
    public class GameEventListenerDrawer : Editor
    {
        private SerializedProperty _eventProeperty;
        private SerializedProperty _responseEvent;

        private void OnEnable()
        {
            _eventProeperty = serializedObject.FindProperty("_event");
            _responseEvent = serializedObject.FindProperty("_response");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_eventProeperty);
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(_responseEvent);

            serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomEditor(typeof(GameEventListenerBase<>), true)]
    public class GameEventListenerDrawerT : Editor
    {
        private SerializedProperty _eventProeperty;
        private SerializedProperty _responseEvent;

        private void OnEnable()
        {
            _eventProeperty = serializedObject.FindProperty("_event");
            _responseEvent = serializedObject.FindProperty("_response");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_eventProeperty);
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(_responseEvent);

            serializedObject.ApplyModifiedProperties();
        }
    }
}