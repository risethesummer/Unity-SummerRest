﻿using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Utilities
{
    public static class UIToolkitExtensions
    {
        public static void ReplaceBackgroundColor(this IStyle style, Color color)
        {
            var oldStyle = style.backgroundColor;
            oldStyle.value = color;
            style.backgroundColor = oldStyle;
        }
        
        public static string EnumName(this SerializedProperty property)
        {
            if (property.propertyType == SerializedPropertyType.Enum)
                return property.enumNames[property.enumValueIndex];
            return string.Empty;
        }

        
        public static SerializedProperty FindBackingPropertyRelative(this SerializedObject serializedObject, string name)
        {
            return serializedObject.FindProperty($"<{name}>k__BackingField");
        }
        public static object GetReference(this SerializedProperty serializedObject)
        {
            switch (serializedObject.propertyType)
            {
                case SerializedPropertyType.ObjectReference:
                    return serializedObject.objectReferenceValue;
                case SerializedPropertyType.ManagedReference:
                    return serializedObject.managedReferenceValue;
            }
            return null;
        }
        public static SerializedProperty FindBackingPropertyRelative(this SerializedProperty serializedObject, string name)
        {
            return serializedObject.FindPropertyRelative($"<{name}>k__BackingField");
        }
        public static NativeArray<byte> GetByteArray(this SerializedProperty serializedProperty)
        {
            var nativeBytes = new NativeArray<byte>(serializedProperty.arraySize, Allocator.Temp);
            for (var i = 0; i < serializedProperty.arraySize; i++)
                nativeBytes[i] = (byte)serializedProperty.GetArrayElementAtIndex(i).intValue;
            return nativeBytes;
        }
        public static List<string> GetStringArray(this SerializedProperty serializedProperty, Func<SerializedProperty, string> getValue = null)
        {
            var result = new List<string>(serializedProperty.arraySize);
            for (var i = 0; i < serializedProperty.arraySize; i++)
            {
                var element = serializedProperty.GetArrayElementAtIndex(i);
                var val = getValue is null ? element.stringValue : getValue.Invoke(element);
                result.Add(val);
            }
            return result;
        }

        public static void ShowObjectFields(this VisualElement container, 
            SerializedObject serializedObj)
        {
            if (container.childCount == 0)
            {
                InspectorElement.FillDefaultInspector(container, serializedObj, null);
                // Remove script label
                container.Q<PropertyField>().RemoveFromHierarchy();
            }
            
            container.Bind(serializedObj);
        }
        public static void Show(this IStyle style, bool show)
        {
            style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
        }
        public static void Show(this VisualElement visualElement, bool show)
        {
            visualElement.style.Show(show);
        }
        public static void SetTextValueWithoutNotify<TField>(this TField field, string val) 
            where TField : INotifyValueChanged<string>
        {
            field.SetValueWithoutNotify(val);
        }
 
        
        public static void CallThenTrackPropertyValue(
            this VisualElement element,
            SerializedProperty property,
            Action<SerializedProperty> callback)
        {
            callback?.Invoke(property);
            element.Unbind();
            element.TrackPropertyValue(property, callback);
        }
        public static void CallThenTrackPropertyValue(
            this BindableElement element,
            SerializedObject obj,
            Action<SerializedProperty> callback)
        {
            var property = obj.FindProperty(element.bindingPath);
            callback?.Invoke(property);
            element.Unbind();
            element.TrackPropertyValue(property, callback);
        }
    }
}