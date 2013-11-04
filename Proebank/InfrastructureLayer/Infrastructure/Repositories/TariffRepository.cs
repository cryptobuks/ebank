using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Loans;

namespace Infrastructure.Repositories
{
    public class TariffRepository : ITariffRepository
    {
        public Tariff Get(Func<Tariff, bool> filter)
        {
            throw new NotImplementedException();
        }

        public IList<Tariff> GetAll()
        {
            throw new NotImplementedException();
        }

        public IList<Tariff> GetAll(Func<Tariff, bool> filter)
        {
            throw new NotImplementedException();
        }

        public void SaveOrUpdate(params Tariff[] entities)
        {
            throw new NotImplementedException();
        }

        public Tariff Delete(Tariff entity)
        {
            throw new NotImplementedException();
        }
    }
}
