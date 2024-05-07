using ScriptableArchitecture.Core;
using UnityEditor;
using UnityEngine;

namespace ScriptableArchitecture.EditorScript
{
    [CustomPropertyDrawer(typeof(Emitter), true)]
    public class EmitterDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.LabelField(position, label.text, property.displayName);
            EditorGUI.PropertyField(position, property);
        }
    }
}