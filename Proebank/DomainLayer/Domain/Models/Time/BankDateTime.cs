using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Time
{
    public struct BankDateTime
    {

        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }

        //public static BankDateTime ConvertFrom(DateTime date)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
