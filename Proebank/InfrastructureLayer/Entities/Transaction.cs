using CrossCutting.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureLayer.Entities
{
    class Transaction : ITransaction
    {
        public Guid Id { get; set; }

        public DateTime Time { get; set; }

        public decimal Amount { get; set; }

        public IAccount From { get; set; }

        public IAccount To { get; set; }

        public IEmployee CommitedBy { get; set; }
    }
}
