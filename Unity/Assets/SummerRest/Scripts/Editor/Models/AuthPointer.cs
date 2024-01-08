﻿using System;
using System.Linq;
using Newtonsoft.Json;
using SummerRest.Editor.Configurations;
using UnityEngine;

namespace SummerRest.Editor.Models
{
    [Serializable]
    public partial struct AuthPointer
    {
        [SerializeField, JsonIgnore] private string authKey;
        public string AuthKey => authKey;
    }
#if UNITY_EDITOR
    public partial struct AuthPointer : ISerializationCallbackReceiver
    {
        public static implicit operator AuthContainer(AuthPointer p)
        {
            var authConfigure= SummerRestConfigurations.Instance.AuthenticateConfigurations;
            return authConfigure.AuthContainers.FirstOrDefault(e => e.Key == p.authKey);
        }
        public static implicit operator AuthPointer(AuthContainer key) => new()
        {
            authKey = key.Key
        };
        public void OnBeforeSerialize()
        {
        }
        public void OnAfterDeserialize()
        {

        }
    }
#endif
}