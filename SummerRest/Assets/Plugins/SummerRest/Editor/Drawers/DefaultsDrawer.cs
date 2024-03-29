using System.Collections.Generic;
using System.Linq;
using SummerRest.Editor.Utilities;
using SummerRest.Runtime.Attributes;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(DefaultsAttribute))]
    internal class DefaultsDrawer : UIToolkitDrawer
    {
        protected override string RelativeFromTemplateAssetPath => "Properties/default-or-custom.uxml";

        private List<string> _defaultValues;
        private void Init()
        {
        }
        public int GetIdx(string value)
        {
            var find = _defaultValues.IndexOf(value);
            return Mathf.Max(0, find);
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var tree = Tree;
            
            _defaultValues ??= ((DefaultsAttribute)attribute).Defaults.Prepend("Custom").ToList();
            
            var nameElement = tree.Q<Label>(name: "name");
            nameElement.text = property.displayName;
            
            var customElement = tree.Q<TextField>("custom");
            customElement.BindProperty(property);
            
            var valuesElement = tree.Q<DropdownField>(name: "values");
            valuesElement.choices = _defaultValues;
            valuesElement.CallThenTrackPropertyValue(property, e =>
            {
                valuesElement.index = GetIdx(e.stringValue);
            });
            valuesElement.RegisterValueChangedCallback(e =>
            {
                var val = e.newValue;
                if (val is not null)
                    ShowCustomElement(customElement, property, val);
            });
            ShowCustomElement(customElement, property, property.stringValue);
            return tree;
        }
        private void ShowCustomElement(TextField customElement, SerializedProperty property, string newVal)
        {
            property.serializedObject.Update();
            var currentIdx = GetIdx(property.stringValue);
            var newIdx = GetIdx(newVal);
            if (currentIdx != newIdx) //"Custom" always return 0 => no need to update because of overwriting the current value
                customElement.value = newVal;
            customElement.Show(newIdx == 0);
        }

    }
}