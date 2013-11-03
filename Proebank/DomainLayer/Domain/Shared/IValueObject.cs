using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shared
{
    public interface IValueObject<T>
    {
        /// <summary>
        /// Value objects compare by the values of their attributes, 
        /// they don't have an identity.
        /// </summary>      
        bool SameValueAs(T other);
    }
}
