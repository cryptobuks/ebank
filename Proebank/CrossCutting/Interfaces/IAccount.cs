using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCutting.Interfaces
{
    public interface IAccount
    {
        Guid Id { get; set; }
        
        string Number { get; set; }

        decimal Balance { get; set; }

        DateTime CreationDate { get; set; }

        IEmployee Employee { get; set; }
    }
}
