using CrossCutting.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public class AccountModel : IAccount
    {
        public Guid Id { get; set; }

        public string Number { get; set; }

        public decimal Balance { get; set; }

        public DateTime CreationDate { get; set; }

        public IEmployee Employee { get; set; }
    }
}
