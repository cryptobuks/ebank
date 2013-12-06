using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Migrations;

namespace Domain
{
    public class DataInitializer : MigrateDatabaseToLatestVersion<DataContext, Configuration>
    {
    }
}
