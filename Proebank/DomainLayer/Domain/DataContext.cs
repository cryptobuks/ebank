using System.Data.Entity;
using Domain.Models.Accounts;
using Domain.Models.Calendars;
using Domain.Models.Customers;
using Domain.Models.Loans;
using Domain.Models.Users;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Domain
{
    public class DataContext : AbstractDataContext
    {
        public DataContext() : base("Proebank")
        {
        }
    }
}
