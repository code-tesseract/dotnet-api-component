using Component.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Component.Helpers;

public static class DatabaseHelper
{
    private const string DriverSqlServer = "MSSQL";
    private const string DriverMySql = "MYSQL";

    public static void GetDbContextOptionsBuilder(
        this DbContextOptionsBuilder optionsBuilder, IConfiguration conf, string sectionName)
    {
        var dbSetting = conf.GetDatabaseSettingFromSection(sectionName);
        var connectionString = dbSetting.GenerateConnectionString();

        switch (dbSetting.Driver.ToUpper())
        {
            case DriverSqlServer:
                optionsBuilder.UseSqlServer(connectionString);
                break;
            case DriverMySql:
                optionsBuilder.UseMySQL(connectionString);
                break;
            default: throw new Exception($"Invalid `{dbSetting.Driver}` as database driver");
        }
    }

    private static BaseDatabaseSetting GetDatabaseSettingFromSection(this IConfiguration conf, string sectionName)
    {
        try
        {
            var section = conf.GetSection("DatabaseSettings").GetSection(sectionName);
            if (section == null) throw new Exception("Invalid database setting section.");
            var dbSetting = section.Get<BaseDatabaseSetting>();
            if (dbSetting == null) throw new Exception("Invalid database setting.");
            return dbSetting;
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(e.Message);
            Console.ResetColor();
            throw;
        }
    }

    private static string GenerateConnectionString(this BaseDatabaseSetting dbSetting)
    {
        return $"Data Source={dbSetting.InstanceName};" +
               $"Initial Catalog={dbSetting.DatabaseName};" +
               $"Integrated Security={dbSetting.IntegratedSecurity.ToString().ToTitleCase()};" +
               $"TrustServerCertificate={dbSetting.TrustServerCertificate.ToString().ToTitleCase()};" +
               $"MultipleActiveResultSets={dbSetting.MultipleActiveResultSets.ToString().ToTitleCase()};" +
               $"User id={dbSetting.Username};" +
               $"Password={dbSetting.Password}";
        // $"Collation={dbSetting.CollationType}";
    }
}