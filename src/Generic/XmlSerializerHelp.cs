using System.Text;
using System.Xml;
using System.Xml.Serialization;

class XmlSerializerHelp
{
    public static void SaveXML(string file, object obj)
    {
        using XmlTextWriter writer = new XmlTextWriter(file, Encoding.UTF8);
        writer.Formatting = Formatting.Indented;
        XmlSerializer serializer = new XmlSerializer(obj.GetType());
        serializer.Serialize(writer, obj);
    }

    public static T LoadXML<T>(string file)
    {
        using XmlTextReader reader = new XmlTextReader(file);
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        return (T)serializer.Deserialize(reader);
    }
}