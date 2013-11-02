using System;

namespace DomainLayer.Models
{
    public class PaymentModel
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public PaymentType PaymentType { get; set; }
    }
}
