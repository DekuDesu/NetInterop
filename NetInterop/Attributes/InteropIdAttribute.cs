using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetInterop.Attributes
{
    /// <summary>
    /// This is a more compact version of [Guid("000-...")], use this if you do not have many types to register < 100
    /// </summary
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Assembly | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Interface)]
    public class InteropIdAttribute : Attribute
    {
        /// <summary>
        /// The Id that is used to identify a Interop resource between client and server. Should be unique from all other Id's
        /// </summary>
        public ushort Id { get; set; }

        public InteropIdAttribute(ushort networkId)
        {
            this.Id = networkId;
        }
    }
}
