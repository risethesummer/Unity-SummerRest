﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SummerRest.Editor.DataStructures;
using SummerRest.Runtime.DataStructures;
using SummerRest.Runtime.Extensions;
using UnityEngine;

namespace SummerRest.Editor.Models
{
    [Serializable]
    public class PathContainer
    {
        [SerializeField] private string text;
        [SerializeField] private List<SmartString> values = new();
        public string FormatText { get; private set; }
        // Use KeyValue for simple source generation
        public KeyValue[] Containers => values.Select(e => new KeyValue(e.Key, e.Value)).ToArray();
        public string FinalText { get; private set; }
        public int Count => values.Count;
        public void DetectSmartKeys()
        {
            char previousCut = default;
            int currentIdx = 0;
            // Clear all keys
            foreach (var smartString in values)
                smartString.Key = null;
            foreach (var (start, c) in new StringSegment(text, '{'))
            {
                if (previousCut == '{' 
                    && start.SplitKeyValue(out var key, out _, separator: '}'))
                {
                    var keyStr = key.ToString();
                    // Duplicated keys => use the first one
                    if (values.Find(e => e.Key == keyStr) is not null)
                        continue;
                    if (currentIdx < values.Count)
                        values[currentIdx].Key = keyStr;
                    else
                        values.Add(new SmartString(keyStr));
                    currentIdx++;
                }
                previousCut = c.Length > 0 ? c[0] : default;
            }
            // Remove from the latest key to the end
            if (currentIdx < values.Count) 
                values.RemoveAt(currentIdx);
        }
        private string FormFormatText()
        {
            var builder = new StringBuilder();
            builder.Append(text);
            //replace {key} => {<position>} to form formatter string
            for (var i = 0; i < values.Count; i++)
            {
                builder.Replace($"{{{values[i].Key}}}", $"{{{i}}}");
            }
            return builder.ToString();
        }
        public void CacheValues()
        {
            DetectSmartKeys();
            FormatText = FormFormatText();
            try
            {
                FinalText = string.Format(FormatText, values.Select(e => (object)e.Value).ToArray());
            }
            catch (Exception)
            {
                Debug.LogWarning($"{FormatText} is not a valid formatted string");
            }
        }
    }
}