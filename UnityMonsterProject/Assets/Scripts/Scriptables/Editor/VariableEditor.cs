using UnityEngine;
using UnityEditor;
using ScriptableArchitecture.Core;
using System.Reflection;
using System;
using System.Linq;

namespace ScriptableArchitecture.EditorScript
{
    [CustomEditor(typeof(Variable<>), true)]
    [CanEditMultipleObjects]
    public class VariableEditor : Editor
    {
        private Variable _target;

        private bool _canShowVariable;
        private bool _canShowGameEvent;
        private bool _canShowRuntimeSet;

        private bool _showVariable;
        private bool _showGameEvent;
        private bool _showRuntimeSet;
        private bool _showStacktrace;

        //Variable
        private SerializedProperty _initializeTypeVariableProperty;
        private SerializedProperty _valueProperty;
        private SerializedProperty _startValueProperty;

        //Event
        private bool _showDebugEvent;
        private bool _showListeners;
        
        private SerializedProperty _debugValueProperty;
        private SerializedProperty _variableTypeProperty;
        private MethodInfo _raiseMethod;

        //RuntimeSet
        private SerializedProperty _initializeTypeRuntimeSetProperty;
        private SerializedProperty _runtimeSetProperty;
        private SerializedProperty _startRuntimeSetProperty;


        //Stacktrace
        private bool[] _showSubStackTraces = new bool[3];
        private Vector2[] _scrollStacktraces = new Vector2[3];

        private bool _inPlayMode;

        private void OnEnable()
        {
            _target = target as Variable;

            //Types
            _variableTypeProperty = serializedObject.FindProperty("VariableType");
            _initializeTypeVariableProperty = serializedObject.FindProperty("InitializeTypeVariable");
            _initializeTypeRuntimeSetProperty = serializedObject.FindProperty("InitializeTypeRuntimeSet");

            //Variable
            _valueProperty = serializedObject.FindProperty("_value");
            _startValueProperty = serializedObject.FindProperty("_startValue");

            //Event
            _debugValueProperty = serializedObject.FindProperty("DebugValue");
            _raiseMethod = target.GetType().BaseType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public).Where(m => m.GetParameters().Length == 1).ToArray().FirstOrDefault();

            //RuntimeSet
            _runtimeSetProperty = serializedObject.FindProperty("_runtimeSet");
            _startRuntimeSetProperty = serializedObject.FindProperty("_startRuntimeSet");

            ResetFoldouts();
        }

        public override void OnInspectorGUI()
        {
            if (target == null) return;

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            _inPlayMode = EditorApplication.isPlaying;

            GUI.enabled = !_inPlayMode;
            EditorGUILayout.PropertyField(_variableTypeProperty);
            GUI.enabled = true;

            if (EditorGUI.EndChangeCheck())
                ResetFoldouts();

            if (GUI.changed)
                EditorUtility.SetDirty(_target);

            serializedObject.ApplyModifiedProperties();

            if (_canShowVariable)
            {
                EditorGUILayout.Space(10);
                DrawVariableEditor();
            }
                
            if (_canShowGameEvent)
            {
                EditorGUILayout.Space(10);
                DrawGameEventEditor();
            }
            
            if (_canShowRuntimeSet)
            {
                EditorGUILayout.Space(10);
                DrawRuntimeSetEditor();
            }
                
            EditorGUILayout.Space(10);
            DrawStacktrace();
        }

        private void DrawVariableEditor()
        {
            _showVariable = EditorGUILayout.Foldout(_showVariable, "Variable", true, new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold });

