using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Server.Data;

namespace Server.DB
{
	public class AppDbContext : DbContext
	{
		public DbSet<AccountDb> Accounts { get; set; }
		public DbSet<PlayerDb> Players { get; set; }

        static readonly ILoggerFactory mLogger = LoggerFactory.Create(
            builder => { builder.AddConsole(); });

        string mConnectionString = "Server=localhost;Database=GameDB;User Id=sa;Password=alstlr1278; TrustServerCertificate=True; ApplicationIntent=ReadWrite;";

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options
                .UseLoggerFactory(mLogger)
                .UseSqlServer(ConfigManager.Config == null ? mConnectionString : ConfigManager.Config.connectionString);
        }

        protected override void OnModelCreating(ModelBuilder model)
        {
            model.Entity<AccountDb>()
                .HasIndex(a => a.AccountName)
                .IsUnique();

            model.Entity<PlayerDb>()
                .HasIndex(a => a.PlayerName)
                .IsUnique();
        }
    }
}

