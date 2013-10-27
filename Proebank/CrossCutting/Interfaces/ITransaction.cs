using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCutting.Interfaces
{
    public interface ITransaction
    {
        Guid Id { get; set; }

        DateTime Time { get; set; }

        decimal Amount { get; set; }

        IAccount From { get; set; }

        IAccount To { get; set; }

        IEmployee CommitedBy { get; set; }
    }
}