            if (_showVariable)
            {
                EditorGUI.indentLevel++;

                InitializeType initializeType = (InitializeType)_initializeTypeVariableProperty.enumValueIndex;

                EditorGUI.BeginChangeCheck();

                EditorGUILayout.PropertyField(_initializeTypeVariableProperty);

                if (_inPlayMode && initializeType != InitializeType.ReadOnly)
                    EditorGUILayout.PropertyField(_valueProperty, true);

                if (initializeType == InitializeType.ResetOnGameStart || initializeType == InitializeType.ReadOnly)
                    EditorGUILayout.PropertyField(_startValueProperty, true);

                serializedObject.ApplyModifiedProperties();

                EditorGUI.indentLevel--;
            }
        }

        private void DrawGameEventEditor()
        {
            _showGameEvent = EditorGUILayout.Foldout(_showGameEvent, "Game Event", true, new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold });

            if (_showGameEvent)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUI.indentLevel++;

                _showDebugEvent = EditorGUILayout.Foldout(_showDebugEvent, "Debug Event", true, new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold });

                if (_showDebugEvent)
                {
                    EditorGUILayout.PropertyField(_debugValueProperty);

                    serializedObject.ApplyModifiedProperties();

                    if (GUILayout.Button("Raise Event"))
                    {
                        Type targetType = _debugValueProperty.serializedObject.targetObject.GetType();
                        FieldInfo targetField = targetType.GetField("DebugValue", BindingFlags.Instance | BindingFlags.Public);
                        object debugValue = targetField.GetValue(_debugValueProperty.serializedObject.targetObject);
                        _raiseMethod.Invoke(target, new object[1] { debugValue });
                    }
                }

                EditorGUILayout.EndVertical();

                GameEventEditorHelper.DrawListeners(target as IGameEvent, ref _showListeners);
                EditorGUI.indentLevel--;
            }
        }

        private void DrawRuntimeSetEditor()
        {
            _showRuntimeSet = EditorGUILayout.Foldout(_showRuntimeSet, "RuntimeSet", true, new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold });
            
            if (_showRuntimeSet)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(_initializeTypeRuntimeSetProperty);
                EditorGUI.indentLevel++;

                InitializeType initializeType = (InitializeType)_initializeTypeRuntimeSetProperty.enumValueIndex;

                if (_inPlayMode && initializeType != InitializeType.ReadOnly)
                    EditorGUILayout.PropertyField(_runtimeSetProperty, true);

                if (initializeType == InitializeType.ResetOnGameStart || initializeType == InitializeType.ReadOnly)
                    EditorGUILayout.PropertyField(_startRuntimeSetProperty, true);
                
                EditorGUI.indentLevel--;

                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
            }
        }

        private void DrawStacktrace()
        {
            _showStacktrace = EditorGUILayout.Foldout(_showStacktrace, "Stacktrace", true, new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold });

            if (_showStacktrace)
            {
                EditorGUI.indentLevel++;

                Stacktrace[] stacktraces = (target as IGameEvent).GetStackTraces();
                
                for (int i = 0; i < stacktraces.Length; i++)
                {
                    Stacktrace stacktrace = stacktraces[i];

                    VariableType type = stacktrace.GetStackType();

                    bool drawAsVariable = _canShowVariable && (type == VariableType.Variable || type == VariableType.VariableEvent);
                    bool drawAsEvent = _canShowGameEvent && (type == VariableType.Event || type == VariableType.VariableEvent);
                    bool drawAsRuntimeSet = _canShowRuntimeSet && (type == VariableType.RuntimeSet);

                    if (drawAsVariable || drawAsEvent || drawAsRuntimeSet)
                        GameEventEditorHelper.DrawStackTrace(stacktrace, ref _showSubStackTraces[i], ref _scrollStacktraces[i]);
                }

                EditorGUI.indentLevel--;
            }
        }

        private void ResetFoldouts()
        {
            VariableType variableType = (VariableType)_variableTypeProperty.enumValueIndex;

            _canShowVariable = variableType == VariableType.Variable || variableType == VariableType.VariableEvent;
            _canShowGameEvent = variableType == VariableType.Event || variableType == VariableType.VariableEvent;
            _canShowRuntimeSet = variableType == VariableType.RuntimeSet;

            _showVariable = _canShowVariable;
            _showGameEvent = _canShowGameEvent;
            _showRuntimeSet = _canShowRuntimeSet;
        }
    }
}