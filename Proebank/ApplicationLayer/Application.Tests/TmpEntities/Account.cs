using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrossCutting.Interfaces;

namespace Application.Tests.TmpEntities
{
    class Account : IAccount
    {
        public Account(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; private set; }

        public string Number { get; set; }

        public decimal Balance { get; set; }

        public DateTime CreationDate { get; set; }

        public IEmployee Employee { get; set; }
    }
}
