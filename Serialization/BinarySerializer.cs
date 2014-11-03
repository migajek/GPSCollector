using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
namespace Serialization
{
    internal struct PropertyDeserializer
    {
        public byte DataLength { get; set; }
        public Func<byte[], object> Convert { get; set; }
    }


    public class BinarySerializer
    {        
        private Dictionary<Type, PropertyDeserializer> deserializers = new Dictionary<Type, PropertyDeserializer>            
        {
            {typeof(int), new PropertyDeserializer(){ DataLength = 4, Convert = bytes => BitConverter.ToInt32(bytes,0)}},
            {typeof(ulong), new PropertyDeserializer(){ DataLength = 8, Convert = bytes => BitConverter.ToUInt64(bytes,0)}},
            {typeof(byte), new PropertyDeserializer(){ DataLength = 1, Convert = bytes => bytes[0]}},
            {typeof(UInt32), new PropertyDeserializer(){ DataLength = 4, Convert = bytes => BitConverter.ToUInt32(bytes, 0)}},
            {typeof(UInt16), new PropertyDeserializer(){ DataLength = 2, Convert = bytes => BitConverter.ToUInt16(bytes, 0)}}
        };

        protected IEnumerable<PropertyInfo> GetOrderedProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(x => new
                {
                    MemberInfo = x,
                    SerializeAttributes = x.GetCustomAttributes(typeof(SerializeFieldAttribute), true)
                })
                .Where(x => x.SerializeAttributes.Any())
                .OrderBy(x => ((SerializeFieldAttribute)x.SerializeAttributes.First()).Order)
                .Select(x => x.MemberInfo);
        }

        protected IEnumerable<byte> SerializeProperty(PropertyInfo propertyInfo, object instance)
        {
            byte[] bytes = null;
            if (propertyInfo.PropertyType == typeof(byte))
                bytes = new[] { (byte)propertyInfo.GetValue(instance, null) };
            else
            {                
                // TODO: cache "GetBytes" methods (static filed) Dict<Type-to-be-serialized, Method>
                var type = propertyInfo.PropertyType;
                object value = propertyInfo.GetValue(instance, null);
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    type = type.GetGenericTypeDefinition().GetGenericArguments()[0];                    
                }
                var method = typeof(BitConverter).GetMethod("GetBytes",
                    new Type[] { type });
                if (method == null)
                    throw new Exception(
                        String.Format("No BitConverter.GetBytes for property {0}.{1} ({2})",
                            instance.GetType().Name, propertyInfo.Name, propertyInfo.PropertyType.Name));
                bytes = (byte[])method.Invoke(null, new object[] { value });
            }
            return bytes;
        }

        public IEnumerable<byte> Serialize(object instance)
        {
            if (instance == null)
                yield break;
            var properties = GetOrderedProperties(instance.GetType());            
            foreach (var b in properties.SelectMany(property => SerializeProperty(property, instance)))
            {
                yield return b;
            }
        }

        public T Deserialize<T>(Stream s) where T : class
        {
            return Deserialize(s, typeof (T)) as T;
        }

        public object Deserialize(Stream s, Type type)
        {
            var instance = Activator.CreateInstance(type);
            using (var reader = new StreamReader(s))
                foreach (var property in GetOrderedProperties(type))
                {
                    if (!deserializers.ContainsKey(property.PropertyType))
                        throw new Exception(String.Format("No deserializer for {0} {1}", property.PropertyType.Name,
                            property.Name));
                    var deserializer = deserializers[property.PropertyType];
                    var b = new byte[deserializer.DataLength];
                    s.Read(b, 0, b.Length);
                    var value = deserializer.Convert(b);
                    property.SetValue(instance, value, null);
                }
            return instance;
        }
    }
}
