using System;
using System.Collections.Generic;

namespace DomainLayer.Models
{
    public class DocumentModel
    {
        public Guid Id { get; set; }

        public string Number { get; set; }

        public CustomerModel Customer { get; set; }

        public DocType DocType { get; set; }

        public TariffDocType TariffDocType { get; set; }

        public IEnumerable<LoanApplicationModel> LoanApplications { get; set; }
    }
}
