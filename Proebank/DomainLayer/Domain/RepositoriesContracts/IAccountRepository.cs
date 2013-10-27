using CrossCutting.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.RepositoriesContracts
{
    public interface IAccountRepository : IRepository<IAccount, Guid>
    {
    }
}
