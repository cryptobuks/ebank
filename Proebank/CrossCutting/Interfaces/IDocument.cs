using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCutting.Interfaces
{
    public interface IDocument
    {
        Guid Id { get; set; }

        string Number { get; set; }

        ICustomer Customer { get; set; }

        DocType DocType { get; set; }

        TariffDocType TariffDocType { get; set; }

        IEnumerable<ILoanApplication> LoanApplications { get; set; }
    }
}
