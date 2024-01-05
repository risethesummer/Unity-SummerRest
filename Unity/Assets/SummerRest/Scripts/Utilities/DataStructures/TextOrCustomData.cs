﻿using System;
using UnityEngine;

namespace SummerRest.Scripts.Utilities.DataStructures
{
    public enum TextOrCustomDataType
    {
        PlainText, Data
    }
    [Serializable]
    public class TextOrCustomData<TBody, TBodyContainer> where TBodyContainer : InterfaceContainer<TBody>
    {

        [SerializeField] private TextOrCustomDataType type;
        [SerializeField] private string text;
        [SerializeField] private TBodyContainer bodyContainer;
        public string SerializedData => type == TextOrCustomDataType.PlainText ? text : bodyContainer.SerializedData;
    }
}