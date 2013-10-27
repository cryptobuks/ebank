using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCutting.Interfaces
{
    public interface ITariff
    {
        int Id { get; set; }

        string Name { get; set; }

        decimal InterestRate { get; set; }

        decimal MinAmount { get; set; }

        decimal MaxAmount { get; set; }

        DateTime CreationDate { get; set; }

        DateTime EndDate { get; set; }
        int MinTerm { get; set; }

        int MaxTerm { get; set; }

        ushort MinAge { get; set; }

        ushort MaxAge { get; set; }

        decimal InitialFee { get; set; }

        bool IsGuarantorNeeded { get; set; }

        bool IsSecondaryDocumentNeeded { get; set; }

        LoanPurpose LoanPurpose { get; set; }
    }
}
