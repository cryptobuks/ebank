using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Models
{
    /// <summary>
    /// Какая-либо операция со счётом (Account): начисление платежей или процентов
    /// </summary>
    public class Entry
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public DateTime Date { get; set; }
        public EntryType Type { get; set; }
        public EntrySubType SubType { get; set; }
    }
}
