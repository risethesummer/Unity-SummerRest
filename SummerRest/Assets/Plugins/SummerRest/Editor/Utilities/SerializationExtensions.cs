﻿using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using SummerRest.Editor.Utilities.Validators;
using SummerRest.Runtime.DataStructures;
using SummerRest.Runtime.RequestComponents;

namespace SummerRest.Editor.Utilities
{
    public static class SerializationExtensions
    {

        public static void WriteArray<T>(this XmlWriter xmlWriter, string name, string elementName, IReadOnlyCollection<T> array) where T : IXmlSerializable
        {
            if (array is {Count: >0})
            {
                xmlWriter.WriteStartElement(name);
                foreach (var element in array)
                {
                    xmlWriter.WriteStartElement(elementName);
                    element.WriteXml(xmlWriter);
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();
            }
        }
        
        public static void WriteArray<T>(this XmlWriter xmlWriter, string name, IReadOnlyCollection<T> array)
        {
            if (array is {Count: >0})
            {
                xmlWriter.WriteStartElement(name);
                foreach (var element in array)
                    xmlWriter.WriteObject(null, element);
                xmlWriter.WriteEndElement();
            }
        }
        public static void WriteObject<T>(this XmlWriter writer, string name, T data)
        {
            var xsSubmit = string.IsNullOrEmpty(name) ? new XmlSerializer(typeof(T)) : new XmlSerializer(typeof(T), new XmlRootAttribute(name));
            xsSubmit.Serialize(writer, data);
        }


        public static DataFormat DetectFormat(this string data)
        {
            // Try to detect json
            if (ISingleton<JsonValidator>.GetSingleton().Validate(data))
                return DataFormat.Json;
            if (ISingleton<XmlValidator>.GetSingleton().Validate(data))
                return DataFormat.Xml;
            return DataFormat.PlainText;
        }
    }
}