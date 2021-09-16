using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NetInterop.Transport.Core.Delegates;
using NetInterop.Transport.Core.Abstractions;
using NetInterop.Transport.Core.Abstractions.Packets;

namespace NetInterop.Transport.Core.Packets
{
    public class MemberConverter
    {
        private readonly IPrimitiveConverterGenerator primitiveGenerator;

        public MemberConverter(IPrimitiveConverterGenerator primitiveGenerator)
        {
            this.primitiveGenerator = primitiveGenerator;
        }

        public ToSpanFunc<object, byte> GetConverter(PropertyInfo info)
        {
            if (info.PropertyType.IsPrimitive)
            {
                var propertyType = info.PropertyType;

                var primitiveConverter = primitiveGenerator.GetPrimitiveConverter(propertyType);

                return (instance) => primitiveConverter(info.GetValue(instance));
            }
            if (info.PropertyType.IsArray)
            {
                // LINQ might be slow here TBD
                Type subType = info.PropertyType.GetGenericArguments().FirstOrDefault();

                if (subType.IsPrimitive)
                {

                }
            }

            return default;
        }
    }
}
