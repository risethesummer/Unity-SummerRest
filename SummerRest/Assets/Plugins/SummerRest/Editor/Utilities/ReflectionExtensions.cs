using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor.Compilation;
using UnityEngine;
using Assembly = System.Reflection.Assembly;

namespace SummerRest.Editor.Utilities
{
    public static class ReflectionExtensions
    {
        public static string ToClassName(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            StringBuilder formattedName = new StringBuilder();
            bool capitalizeNextChar = true;
            foreach (var c in value)
            {
                if (char.IsLetterOrDigit(c) || c == '_')
                {
                    if (formattedName.Length == 0 && char.IsDigit(c))
                    {
                        // If the first character is a digit, prefix with an underscore
                        formattedName.Append('_');
                    }
                    if (capitalizeNextChar)
                    {
                        formattedName.Append(char.ToUpper(c));
                        capitalizeNextChar = false;
                    }
                    else
                    {
                        formattedName.Append(c);
                    }
                }
                else
                {
                    capitalizeNextChar = true;
                }
            }

            return formattedName.ToString();
        }

        public static bool IsRunnableWith(this Type type, Assembly assembly)
        {
            if (type is null || assembly is null)
                return true;
            if (type.Assembly.FullName == assembly.FullName)
                return true;
            if (type.IsNotPublic)
                return false;
            foreach (var referencedAssembly in assembly.GetReferencedAssemblies())
                if (type.Assembly.FullName == referencedAssembly.FullName)
                    return true;
            return false;
        }
        
        public static IEnumerable<Assembly> GetAllAssemblies()
        {
            return CompilationPipeline.GetAssemblies().Select(e => Assembly.Load(e.name));
        } 
        /// <summary>
        /// Default assembly of Unity
        /// </summary>
        private const string DefaultUnityFirstPassAssembly = "Assembly-CSharp-firstpass";
        private const string DefaultUnityAssembly = "Assembly-CSharp";
        private const string SummerRestAssembly = "SummerRest";
        public static Assembly LoadDefaultAssembly()
        {
            try
            {
                return Assembly.Load(DefaultUnityFirstPassAssembly);
            }
            catch (Exception)
            {
                return Assembly.Load(DefaultUnityAssembly);
            }
        }
        public static string LoadDefaultAssemblyName()
        {
            try
            {
                return LoadDefaultAssembly().GetName().Name;
            }
            catch (Exception)
            {
                Debug.LogWarningFormat("There is no script in the default assembly of your project! The generating process will happen in {0}", nameof(SummerRestAssembly));
                return SummerRestAssembly;
            }
        }

        public static void CallGenericMethod(this object obj, string methodName, Type[] typeArguments, params object[] parameters)
        {
            Type objectType = obj.GetType();
            MethodInfo method = objectType.GetMethod(methodName);
            MethodInfo genericMethod = method.MakeGenericMethod(typeArguments);
            // Invoke the generic method
            genericMethod.Invoke(obj, parameters);
        }
        public static void CallMethod(this object obj, string methodName, params object[] parameters)
        {
            Type objectType = obj.GetType();
            MethodInfo method = objectType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            method?.Invoke(obj, parameters);
        }
    }
}