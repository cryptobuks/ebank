using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Calendars;

namespace Infrastructure.Repositories
{
    public class CalendarRepository : ICalendarRepository
    {
        private readonly DataContext _context;

        public CalendarRepository(DataContext context)
        {
            _context = context;
        }

        public Calendar Get(Func<Calendar, bool> filter)
        {
            return _context.Calendars
                .AsQueryable()
                .First(filter);
        }

        public IQueryable<Calendar> GetAll()
        {
            return _context.Calendars.AsQueryable();
        }

        public IEnumerable<Calendar> GetAll(Func<Calendar, bool> filter)
        {
            return _context.Calendars
                .AsQueryable()
                .Where(filter);
        }

        public void Upsert(params Calendar[] entities)
        {
            _context.Calendars.AddOrUpdate(entities);
        }

        public Calendar Delete(Calendar entity)
        {
            throw new NotImplementedException("Calendar removing is not allowed");
        }
    }
}
