using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Time
{
    // TODO: add to context
    public class BankCalendarEntry
    {
        public int Id { get; set; }
        public DateTime RealDate { get; set; }
        public BankDateTime BankDate { get; set; }
    }
}
