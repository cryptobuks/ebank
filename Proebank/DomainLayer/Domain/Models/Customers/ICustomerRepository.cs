using System;

namespace Domain.Models.Customers
{
    public interface ICustomerRepository : IRepository<Customer, Guid>
    {
    }
}
