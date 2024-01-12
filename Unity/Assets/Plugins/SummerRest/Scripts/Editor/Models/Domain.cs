using Newtonsoft.Json;
using SummerRest.Editor.DataStructures;
using UnityEngine;

namespace SummerRest.Editor.Models
{
    public class Domain : EndpointContainer
    {
        [SerializeField] private OptionsArray<string> versions;
        public override void CacheValues()
        {
            activeVersion = versions.Value;
            if (string.IsNullOrEmpty(activeVersion))
            {
                activeVersion = "http://localhost:8080";
                versions.Values = new[] { activeVersion };
            }
            base.CacheValues();
        }
        [SerializeField, JsonIgnore] private string activeVersion;
        public string ActiveVersion => activeVersion;
        public override string TypeName => nameof(Models.Domain);
    }
}