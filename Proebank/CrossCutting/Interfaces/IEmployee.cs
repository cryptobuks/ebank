using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCutting.Interfaces
{
    public interface IEmployee
    {
        object Id
        {
            get;
            set;
        }

        object FirstName
        {
            get;
            set;
        }

        object LastName
        {
            get;
            set;
        }

        object MiddleName
        {
            get;
            set;
        }

        object HiredOn
        {
            get;
            set;
        }

        object FiredOn
        {
            get;
            set;
        }

        EmployeeRole EmployeeRole
        {
            get;
            set;
        }
    }
}
