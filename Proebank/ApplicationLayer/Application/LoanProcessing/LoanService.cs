using System.Collections.Generic;
using Domain.Models;

namespace Application.LoanProcessing
{
    class LoanService
    {

        public IEnumerable<LoanModel> Loans
        {
            get;
            set;
        }

        public void CreateLoanContract()
        {
            throw new System.NotImplementedException();
        }

        public void CloseLoanContract()
        {
            throw new System.NotImplementedException();
        }

        public void RegisterPayment()
        {
            throw new System.NotImplementedException();
        }
    }
}
