using System;
using System.Configuration;
using System.Data.Entity;
using Domain.Contexts;
using Domain.Migrations;

namespace Domain
{
    public class DataInitializer : MigrateDatabaseToLatestVersion<DbDataContext, Configuration>
    {
    }
}
