using Entities.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Entities.Context
{
    public interface IDbContext : IDisposable
    {
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
    }

    public interface ITulosDbContext : IDbContext
    {
        DbSet<CartItem> CartItems { get; set; }
        DbSet<Favorite> Favorites { get; set; }
    }

    public class TulosDbContext : IdentityDbContext<ApplicationUser>, ITulosDbContext
    {
        private readonly IConfiguration _configuration;

        public TulosDbContext()
        {
        }

        public TulosDbContext(DbContextOptions<TulosDbContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Favorite> Favorites { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Retrieve secrets from configuration
                var userId = _configuration["DbUserId"];
                var password = _configuration["DbPassword"];

                var connectionString = $"Server=tcp:jobnest.database.windows.net,1433;Initial Catalog=TulosDb;Persist Security Info=False;User ID={userId};Password={password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

                optionsBuilder.UseSqlServer(connectionString);
            }
        }
    }

}
