using ScriptableArchitecture.Core;
using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ScriptableArchitecture.EditorScript
{
    [CustomPropertyDrawer(typeof(Variable<>), true)]
    public class VariableDrawer : PropertyDrawer
    {
        private bool _foldoutOpen;
        private float _height = 18f;

        private ReorderableList _runtimeSetList;
        private ReorderableList _startRuntimeSetList;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (position.width == 1f && position.height == 1f) return;

            EditorGUI.BeginProperty(position, label, property);

            _height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if (property.boxedValue != null)
            {
                Rect variableRect = new Rect(EditorGUIUtility.labelWidth + position.x / 2f + 10f, position.y, position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);

                EditorGUI.PropertyField(variableRect, property, GUIContent.none);

                Rect foldoutRect = new Rect(position.x + 15f, position.y, 15f, EditorGUIUtility.singleLineHeight);

                //Draw foldout
                _foldoutOpen = EditorGUI.Foldout(foldoutRect, _foldoutOpen, label);
                if (_foldoutOpen && property.objectReferenceValue != null)
                {
                    Rect valueRect = new Rect(position.x / 2f + 10f, position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing + 1f, EditorGUIUtility.currentViewWidth - 22f, EditorGUIUtility.singleLineHeight);
                    EditorGUI.indentLevel++;
                    bool inPlaymode = EditorApplication.isPlaying;

                    var valueVariable = property.objectReferenceValue as Variable;
                    SerializedObject serializedObject = new SerializedObject(valueVariable);

                    //Types
                    SerializedProperty _variableTypeProperty = serializedObject.FindProperty("VariableType");
                    SerializedProperty _initializeTypeVariableProperty = serializedObject.FindProperty("InitializeTypeVariable");
                    SerializedProperty _initializeTypeRuntimeSetProperty = serializedObject.FindProperty("InitializeTypeRuntimeSet");

                    //Variable
                    SerializedProperty valueProperty = serializedObject.FindProperty("_value");
                    SerializedProperty startValueProperty = serializedObject.FindProperty("_startValue");

                    //RuntimeSet
                    SerializedProperty runtimeSetProperty = serializedObject.FindProperty("_runtimeSet");
                    SerializedProperty startRuntimeSetProperty = serializedObject.FindProperty("_startRuntimeSet");

                    VariableType variableType = (VariableType)_variableTypeProperty.enumValueIndex;
                    InitializeType initializeVariableType = (InitializeType)_initializeTypeVariableProperty.enumValueIndex;
                    InitializeType initializeRuntimeSetType = (InitializeType)_initializeTypeRuntimeSetProperty.enumValueIndex;

                    EditorGUI.BeginChangeCheck();

                    if (variableType == VariableType.Variable || variableType == VariableType.VariableEvent)
                    {
                        if (inPlaymode && initializeVariableType != InitializeType.ReadOnly)
                        {
                            EditorGUI.PropertyField(valueRect, valueProperty, true);
                            AddPropertyHeight(valueProperty, ref valueRect);
                        }

                        if (initializeVariableType == InitializeType.ResetOnGameStart || initializeVariableType == InitializeType.ReadOnly)
                        {
                            EditorGUI.PropertyField(valueRect, startValueProperty, true);
                            AddPropertyHeight(startValueProperty, ref valueRect);
                        }
                    }
                    
                    if (variableType == VariableType.RuntimeSet)
                    {
                        if (inPlaymode && initializeRuntimeSetType != InitializeType.ReadOnly)
                        {
                            DrawReorderableList(ref _runtimeSetList, valueRect, runtimeSetProperty);
                            AddPropertyHeight(runtimeSetProperty, ref valueRect, 18);
                        }

                        if (initializeRuntimeSetType == InitializeType.ResetOnGameStart || initializeRuntimeSetType == InitializeType.ReadOnly)
                        {
                            DrawReorderableList(ref _startRuntimeSetList, valueRect, startRuntimeSetProperty);
                            AddPropertyHeight(startRuntimeSetProperty, ref valueRect, 18);
                        }
                    }

                    EditorGUI.PropertyField(valueRect, _variableTypeProperty);

                    if (variableType == VariableType.Variable || variableType == VariableType.VariableEvent)
                    {
                        valueRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        _height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                        EditorGUI.PropertyField(valueRect, _initializeTypeVariableProperty);
                    }

                    if (variableType == VariableType.RuntimeSet)
                    {
                        valueRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        _height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                        EditorGUI.PropertyField(valueRect, _initializeTypeRuntimeSetProperty);
                    }

                    _height++;

                    if (EditorGUI.EndChangeCheck())
                        serializedObject.ApplyModifiedProperties();
                        
                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
                
                Rect valueRect = new Rect(position.x, position.y, position.width - 52f, position.height);
                EditorGUI.PropertyField(valueRect, property, GUIContent.none);

                Rect buttonRect = new Rect(position.x + position.width - 50f, position.y, 50f, position.height);
                if (GUI.Button(buttonRect, "New", EditorStyles.miniButton))
                {
                    Type newType = GetVariableType(property.type, out string variableTypeName);
                    Variable newVariable = ScriptableObject.CreateInstance(newType) as Variable;

                    string typeName = label.text.ToLower();

                    if (typeName.Contains("event"))
                        newVariable.VariableType = VariableType.Event;
                    else if (typeName.Contains("runtimeset") || typeName.Contains("list") || typeName.Contains("collection"))
                        newVariable.VariableType = VariableType.RuntimeSet;

                    string path = EditorUtility.SaveFilePanel($"Create new {variableTypeName}", "Assets/Data", property.propertyPath.RemoveUnderscore().CapitalizeFirstLetter().RemoveAfterDot(), "asset");

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

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float baseHeight = base.GetPropertyHeight(property, label);

            if (_foldoutOpen)
                return baseHeight + _height;

            return baseHeight;
        }

        private void AddPropertyHeight(SerializedProperty property, ref Rect valueRect, float extraHeight = 0)
        {
            float height = EditorGUI.GetPropertyHeight(property) + EditorGUIUtility.standardVerticalSpacing + 1 + extraHeight;
            _height += height;
            valueRect.y += height;
        }

        //Workaround to edit nested lists in the inspector

        private void DrawReorderableList(ref ReorderableList reorderableList, Rect rect, SerializedProperty list)
        {
            if (reorderableList == null)
                reorderableList = new ReorderableList(list.serializedObject, list, false, true, true, true);

            reorderableList.drawHeaderCallback = (headerRect) =>
            {
                EditorGUI.LabelField(headerRect, list.displayName);
            };

            reorderableList.drawElementCallback = (elementRect, index, active, focused) =>
            {
                SerializedProperty element = list.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(elementRect, element, true);
            };

            reorderableList.elementHeightCallback = (index) =>
            {
                SerializedProperty element = list.GetArrayElementAtIndex(index);
                return EditorGUI.GetPropertyHeight(element) + EditorGUIUtility.standardVerticalSpacing;
            };

            reorderableList.onAddCallback = (list) =>
            {
                list.serializedProperty.serializedObject.Update();
                list.serializedProperty.arraySize++;
                list.index = list.serializedProperty.arraySize - 1;
                list.serializedProperty.serializedObject.ApplyModifiedProperties();
            };

            reorderableList.onRemoveCallback = (list) =>
            {
                list.serializedProperty.serializedObject.Update();
                list.serializedProperty.DeleteArrayElementAtIndex(list.index);
                list.serializedProperty.serializedObject.ApplyModifiedProperties();
            };

            //Bug can't reorder - snaps back to old value - don't allow reordering
            reorderableList.onReorderCallback = (list) =>
            {
                list.serializedProperty.serializedObject.Update();
                list.serializedProperty.serializedObject.ApplyModifiedProperties();
            };

            reorderableList.DoList(rect);
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