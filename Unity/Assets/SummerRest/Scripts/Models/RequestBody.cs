﻿using System;
using SummerRest.DataStructures.Containers;
using SummerRest.Models.Interfaces;
using TypeReferences;
using UnityEngine;

namespace SummerRest.Models
{
    [Serializable]
    public class RequestBody
    {
        [SerializeField] private RequestBodyType type;
        [SerializeField] private string text;
        [SerializeField] private RequestBodyContainer bodyContainer;
        public string SerializedData => type == RequestBodyType.PlainText ? text : bodyContainer.SerializedData;
        [Serializable]
        public class RequestBodyContainer : InterfaceContainer<IRequestBodyData>
        {
            [SerializeField, Inherits(typeof(IRequestBodyData))] private TypeReference typeReference;
            public override Type Type => typeReference?.Type is null ? null : Type.GetType(typeReference.TypeNameAndAssembly);
        }
    }
}