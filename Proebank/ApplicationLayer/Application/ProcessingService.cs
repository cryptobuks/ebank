using System.Collections.Generic;

namespace Application
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
