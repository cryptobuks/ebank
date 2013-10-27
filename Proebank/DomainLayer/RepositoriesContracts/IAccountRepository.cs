using System;
using CrossCutting.Interfaces;

namespace RepositoriesContracts
{
    public interface IAccountRepository : IRepository<IAccount, Guid>
    {
        // TODO: replace number with account type!
        IAccount Build(string number, IEmployee employee);
    }
}
