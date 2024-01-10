﻿using System.IO;
using Newtonsoft.Json;
using SummerRest.Editor.Configurations;
using SummerRest.Editor.Utilities;
using UnityEditor;
using UnityEngine;

namespace SummerRest.Editor.Manager
{

    public static class SourceGenerator
    {
        private const string FileName = "summer-rest-generated.SummerRestRequestsGenerator.additionalfile";
        public static void GenerateAdditionalFile()
        {
            var path = SummerRestConfiguration.Instance.GetAssetFolder() + "/" + FileName;
            var domains = SummerRestConfiguration.Instance.Domains;
            var configureJson = JsonConvert.SerializeObject(domains);
            var jsonAsset = EditorAssetUtilities.LoadOrCreate(path, () => new TextAsset());
            if (jsonAsset is null)
            {
                Debug.LogErrorFormat("Can not create new asset file {0} at {1}", FileName, path);
                return;
            }
            File.WriteAllText(path, string.Empty);
            File.WriteAllText(path, configureJson);
            AssetDatabase.ImportAsset(path);
        }
    }
}