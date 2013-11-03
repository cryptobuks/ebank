using System;
using System.Collections.Generic;
using Domain.Enums;
using Domain.Models.Users;

namespace Domain.Models.Loans
{
    /// <summary>
    /// Класс для представления документа, предъявляемого клиентом
    /// </summary>
    public class Document 
    {
        public Guid Id { get; set; }

        public string Number { get; set; }

        public string CustomerId { get; set; }

        public DocType DocType { get; set; }

        public TariffDocType TariffDocType { get; set; }

        public IEnumerable<LoanApplication> LoanApplications { get; set; }
    }
}
