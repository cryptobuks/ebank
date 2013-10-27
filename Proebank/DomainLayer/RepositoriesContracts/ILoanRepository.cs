using System;
using CrossCutting.Interfaces;

namespace RepositoriesContracts
{
    interface ILoanRepository : IRepository<ILoan, Guid>
    {
    }
}
