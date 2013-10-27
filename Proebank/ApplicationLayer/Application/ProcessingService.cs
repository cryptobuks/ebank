using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer
{
    public static class ProcessingService
    {
        public static IEnumerable<string> GetCustomers()
        {
            return new[] { "One", "Two", "Three" };
        }

        public static string GetCustomer(int id)
        {
            return (id / 2 + 5).ToString(); 
        }
    }
}
