using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
namespace Serialization
{
    public class BinarySerializer
    {
        protected IEnumerable<byte> SerializeProperty(PropertyInfo propertyInfo, object instance)
        {
            byte[] bytes = null;
            if (propertyInfo.PropertyType == typeof(byte))
                bytes = new byte[1] { (byte)propertyInfo.GetValue(instance, null) };
            else
            {
                // for each public property, find the GetBytes method
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
            var properties = instance.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(x => new
                {
                    MemberInfo = x,
                    SerializeAttributes = x.GetCustomAttributes(typeof(SerializeFieldAttribute), true)
                })
                .Where(x => x.SerializeAttributes.Any())
                .OrderBy(x => ((SerializeFieldAttribute)x.SerializeAttributes.First()).Order)
                .Select(x => x.MemberInfo);
            
            foreach (var property in properties)
            {
                //TODO: is that faster than appending?
                foreach (var b in SerializeProperty(property, instance))
                {
                    yield return b;
                }
            }
        }
    }
}
