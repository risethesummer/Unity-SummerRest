using System;
using System.Linq;
using SummerRest.Editor.Models;
using SummerRest.Editor.Utilities;
using Unity.Collections;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(RequestBody))]
    internal class RequestBodyDrawer : TextOrCustomDataDrawer
    {
        protected override string RelativeFromTemplateAssetPath => "Properties/request-body.uxml";
        public override Enum DefaultEnum => RequestBodyType.PlainText;
        protected override VisualElement[] GetShownElements(VisualElement tree)
        {
            return base.GetShownElements(tree).Concat(new VisualElement[]
            {
                tree.Q<PropertyField>("form"),
                tree.Q<Foldout>("serialized-body")
            }).ToArray();
        }
        protected override void BindValueElement(int value, VisualElement tree)
        {
            base.BindValueElement(value, tree);
            if ((RequestBodyType)value is RequestBodyType.Data or RequestBodyType.PlainText)
                tree.Q<Foldout>("serialized-body").Show(true);
        }
    }
}