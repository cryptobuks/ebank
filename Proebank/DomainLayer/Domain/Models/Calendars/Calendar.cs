using System;

namespace Domain.Models.Calendars
{
    public class Calendar
    {
        // TODO: hardcode Id
        public Guid Id { get; set; }

        public DateTime? CurrentTime { get; set; }

        public DateTime? LastDailyProcessingTime { get; set; }

        public DateTime? LastMonthlyProcessingTime { get; set; }

        public bool ProcessingLock { get; set; }
    }
}
