using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MyApps.Infrastructure;

[XmlRoot("dictionary")]
[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
{
    public SerializableDictionary()
    {
    }

    public SerializableDictionary(int capacity) : base(capacity)
    {
    }

    public SerializableDictionary(IEqualityComparer<TKey> comparer) : base(comparer)
    {
    }

    public SerializableDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer)
    {
    }

    public SerializableDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary)
    {
    }

    public SerializableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer)
    {
    }

    protected SerializableDictionary(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public XmlSchema GetSchema()
    {
        return null;
    }

    public void ReadXml(XmlReader reader)
    {
        var wasEmpty = reader.IsEmptyElement;
        reader.Read();
        if (wasEmpty) return;

        while (reader.NodeType != XmlNodeType.EndElement)
        {
            reader.ReadStartElement("item");
            var key = XmlDeserializer<TKey>(reader, "key");
            var value = XmlDeserializer<TValue>(reader, "value");
            Add(key, value);
            reader.ReadEndElement();
            reader.MoveToContent();
        }

        reader.ReadEndElement();
    }

    public void WriteXml(XmlWriter writer)
    {
        var emptyNamespaces = new XmlSerializerNamespaces();
        emptyNamespaces.Add("", "any-non-empty-string");

        foreach (var key in Keys)
        {
            var value = this[key];
            writer.WriteStartElement("item");
            XmlSerializer(writer, "key", key, emptyNamespaces);
            XmlSerializer(writer, "value", value, emptyNamespaces);
            writer.WriteEndElement();
        }
    }

    private static T XmlDeserializer<T>(XmlReader reader, string elementName)
    {
        var serializer = new XmlSerializer(typeof(T));

        reader.ReadStartElement(elementName);
        var value = (T)serializer.Deserialize(reader);
        reader.ReadEndElement();

        return value;
    }

    private static void XmlSerializer<T>(XmlWriter writer, string elementName, T value, XmlSerializerNamespaces namespaces)
    {
        var serializer = new XmlSerializer(typeof(T));

        writer.WriteStartElement(elementName);
        serializer.Serialize(writer, value, namespaces);
        writer.WriteEndElement();
    }
}