using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Users;

namespace Domain.Models.Employees
{
    interface IEmployeeRepository : IRepository<Employee, string>
    {
    }
}
