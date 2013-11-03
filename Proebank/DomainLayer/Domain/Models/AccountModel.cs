using System;
using System.Collections;
using System.Collections.Generic;

namespace Domain.Models
{
    public class AccountModel
    {
        public Guid Id { get; set; }

        public string Number { get; set; }

        //public decimal Balance { get; set; }

        public IList<EntryModel> Entries { get; set; }

        public DateTime CreationDate { get; set; }

        public EmployeeModel Employee { get; set; }
    }
}
