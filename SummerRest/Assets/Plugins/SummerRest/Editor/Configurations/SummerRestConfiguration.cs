using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using SummerRest.Editor.Models;
using SummerRest.Editor.TypeReference;
using SummerRest.Editor.Utilities;
using SummerRest.Runtime.Authenticate.Repositories;
using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.Requests;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace SummerRest.Editor.Configurations
{
    /// <summary>
    /// Singleton class storing the configuration of the plugin <br />
    /// </summary>
    public class SummerRestConfiguration : DataStructures.ScriptableSingleton<SummerRestConfiguration>
    {
        private void OnEnable()
        {
            // Just run on persistant object
            if (!this.IsPersistentAsset())
                return;
            LoadInstance();
        }

        /// <summary>
        /// Domains
        /// </summary>
        [XmlArray]
        [field: SerializeReference]
        public List<Domain> Domains { get; set; } = new();

        /// Each <see cref="AuthContainer"/> points to an auth data (userId, token...)/><br /> 
        [SerializeField] private List<AuthContainer> authContainers = new();

        [XmlIgnore] public List<AuthContainer> AuthContainers => authContainers;

        [XmlArray]
        public string[] AuthKeys
        {
            get => authContainers.Select(e => e.AuthKey).ToArray();
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// Type ref of auth repository <see cref="ISecretRepository"/> <br />
        /// Default is <see cref="PlayerPrefsSecretRepository"/>
        /// </summary>
        [SerializeField, ClassTypeConstraint(typeof(ISecretRepository))]
        private ClassTypeReference secretRepository = new(typeof(PlayerPrefsSecretRepository));

        [XmlAttribute]
        public string SecretRepository
        {
            get => secretRepository.Type is null ? typeof(PlayerPrefsSecretRepository).AssemblyQualifiedName : secretRepository.Type.AssemblyQualifiedName;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// Type ref of data serializer <see cref="IDataSerializer"/> <br />
        /// Default is <see cref="DefaultDataSerializer"/>
        /// </summary>
        [SerializeField, ClassTypeConstraint(typeof(IDataSerializer))]
        private ClassTypeReference dataSerializer = new(typeof(DefaultDataSerializer));

        [XmlAttribute]
        public string DataSerializer
        {
            get => dataSerializer.Type is null ? typeof(DefaultDataSerializer).AssemblyQualifiedName : dataSerializer.Type.AssemblyQualifiedName;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The target assembly for generating <see cref="BaseRequest{TRequest}"/> classes to call apis in runtime
        /// </summary>
        [SerializeField] private AssemblyDefinitionAsset targetAssembly;

        [Serializable]
        private class AssemblyName
        {
            public string name;
        }


        [XmlAttribute]
        public string Assembly
        {
            get
            {
                if (targetAssembly is null)
                    return ReflectionExtensions.LoadDefaultAssemblyName();
                try
                {
                    // Unity does not provide any way to access name of an AssemblyDefinitionAsset
                    // We need to deserialize back into a class first to get the name
                    var assembly = JsonUtility.FromJson<AssemblyName>(targetAssembly.text);
                    return assembly.name;
                }
                catch (Exception)
                {
                    return ReflectionExtensions.LoadDefaultAssemblyName();
                }
            }
            set => throw new NotImplementedException();
        }

        public void ValidateToGenerate()
        {
            ValidateDomains();
            var assembly = System.Reflection.Assembly.Load(Assembly);
            ValidateAuthContainers(assembly);
        }

        internal void ValidateDomains()
        {
            foreach (var domain in Domains)
                domain.ValidateToGenerate();
        }

        internal void ValidateAuthContainers(Assembly assembly)
        {
            var authDuplicateKeys = authContainers.FindDuplicates(s => s.AuthKey);
            if (authDuplicateKeys.Length > 0)
            {
                throw new Exception(
                    $"The followings auth keys [{string.Join(',', authDuplicateKeys)}] are duplicated, please check your auth containers in the advanced settings section");
            }
            foreach (var authContainer in authContainers)
                authContainer.ValidateToGenerate(assembly);
        }

        
        public void RenameAssets()
        {
            for (int i = 0; i < Domains.Count; i++)
                Domains[i].Rename(string.Empty, i);
            this.MakeDirty();
            AssetDatabase.SaveAssets();
        }
    }
}