using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace DataAccessLayer.Concrete
{
    class ProebankContext : DbContext
    {
        public DbSet<Tariff> Tariffs { get; set; }
        public DbSet<Application> Applications { get; set; }
    }
}
