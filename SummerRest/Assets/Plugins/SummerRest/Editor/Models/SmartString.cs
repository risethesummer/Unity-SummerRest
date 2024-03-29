﻿using System;
using UnityEngine;

namespace SummerRest.Editor.Models
{
    // I encountered an error that ISerializationCallbackReceiver won't be called anymore after changing any serialized field 
    // So I need to create this class to leverage ISerializationCallbackReceiver instead of the KeyValue struct
    [Serializable]
    public class SmartString : ISerializationCallbackReceiver
    {
        [SerializeField] private string key;
        [SerializeField] private string value;

        // Trigger editor redraw 
        public string Value => value;

        public string Key
        {
            get;
            set;
        }

        public SmartString(string key)
        {
            this.Key = key;
        }

        public void OnBeforeSerialize()
        {
            key = Key;
        }
        public void OnAfterDeserialize()
        {
        }
    }
}