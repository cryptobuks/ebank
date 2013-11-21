using System;

namespace Domain.Models.Calendars
{
    public interface ICalendarRepository : IRepository<Calendar, Guid>
    {
    }
}
