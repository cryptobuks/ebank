using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
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
            using (var ctx = new DataContext())
            {
                return ctx.Tariffs
                    .First(a => filter(a));
            }
        }

        // TODO: to IQueryable
        public IList<Tariff> GetAll()
        {
            using (var ctx = new DataContext())
            {
                return ctx.Tariffs.ToList();
            }
        }

        public IList<Tariff> GetAll(Func<Tariff, bool> filter)
        {
            using (var ctx = new DataContext())
            {
                return ctx.Tariffs
                    .Where(loan => filter(loan))
                    .ToList();
            }
        }

        public void SaveOrUpdate(params Tariff[] entities)
        {
            using (var ctx = new DataContext())
            {
                ctx.Tariffs.AddOrUpdate(entities);
                ctx.SaveChanges();
            }
        }

        public Tariff Delete(Tariff entity)
        {
            using (var ctx = new DataContext())
            {
                var removedTariff = ctx.Tariffs.Remove(entity);
                ctx.SaveChanges();
                return removedTariff;
            }
        }
    }
}
