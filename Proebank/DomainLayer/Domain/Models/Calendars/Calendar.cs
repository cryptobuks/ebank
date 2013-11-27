using System;

namespace Domain.Models.Calendars
{
    public class Calendar
    {
        // as we have only one record, we can hardcode id for it
        // TODO: should we set it in constructor?
        public static Guid ConstGuid { get { return Guid.Parse("15AB1FE1-081D-440A-BD73-9DEBF4976084"); } }

        public Guid Id { get; set; }

        public DateTime? CurrentTime { get; set; }

        public DateTime? LastDailyProcessingTime { get; set; }

        public DateTime? LastMonthlyProcessingTime { get; set; }

        public bool ProcessingLock { get; set; }
    }
}
