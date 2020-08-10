using Microsoft.EntityFrameworkCore;


namespace HealthCheckHostServer.Core.Data
{
    class HealthChecksDb : DbContext
    {
        public DbSet<HealthCheckConfiguration> Configurations { get; set; }
        public DbSet<HealthCheckExecution> Executions { get; set; }
        public DbSet<HealthCheckFailureNotification> Failures { get; set; }

        public DbSet<HealthCheckUIParameter> HealthCheckUIParameter { get; set; }
        public DbSet<HealthCheckExecutionEntry> HealthCheckExecutionEntries { get; set; }
        public DbSet<HealthCheckParameter> HealthCheckParameters { get; set; }
        public HealthChecksDb(DbContextOptions<HealthChecksDb> options) : base(options) { }

        public DbSet<HealthCheckExecutionHistory> HealthCheckExecutionHistories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=DBA-USYS;Initial Catalog=LionExAPI_developers;Persist Security Info=True;User ID=exapidev;Password=exapidev#621;Application Name=Lion.ExAPI.Utility.Hangfire.Core;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) { }
    }
}
