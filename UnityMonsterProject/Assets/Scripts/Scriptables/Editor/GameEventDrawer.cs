using ScriptableArchitecture.Core;
using System;
using UnityEditor;
using UnityEngine;

namespace ScriptableArchitecture.EditorScript
{
    [CustomPropertyDrawer(typeof(GameEventBase), true)]
    public class GameEventDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            if (property.boxedValue == null)
            {
                position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
                
                Rect valueRect = new Rect(position.x, position.y, position.width - 52f, position.height);
                EditorGUI.PropertyField(valueRect, property, GUIContent.none);

                Rect buttonRect = new Rect(position.x + position.width - 50f, position.y, 50f, position.height);
                if (GUI.Button(buttonRect, "New", EditorStyles.miniButton))
                {
                    Type newType = GetVariableType(property.type, out string variableTypeName);
                    GameEventBase newVariable = ScriptableObject.CreateInstance(newType) as GameEventBase;

                    string path = EditorUtility.SaveFilePanel($"Create new Event", "Assets/Data", "Event", "asset");

                    if (!string.IsNullOrEmpty(path))
                    {
                        path = "Assets" + path.Substring(Application.dataPath.Length);

                        AssetDatabase.CreateAsset(newVariable, path);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        property.objectReferenceValue = newVariable;
                        property.serializedObject.ApplyModifiedProperties();
                    }

                    GUIUtility.ExitGUI();
                }
            }
            else
            {
                EditorGUI.PropertyField(position, property);
            }
        }

        private Type GetVariableType(string name, out string variableTypeName)
        {
            int start = name.IndexOf("<") + 2;
            int end = name.LastIndexOf(">");

            variableTypeName = name.Substring(start, end - start);
            return Type.GetType($"ScriptableArchitecture.Data.{variableTypeName}, ScriptableAssembly.Data");
        }
    }
}