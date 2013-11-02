using System;

namespace DomainLayer.Models
{
    public class AccountModel
    {
        public Guid Id { get; set; }

        public string Number { get; set; }

        public decimal Balance { get; set; }

        public DateTime CreationDate { get; set; }

        public EmployeeModel Employee { get; set; }
    }
}
