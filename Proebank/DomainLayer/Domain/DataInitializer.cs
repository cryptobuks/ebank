using System;
using System.Configuration;
using System.Data.Entity;
using Domain.Migrations;

namespace Domain
{
    public class DataInitializer : MigrateDatabaseToLatestVersion<DataContext, Configuration>
    {
        //public new void InitializeDatabase(DataContext context)
        //{
        //    if (!context.Database.Exists())
        //    {
        //        throw new ConfigurationException(
        //          "Database does not exist");
        //    }
        //    else
        //    {
        //        if (!context.Database.CompatibleWithModel(true))
        //        {
        //            throw new InvalidOperationException(
        //              "The database is not compatible with the entity model.");
        //        }
        //    }
        //    base.InitializeDatabase(context);
        //}
    }
}
