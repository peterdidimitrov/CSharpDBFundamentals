using System.Text;
using System.Xml.Serialization;

namespace Boardgames.Extensions
{
    public static class SerializationMethods
    {
        public static T DeserializeFromXml<T>(this string xml, string rootElement)
        {
            T result = default(T)!;

            XmlRootAttribute rootAttribute = new XmlRootAttribute(rootElement);
            XmlSerializer serializer = new XmlSerializer(typeof(T), rootAttribute);

            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                result = (T)serializer.Deserialize(ms);
            }

            return result;
        }

        public static string SerializeToXml<A>(this A obj, string rootElement)
            where A : class
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            string result = string.Empty;
            XmlSerializer serializer = new XmlSerializer(typeof(A), new XmlRootAttribute(rootElement));

            using (MemoryStream ms = new MemoryStream())
            {
                serializer.Serialize(ms, obj, ns);
                result = Encoding.UTF8.GetString(ms.ToArray()); 
            }

            return result;
        }
    }
}
