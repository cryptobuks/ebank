using CrossCutting.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureLayer.Entities
{
    class LoanApplication : ILoanApplication
    {
        public long Id { get; set; }

        public decimal LoanAmount { get; set; }

        public DateTime TimeCreated { get; set; }

        public int Term { get; set; }

        public ITariff Tariff { get; set; }

        public LoanPurpose LoanPurpose { get; set; }

        public IEnumerable<IDocument> Documents { get; set; }
    }
}
