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
        private DataContext _context;

        public TariffRepository(DataContext context)
        {
            // TODO: Complete member initialization
            this._context = context;
        }

        public Tariff Get(Func<Tariff, bool> filter)
        {
            return _context.Tariffs
                .AsQueryable()
                .First(filter);
        }

        public IQueryable<Tariff> GetAll()
        {
            return _context.Tariffs.AsQueryable();
        }

        public IEnumerable<Tariff> GetAll(Func<Tariff, bool> filter)
        {
            return _context.Tariffs
                .AsQueryable()
                .Where(filter);
        }

        public void Upsert(params Tariff[] entities)
        {
            _context.Tariffs.AddOrUpdate(entities);
        }

        public Tariff Delete(Tariff entity)
        {
            // TODO: refactor to remove entity or (preferrably) as in
            // http://www.asp.net/mvc/tutorials/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application
            var tariffToRemove = _context.Tariffs.Single(t => t.Id.Equals(entity.Id));
            return _context.Tariffs.Remove(tariffToRemove);
        }
    }
}
