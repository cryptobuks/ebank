using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCutting.Interfaces
{
    public interface ILoanApplication
    {
        /// <summary>
        /// Application identity
        /// </summary>
        long Id
        {
            get;
            set;
        }

        /// <summary>
        /// Запрашиваемый размер кредита
        /// </summary>
        decimal LoanAmount
        {
            get;
            set;
        }

        DateTime TimeCreated
        {
            get;
            set;
        }

        /// <summary>
        /// Срок, на который запрашивается кредит (в месяцах)
        /// </summary>
        int Term
        {
            get;
            set;
        }

        ITariff Tariff
        {
            get;
            set;
        }

        LoanPurpose LoanPurpose
        {
            get;
            set;
        }

        IEnumerable<IDocument> Documents
        {
            get;
            set;
        }
    }
}
