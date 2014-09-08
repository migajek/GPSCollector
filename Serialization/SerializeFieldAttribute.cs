using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serialization
{
    public class SerializeFieldAttribute: Attribute
    {
        public int Order { get; internal set; }
        public SerializeFieldAttribute(int order)
        {
            Order = order;
        }
    }
}
