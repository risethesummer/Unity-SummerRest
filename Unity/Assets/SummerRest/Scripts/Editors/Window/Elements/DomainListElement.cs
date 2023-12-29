﻿using SummerRest.Models;
using UnityEngine.UIElements;

namespace SummerRest.Editors.Window.Elements
{
    public class DomainListElement : SelectableListElement<DomainListElement, DomainItemElement, Domain>
    {
        public new class UxmlFactory : UxmlFactory<DomainListElement, UxmlTraits>
        {
        }
    }
}