using CrossCutting.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureLayer.Entities
{
    class Document : IDocument
    {
        public Guid Id { get; set; }

        public string Number { get; set; }

        public ICustomer Customer { get; set; }

        public DocType DocType { get; set; }

        public TariffDocType TariffDocType { get; set; }

        public IEnumerable<ILoanApplication> LoanApplications { get; set; }
    }
}
