﻿using System;
using UnityEngine;

namespace SummerRest.Scripts.DataStructures
{
    [Serializable]
    public class GenericObject
    {
        
    }
    [Serializable]
    public class OptionsArray<T> 
    {
        [SerializeField, HideInInspector] private T[] values;
        [SerializeField, HideInInspector] private int selectedIndex;
        public T[] Values => values;
        public T Value => values[selectedIndex];
    }
}