using System;
using System.Collections.Generic;
using Domain.Enums;

namespace Domain.Models
{
    /// <summary>
    /// Класс для представления документа, предъявляемого клиентом
    /// </summary>
    public class Document
    {
        public Guid Id { get; set; }

        public string Number { get; set; }

        public Customer Customer { get; set; }

        public DocType DocType { get; set; }

        public TariffDocType TariffDocType { get; set; }

        public IEnumerable<LoanApplication> LoanApplications { get; set; }
    }
}
