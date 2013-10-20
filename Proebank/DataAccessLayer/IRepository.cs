using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Abstract;

namespace DataAccessLayer
{
    public interface IRepository
    {
        void AddApplication(Application application);
        Application GetApplication(Guid id);
        IEnumerable<Application> GetApplicationsByFilter(Func<bool, Application> filter);
        void RemoveApplication(Guid id);

        void AddTariff(Tariff tariff);
        Tariff GetTariff(int id);
        IEnumerable<Tariff> GetTariffs(Func<bool, Tariff> filter);
        void RemoveTariff(int id);
    }
}
