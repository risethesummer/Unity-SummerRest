﻿using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.RequestComponents;
using UnityEngine;

namespace SummerRest.Runtime.Authenticate.Repositories
{
    /// <summary>
    /// Default <see cref="ISecretRepository"/> that leverages <see cref="PlayerPrefs"/> to store and retrieve data
    /// </summary>
    public class PlayerPrefsSecretRepository : ISecretRepository
    {
        public void Save<TData>(string key, TData data)
        {
            var json = IDataSerializer.Current.Serialize(data, DataFormat.Json);
            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();
        }
        public void Delete(string key)
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }

        public bool TryGet<TData>(string key, out TData data)
        {
            var valueFromPref = PlayerPrefs.GetString(key, null);
            if (string.IsNullOrEmpty(valueFromPref))
            {
                data = default;
                return false;
            }
            data = IDataSerializer.Current.Deserialize<TData>(PlayerPrefs.GetString(key, null), DataFormat.Json);
            return true;
        }
    }
}