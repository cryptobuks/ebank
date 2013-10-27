using System;
using CrossCutting.Interfaces;

namespace RepositoriesContracts
{
    public interface IAccountRepository : IRepository<IAccount, Guid>
    {
    }
}
