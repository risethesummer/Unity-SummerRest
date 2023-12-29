﻿using System;
using System.Net;
using SummerRest.Models;
using SummerRest.Scripts.Utilities.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SummerRest.Editors.Window.Elements
{
    public class ResponseElement : VisualElement
    {
        private TextField _statusCodeElement;
        public new class UxmlFactory : UxmlFactory<ResponseElement, UxmlTraits>
        {
        }
        public void Init(SerializedProperty serializedProperty)
        {
            if (serializedProperty is null)
            {
                this.Show(false);
                return;
            }
            this.Show(true);
            var statusCodeProp = serializedProperty.FindPropertyRelative("statusCode");
            _statusCodeElement = this.Q<TextField>("status-code");
            _statusCodeElement.Unbind();
            _statusCodeElement.BindWithCallback<TextField, string>(statusCodeProp, SetStatusCodeValue);
            this.Q<PropertyField>("headers").BindProperty(serializedProperty.FindPropertyRelative("headers"));
            this.Q<TextField>("body").BindProperty(serializedProperty.FindPropertyRelative("body"));
        }

        private void SetStatusCodeValue(string p)
        {
            var statusCode = Enum.Parse<HttpStatusCode>(p, true);
            _statusCodeElement.SetValueWithoutNotify($"{(int)statusCode} ({p})");
        }
        public ResponseElement()
        {
        }
    }
}