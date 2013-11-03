using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrossCutting.Enums;

namespace Domain.Models
{
    public class EntryModel
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public EntryType Type { get; set; }
        public EntrySubType SubType { get; set; }
    }
}
