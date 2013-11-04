using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;
using Domain.Models.Customers;
using Domain.Models.Users;

namespace Domain.Models.Loans
{
    /// <summary>
    /// Класс для представления документа, предъявляемого клиентом
    /// </summary>
    public class Document 
    {
        public Document()
        {
            Id = Guid.NewGuid();
        }

        public readonly Guid Id;

        public string Number { get; set; }

        public Guid CustomerId { get; set; }

        public DocType DocType { get; set; }

        public TariffDocType TariffDocType { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual IEnumerable<LoanApplication> LoanApplications { get; set; }
    }
}
