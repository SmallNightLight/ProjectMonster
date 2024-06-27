using ScriptableArchitecture.Core;
using UnityEditor;
using UnityEngine;

namespace ScriptableArchitecture.EditorScript
{
    [CustomPropertyDrawer(typeof(Instancer<>), true)]
    public class InstancerDrawer : PropertyDrawer
    {
        private bool _foldoutOpen;
        private float _height;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.objectReferenceValue != null)
            {
                var valueVariable = property.objectReferenceValue as Instancer;
                SerializedObject serializedObject = new SerializedObject(valueVariable);

                SerializedProperty instanceScopeProperty = serializedObject.FindProperty("_instanceScope");

                EditorGUI.BeginProperty(position, label, property);
                EditorGUI.BeginChangeCheck();

                Rect variableRect = new Rect(EditorGUIUtility.labelWidth + position.x / 2f + 3f, position.y, position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(variableRect, instanceScopeProperty, GUIContent.none);

                Rect foldoutRect = new Rect(position.x, position.y, 15f, EditorGUIUtility.singleLineHeight);

                _height = 0;

                _foldoutOpen = EditorGUI.Foldout(foldoutRect, _foldoutOpen, label);
                if (_foldoutOpen && property.objectReferenceValue != null)
                {
                    EditorGUI.indentLevel++;
                    Rect valueRect = new Rect(position.x - 15, position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, EditorGUIUtility.currentViewWidth - 22f, EditorGUIUtility.singleLineHeight);

                    //Types
                    SerializedProperty baseVariableProperty = serializedObject.FindProperty("_baseVariable");
                    SerializedProperty instancedVariableProperty = serializedObject.FindProperty("_instancedVariable");

                    if (EditorApplication.isPlaying)
                    {
                        EditorGUI.PropertyField(valueRect, instancedVariableProperty, true);
                        _height = EditorGUI.GetPropertyHeight(instancedVariableProperty) + EditorGUIUtility.standardVerticalSpacing;
                    }
                    else
                    {
                        if (baseVariableProperty.objectReferenceValue == null)
                            valueRect.x += 15;

                        EditorGUI.PropertyField(valueRect, baseVariableProperty, true);
                        _height = EditorGUI.GetPropertyHeight(baseVariableProperty) + EditorGUIUtility.standardVerticalSpacing;
                    }

                    Rect propertyRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing + _height, EditorGUIUtility.currentViewWidth - 20, EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(propertyRect, property, true);

                    EditorGUI.indentLevel--;
                    _height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing + 3f;
                }

                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();

                EditorGUI.EndProperty();
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float baseHeight = base.GetPropertyHeight(property, label);

            if (_foldoutOpen)
                return baseHeight + _height;

            return baseHeight;
        }
    }
}