using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete
{
    class EntityFrameworkRepository : IRepository
    {
        public void AddApplication(Abstract.Application application)
        {
            throw new NotImplementedException();
        }

        public Abstract.Application GetApplication(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Abstract.Application> GetApplicationsByFilter(Func<bool, Abstract.Application> filter)
        {
            throw new NotImplementedException();
        }

        public void RemoveApplication(Guid id)
        {
            throw new NotImplementedException();
        }

        public void AddTariff(Abstract.Tariff tariff)
        {
            throw new NotImplementedException();
        }

        public Abstract.Tariff GetTariff(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Abstract.Tariff> GetTariffs(Func<bool, Abstract.Tariff> filter)
        {
            throw new NotImplementedException();
        }

        public void RemoveTariff(int id)
        {
            throw new NotImplementedException();
        }
    }
}
