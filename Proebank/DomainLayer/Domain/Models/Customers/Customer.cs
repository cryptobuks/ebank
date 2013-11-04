using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Customers
{
    /// <summary>
    /// Клиент банка
    /// </summary>
    public class Customer
    {
        public Customer()
        {
            Id = Guid.NewGuid();
        }

        public readonly Guid Id;

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public DateTime BirthDate { get; set; }

        public string IdentificationNumber { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }
    }
}
