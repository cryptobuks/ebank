using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Calendars;

namespace Infrastructure.FakeRepositories
{
    public class CalendarRepository : AbstractRepository<Calendar, Guid>, ICalendarRepository
    {
        public CalendarRepository(object isDisposedIndicator) : base(isDisposedIndicator) { }
    }
}
