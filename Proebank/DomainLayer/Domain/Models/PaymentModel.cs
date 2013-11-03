using System;
using CrossCutting.Enums;

namespace Domain.Models
{
    public class PaymentModel
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public EntryType PaymentType { get; set; }
    }
}
