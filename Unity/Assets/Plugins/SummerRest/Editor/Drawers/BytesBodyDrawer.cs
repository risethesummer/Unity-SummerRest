using SummerRest.Editor.Models;
using SummerRest.Editor.Utilities;
using Unity.Collections;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(BytesBody))]
    internal class BytesBodyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var image = new Image
            {
                scaleMode = ScaleMode.ScaleToFit
            };
            image.TrackPropertyValue(property, p =>
            {
                p.serializedObject.Update();
                SetImage(p, image);
            });
            SetImage(property, image);
            return image;
        }
        
        private void SetImage(SerializedProperty property, Image image)
        {
            var bytesProp = property.FindPropertyRelative("data");
            var isImageProp = property.FindPropertyRelative("isImage");
            if (bytesProp.arraySize == 0 || !isImageProp.boolValue)
            {
                image.image = null;
                return;
            }
            using var nativeBytes = bytesProp.GetArrayValue();
            var text = image.image as Texture2D ?? new Texture2D(1, 1);
            text.LoadImage(nativeBytes.ToArray());
            text.Apply();
            image.image = text;
        }
    }
}