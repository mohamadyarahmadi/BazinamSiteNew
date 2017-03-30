using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using EFSecondLevelCache;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Bazinam.DomainClasses;


namespace Bazinam.DataAccessLayer
{
    public class ApplicationDbContext :
        IdentityDbContext<ApplicationUser, CustomRole, int, CustomUserLogin, CustomUserRole, CustomUserClaim>,
        IUnitOfWork
    {

        public DbSet<News> News { get; set; }
        public DbSet<Picture>   Pictures  { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public ApplicationDbContext()
            : base("SystemDbContext")
        {
            //this.Database.Log = data => System.Diagnostics.Debug.WriteLine(data);
        }
        protected override void OnModelCreating(DbModelBuilder builder)
        {
            //builder.Configurations.Add(new ProductConfig());
           
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<CustomRole>().ToTable("Roles");
            builder.Entity<CustomUserClaim>().ToTable("UserClaims");
            builder.Entity<CustomUserRole>().ToTable("UserRoles");
            builder.Entity<CustomUserLogin>().ToTable("UserLogins");


        }
        public new IDbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }

        

        public int SaveAllChanges(bool invalidateCacheDependencies)
        {
            return SaveChanges(invalidateCacheDependencies);
        }

        public async Task<int> SaveAllChangesAsync(bool invalidateCacheDependencies)
        {
            
            return await SaveChangesAsync(invalidateCacheDependencies);
        }

        public int SaveChanges(bool invalidateCacheDependencies)
        {
            var changedEntityNames = this.GetChangedEntityNames();
            var result = base.SaveChanges();
            if (invalidateCacheDependencies)
            {
                new EFCacheServiceProvider().InvalidateCacheDependencies(changedEntityNames);
            }
            return result;
        }

        public async Task<int> SaveChangesAsync(bool invalidateCacheDependencies)
        {
            var changedEntityNames = this.GetChangedEntityNames();
            var result = await base.SaveChangesAsync();
            if (invalidateCacheDependencies)
            {
                new EFCacheServiceProvider().InvalidateCacheDependencies(changedEntityNames);
            }
            return result;
        }
        public void MarkAsAdded<TEntity>(TEntity entity) where TEntity : class
        {
            Entry(entity).State = EntityState.Added;
        }
        public void MarkAsDeleted<TEntity>(TEntity entity) where TEntity : class
        {
            Entry(entity).State = EntityState.Deleted;
        }
        public async Task<TEntity> FindAsyncc<TEntity>(int id) where TEntity : class
        {
            return await ((DbSet<TEntity>)this.Set<TEntity>()).FindAsync(id);
        }
        public IEnumerable<TEntity> AddThisRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            return ((DbSet<TEntity>)this.Set<TEntity>()).AddRange(entities);
        }

        public void MarkAsChanged<TEntity>(TEntity entity) where TEntity : class
        {
            Entry(entity).State = EntityState.Modified;
        }

        public IList<T> GetRows<T>(string sql, params object[] parameters) where T : class
        {
            return Database.SqlQuery<T>(sql, parameters).ToList();
        }

        public void ForceDatabaseInitialize()
        {
            this.Database.Initialize(force: true);
        }
    }
}
