using Entities.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
        public TulosDbContext(DbContextOptions<TulosDbContext> options) : base(options) { }

        public DbSet<CartItem> CartItems { get; set;}
        
        public DbSet<Favorite> Favorites { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(LocalDb)\MSSQLLocalDB;Database=TulosDb;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }
}
