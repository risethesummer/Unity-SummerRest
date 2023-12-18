using System;
using System.Linq;
using SummerRest.Attributes;
using SummerRest.Editors.Utilities;
using UnityEditor;
using UnityEngine;

namespace SummerRest.Editors.Drawers
{
    [CustomPropertyDrawer(typeof(DefaultsAttribute))]
    internal class DefaultsDrawer : PropertyDrawer
    {
        private string[] _defaultValues;
        private GUILayoutOption _labelOption;
        private GUILayoutOption _customOption;
        private void Init(SerializedProperty property, GUIContent label)
        {
            if (_defaultValues is null)
            {
                _defaultValues = ((DefaultsAttribute)attribute).Defaults.Prepend("Custom").ToArray();
                _labelOption = label.Width();
                _customOption = _defaultValues[0].Width();
            }
        }
        public int GetIdx(SerializedProperty property)
        {
            return Array.IndexOf(_defaultValues, property.stringValue);
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Init(property, label);
            EditorGUI.BeginProperty(position, label, property);
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUILayout.HelpBox(
                    $"Can not use {nameof(DefaultsAttribute)} with others types but string",
                    MessageType.Error);
                return;
            }
            var oldIdx = Mathf.Max(0, GetIdx(property));
            var oldVal = property.stringValue;
            using var scope = EditorCustomUtilities.DoHorizontalLayout(
                EditorCustomUtilities.LayoutOptions.ExpandWidth());
            EditorGUILayout.LabelField(label, _labelOption);
            var selectIdx = EditorGUILayout.Popup(oldIdx, _defaultValues, 
                oldIdx != 0 ? EditorCustomUtilities.LayoutOptions.ExpandWidth() : _customOption);
            if (oldIdx != selectIdx)
                property.stringValue = _defaultValues[selectIdx];
            if (selectIdx == 0)
            {                
                //property.stringValue = "";
                EditorGUILayout.PropertyField(property, GUIContent.none, EditorCustomUtilities.LayoutOptions.ExpandWidth());
            }
            EditorGUI.EndProperty();
        }
    }
}