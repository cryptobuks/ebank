using System;

namespace Domain.Models.Calendars
{
    public class Calendar : Entity
    {
        // as we have only one record, we can hardcode id for it
        public static Guid ConstGuid { get { return Guid.Parse("15AB1FE1-081D-440A-BD73-9DEBF4976084"); } }

        public DateTime? CurrentTime { get; set; }

        public DateTime? LastDailyProcessingDate { get; set; }

        public DateTime? LastMonthlyProcessingDate { get; set; }

        public DateTime? NextMonthlyProcessingDate { get; set; }

        public bool ProcessingLock { get; set; }
    }
}
