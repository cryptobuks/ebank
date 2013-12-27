using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contexts.Factories
{
    public interface IDataContextFactory
    {
        DataContext Get();
    }
}
