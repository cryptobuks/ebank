using System;
using DomainLayer.Models;

namespace RepositoriesContracts
{
    public interface IAccountRepository : IRepository<AccountModel, Guid>
    {
        // TODO: declare specific functions for Accounts
    }
}
