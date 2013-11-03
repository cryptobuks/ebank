using System;
using Domain.Enums;

namespace Domain.Models
{
    public class Payment
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public EntryType PaymentType { get; set; }
    }
}
