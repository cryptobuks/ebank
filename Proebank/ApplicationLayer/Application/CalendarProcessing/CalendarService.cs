﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Calendars;
using Infrastructure;

namespace Application.CalendarProcessing
{
    public class CalendarService
    {
        private DataContext Context { get; set; }

        public Calendar Calendar
        {
            get { return Context.Calendars.First(); }
        }

        public CalendarService(DataContext context)
        {
            Context = context;
        }

        // TODO: try TimeSpan if it works well for all time cases
        // TODO: all DateTime.UtcNow should be replaced with exceptions
        public DateTime MoveTime(byte days)
        {
            var currentCalendar = Context.Calendars.First();
            var result = currentCalendar.CurrentTime.HasValue ? currentCalendar.CurrentTime.Value : DateTime.UtcNow;
            if (!currentCalendar.ProcessingLock)
            {
                currentCalendar.ProcessingLock = true;
                Context.Calendars.AddOrUpdate(currentCalendar);
                Context.SaveChanges();
                var calendar2 = Context.Calendars.First();
                if (calendar2.Equals(currentCalendar))
                {
                    calendar2.CurrentTime = calendar2.CurrentTime.HasValue
                        ? calendar2.CurrentTime.Value.AddDays(days)
                        : DateTime.UtcNow;
                    result = calendar2.CurrentTime.Value;
                    calendar2.ProcessingLock = false;
                    // TODO: is it needed to update it explicitly?
                    Context.Calendars.AddOrUpdate(calendar2);
                    Context.SaveChanges();
                }
                else throw new Exception("Calendar is locked");
            }
            return result;
        }

        public void UpdateDailyProcessingTime()
        {
            var currentCalendar = Context.Calendars.First();
            currentCalendar.LastDailyProcessingTime = currentCalendar.CurrentTime;
            Context.Calendars.AddOrUpdate(currentCalendar); // TODO: can it be removed?
            Context.SaveChanges();
        }

        public void UpdateMonthlyProcessingTime()
        {
            var currentCalendar = Context.Calendars.First();
            currentCalendar.LastMonthlyProcessingTime = currentCalendar.CurrentTime;
            Context.Calendars.AddOrUpdate(currentCalendar); // TODO: can it be removed?
            Context.SaveChanges();
        }
    }
}
