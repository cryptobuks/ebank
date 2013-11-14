using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Loans;

namespace Infrastructure.FakeRepositories
{
    class TariffRepository : AbstractRepository<Tariff, int>, ITariffRepository
    {
        public TariffRepository(object _isDisposedIndicator) : base(_isDisposedIndicator) { }
    }
}
