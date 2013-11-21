using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Domain.Models.Accounts;

namespace Infrastructure.FakeRepositories
{
    public class AccountRepository : AbstractRepository<Account, Guid>, IAccountRepository
    {
        public AccountRepository(object isDisposedIndicator) : base(isDisposedIndicator) { }
    }
}
