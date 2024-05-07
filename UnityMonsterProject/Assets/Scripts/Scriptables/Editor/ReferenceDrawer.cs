using ScriptableArchitecture.Core;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ScriptableArchitecture.EditorScript
{
    [CustomPropertyDrawer(typeof(Reference<,>), true)]
    public class ReferenceDrawer : PropertyDrawer
    {
        private bool _isVariable;

        private void OnGUIMain(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty isVariableProperty = property.FindPropertyRelative("_isVariable");
            SerializedProperty variableProperty = property.FindPropertyRelative("_variable");
            SerializedProperty constantProperty = property.FindPropertyRelative("_constant");

            _isVariable = isVariableProperty.boolValue;

            if (_isVariable)
            {
                if (variableProperty.boxedValue != null)
                {
                    Rect variableRect = new Rect(position.x - 15, position.y, position.width - 22f, EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(variableRect, variableProperty, label);
                }
                else
                {
                    position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
                    Rect valueRect = new Rect(position.x, position.y, position.width - 20f, position.height);
                    EditorGUI.PropertyField(valueRect, variableProperty, GUIContent.none);
                }
            }
            else
            {
                Rect valueRect = new Rect(0, 0, position.width - 20f, position.height);
                EditorGUI.PropertyField(valueRect, constantProperty, new GUIContent(property.displayName), true);
            }

            Rect buttonRect = new Rect(position.x + position.width - 18f, position.y, 18f, position.height);

            //Display a button to change the reference type
            if (GUI.Button(buttonRect, "..", EditorStyles.miniButton))
            {
                //Display a popup menu
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Constant"), !_isVariable, () =>
                {
                    isVariableProperty.boolValue = false;
                    property.serializedObject.ApplyModifiedProperties();
                });

                menu.AddItem(new GUIContent("Variable"), _isVariable, () =>
                {
                    isVariableProperty.boolValue = true;
                    property.serializedObject.ApplyModifiedProperties();
                });

                menu.ShowAsContext();
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty isVariableProperty = property.FindPropertyRelative("_isVariable");

            if (isVariableProperty.boolValue)
                return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("_variable"), true);
            else
                return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("_constant"), true);
        }


        private SerializedObject _propertyObject;
        private FieldInfo _nativeObject = typeof(SerializedObject).GetField("m_NativeObjectPtr", BindingFlags.NonPublic | BindingFlags.Instance);
       
        const float _targetFramerate = 10f;
        static double _timer = 0;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!ActiveEditorTracker.HasCustomEditor(property.serializedObject.targetObject))
            {
                _propertyObject = property.serializedObject;
                EditorApplication.update -= Repaint;
                EditorApplication.update += Repaint;
            }

            Selection.selectionChanged -= OnSelectionChange;
            Selection.selectionChanged += OnSelectionChange;

            OnGUIMain(position, property, label);
        }

        private void Repaint()
        {
            double timeSinceStartup = EditorApplication.timeSinceStartup;

            if (timeSinceStartup > _timer + 1 / _targetFramerate)
            {
                _timer = timeSinceStartup;

                foreach (var editor in ActiveEditorTracker.sharedTracker.activeEditors)
                    editor.Repaint();
            }
        }

        private void OnSelectionChange()
        {
            if (_propertyObject == null || (IntPtr)_nativeObject.GetValue(_propertyObject) == IntPtr.Zero)
            {
                EditorApplication.update -= Repaint;
                Selection.selectionChanged -= OnSelectionChange;
            }
        }
    }
}