﻿using System;
using UnityEditor;
using UnityEngine;

namespace SummerRest.Editors.Utilities
{
    public static class EditorCustomUtilities
    {
        public readonly struct HorizontalLayoutCommand : IDisposable
        {
            public Rect Rect { get; }

            public HorizontalLayoutCommand(GUIStyle style, GUILayoutOption[] options)
            {
                Rect = EditorGUILayout.BeginHorizontal(style, options);
            }

            public void Dispose()
            {
                EditorGUILayout.EndHorizontal();
            }
        }

        public static class Heights
        {
            public static float SingleLineHeight => EditorGUIUtility.singleLineHeight;
            public static float GetPropertyHeight(
                SerializedProperty property,
                GUIContent label = null,
                bool includeChildren = true)
            {
                return EditorGUI.GetPropertyHeight(property, label, includeChildren);
            }
        }
        

        public static class LayoutOptions
        {
            public static GUILayoutOption Width(float width) => GUILayout.Width(width);
            public static GUILayoutOption MinWidth(float minWidth) => GUILayout.MinWidth(minWidth);
            public static GUILayoutOption MaxWidth(float maxWidth) => GUILayout.MaxWidth(maxWidth);
            public static GUILayoutOption Height(float height) => GUILayout.Height(height);
            public static GUILayoutOption SingleHeight = GUILayout.Height(Heights.SingleLineHeight);
            public static GUILayoutOption MinHeight(float minHeight) => GUILayout.MinHeight(minHeight);
            public static GUILayoutOption MaxHeight(float maxHeight) => GUILayout.MaxHeight(maxHeight);
            public static GUILayoutOption ExpandWidth(bool expand = true) => GUILayout.ExpandWidth(expand);
            public static GUILayoutOption ExpandHeight(bool expand = true) => GUILayout.ExpandHeight(expand);
            
        }
        
        public static GUILayoutOption Width(this string content, float space = 20f)
        {
            return Width(new GUIContent(content), space);
        }
        public static GUILayoutOption Width(this GUIContent content, float space = 20f)
        {
            return LayoutOptions.Width(content.RawWidth(space));
        }
        public static float RawWidth(this GUIContent content, float space = 20f)
        {
            var dim = GUI.skin.label.CalcSize(content);
            return dim.x + space;
        }

        public static HorizontalLayoutCommand DoHorizontalLayout(params GUILayoutOption[] options)
        {
            return new HorizontalLayoutCommand(GUIStyle.none, options);
        }

        public static HorizontalLayoutCommand DoHorizontalLayout(GUIStyle style, params GUILayoutOption[] options)
        {
            return new HorizontalLayoutCommand(style, options);
        }


        public static void DrawGenericField(this SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Enum:
                    property.enumValueIndex = EditorGUILayout.Popup(property.enumValueIndex, property.enumNames);
                    break;
                case SerializedPropertyType.ManagedReference:
                    DrawNonUnityObject(property);
                    break;
                default:
                    EditorGUILayout.PropertyField(property, GUIContent.none);
                    break;
            }
        }

        public static void DrawNonUnityObject(this SerializedProperty property)
        {
            EditorGUI.indentLevel++;
            // Iterate over child properties
            SerializedProperty iterator = property.Copy();
            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren))
            {
                // Don't draw the parent property itself
                if (SerializedProperty.EqualContents(iterator, property))
                    continue;

                EditorGUILayout.PropertyField(iterator, true);
                enterChildren = false;
            }

            EditorGUI.indentLevel--;
        }

        public struct Section
        {
            public float Size { get; }
            public Action<Rect> DrawCallback { get; }

            public Section(float size, Action<Rect> drawCallback)
            {
                Size = size;
                DrawCallback = drawCallback;
            }

            public Section(Action<Rect> drawCallback) : this(-1, drawCallback)
            {
            }
        }

        public static void DrawSequenceHorizontally(Rect rect,
            params Section[] sections)
        {
            DrawSequenceHorizontally(rect, 0f, sections);
        }
        
        public static void DrawSequenceVertically(Rect rect,
            float space, params Section[] sections)
        {
            foreach (var section in sections)
            {
                var tempRect = rect;
                var size = section.Size;
                tempRect.height = size;
                section.DrawCallback?.Invoke(tempRect);
                var useWidth = space + size;
                rect.y += useWidth;
            }
        }

        public static void DrawSequenceHorizontally(Rect rect,
            float space, params Section[] sections)
        {
            var remainingWidth = rect.width;
            foreach (var section in sections)
            {
                var tempRect = rect;
                var width = section.Size;
                tempRect.width = width > 0 ? width : remainingWidth;
                section.DrawCallback?.Invoke(tempRect);

                var useWidth = space + tempRect.width;
                // Increase x anchor
                rect.x += useWidth;
                remainingWidth -= useWidth;
            }
        }
    }
}