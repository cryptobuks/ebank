using CrossCutting.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureLayer.Entities
{
    class Account : IAccount
    {
        public Guid Id { get; set; }

        public string Number { get; set; }

        public decimal Balance { get; set; }

        public DateTime CreationDate { get; set; }

        public IEmployee Employee { get; set; }
    }
}
