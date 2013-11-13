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
        DataContext ctx = new DataContext();

        public Tariff Get(Func<Tariff, bool> filter)
        {
            return ctx.Tariffs
                .AsQueryable()
                .First(filter);
        }

        // TODO: to IQueryable
        public IList<Tariff> GetAll()
        {
            return ctx.Tariffs.ToList();
        }

        public IList<Tariff> GetAll(Func<Tariff, bool> filter)
        {
            return ctx.Tariffs
                .AsQueryable()
                .Where(filter)
                .ToList();
        }

        public void SaveOrUpdate(params Tariff[] entities)
        {
            ctx.Tariffs.AddOrUpdate(entities);
            ctx.SaveChanges();
        }

        public Tariff Delete(Tariff entity)
        {
            var tariffToRemove = ctx.Tariffs.Single(t => t.Id.Equals(entity.Id));
            var removedTariff = ctx.Tariffs.Remove(tariffToRemove);
            ctx.SaveChanges();
            return removedTariff;
        }
    }
}
