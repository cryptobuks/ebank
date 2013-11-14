using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Domain.Models.Accounts;

namespace Infrastructure.FakeRepositories
{
    class AccountRepository : AbstractRepository<Account, Guid>, IAccountRepository
    {
        public AccountRepository(object _isDisposedIndicator) : base(_isDisposedIndicator) { }
    }
}
