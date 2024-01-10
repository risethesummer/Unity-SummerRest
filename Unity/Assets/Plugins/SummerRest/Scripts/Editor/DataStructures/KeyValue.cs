using System;
using System.Collections.Generic;
using UnityEngine;

namespace SummerRest.Editor.DataStructures
{
    [Serializable]
    public struct KeyValue
    {
        [SerializeField] private string key;
        public string Key => key;
        [SerializeField] private string value;
        public string Value => value;
        public KeyValue(string key, string value)
        {
            this.key = key;
            this.value = value;
        }
        public KeyValue Clone()
        {
            return new KeyValue(key, value);
        }

        public static implicit operator KeyValuePair<string, string>(KeyValue key) => new(key.Key, key.Value);
        public static implicit operator KeyValue(KeyValuePair<string, string> key) => new(key.Key, key.Value);
 
        public void Deconstruct(out string outKey, out string outValue)
        {
            outKey = Key;
            outValue = Value;
        }
    }
}