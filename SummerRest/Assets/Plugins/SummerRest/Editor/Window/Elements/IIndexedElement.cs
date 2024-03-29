using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Window.Elements
{
    public interface IIndexedElement<TElement, TSetupData> where TElement : VisualElement
    {
        event Action<TElement> OnClicked;
        event Action<TElement> OnDeleted;
        TSetupData Data { get; }
        int Index { get; set; }
        void Init(int index, TSetupData data);
        void Enable(Color highlight);
        void Disable();
    }
}