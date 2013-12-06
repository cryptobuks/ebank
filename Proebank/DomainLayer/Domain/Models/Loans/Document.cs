using System;
using System.Collections.Generic;
using Domain.Enums;
using Domain.Models.Customers;

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

        public Guid Id { get; set; }

        public string Number { get; set; }

        public string CustomerId { get; set; }

        public DocType DocType { get; set; }

        public TariffDocType TariffDocType { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual IEnumerable<LoanApplication> LoanApplications { get; set; }
    }
}
