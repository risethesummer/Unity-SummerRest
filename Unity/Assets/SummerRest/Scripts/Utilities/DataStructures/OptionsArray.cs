﻿using System;
using UnityEngine;

namespace SummerRest.Scripts.Utilities.DataStructures
{
    [Serializable]
    public class OptionsArray<T> 
    {
        [SerializeField, HideInInspector] private T[] values;
        [SerializeField, HideInInspector] private int selectedIndex;
        public T[] Values => values;
        public T Value => selectedIndex >= 0 && selectedIndex < values.Length ? values[selectedIndex] : default;
    }
}