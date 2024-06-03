using ScriptableArchitecture.Core;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Instancer<>), true)]
public class InstancerDrawer : PropertyDrawer
{
    private bool _foldoutOpen;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect variableRect = new Rect(EditorGUIUtility.labelWidth + position.x / 2f + 10f, position.y, position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(variableRect, property, GUIContent.none);

        Rect foldoutRect = new Rect(position.x + 15f, position.y, 15f, EditorGUIUtility.singleLineHeight);

        _foldoutOpen = EditorGUI.Foldout(foldoutRect, _foldoutOpen, label);
        if (_foldoutOpen && property.objectReferenceValue != null)
        {
            Rect valueRect = new Rect(position.x / 2f + 10f, position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing + 1f, EditorGUIUtility.currentViewWidth - 22f, EditorGUIUtility.singleLineHeight);
            EditorGUI.indentLevel++;

            var valueVariable = property.objectReferenceValue as Instancer;
            SerializedObject serializedObject = new SerializedObject(valueVariable);

            //Types
            SerializedProperty instanceScopeProperty = serializedObject.FindProperty("_instanceScope");
            SerializedProperty baseVariableProperty = serializedObject.FindProperty("_baseVariable");
            SerializedProperty instancedVariableProperty = serializedObject.FindProperty("_instancedVariable");

            EditorGUI.BeginChangeCheck();

            EditorGUI.PropertyField(valueRect, baseVariableProperty, true);
            valueRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing + 1f;
            EditorGUI.PropertyField(valueRect, instanceScopeProperty, true);


            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            EditorGUI.indentLevel--;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float baseHeight = base.GetPropertyHeight(property, label);

        if (_foldoutOpen)
            return baseHeight + 3 * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing + 1f);

        return baseHeight;
    }
}