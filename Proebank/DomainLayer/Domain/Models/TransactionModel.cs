using System;

namespace Domain.Models
{
    public class TransactionModel
    {
        public Guid Id { get; set; }

        public string Number { get; set; }

        public decimal Balance { get; set; }

        public DateTime CreationDate { get; set; }

        public EmployeeModel Employee { get; set; }
    }
}
