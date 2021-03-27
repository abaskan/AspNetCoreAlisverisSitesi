using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using shoppingApp.DataAccess.Abstract;
using System.Threading.Tasks;

namespace shoppingApp.DataAccess.Concrete.EFCore
{
    public class EfCoreGenericRepository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        protected readonly DbContext context;

        public EfCoreGenericRepository(DbContext dbContext)
        {
            context = dbContext;
        }

        public void Create(TEntity entity)
        {
            //context.Products.Add(entity);
            context.Set<TEntity>().Add(entity);
            context.SaveChanges();
        }

        public async Task CreateAsync(TEntity entity)
        {
            await context.Set<TEntity>().AddAsync(entity);
        }

        public void Delete(TEntity entity)
        {
            
            context.Set<TEntity>().Remove(entity);
            context.SaveChanges();
            
        }

        public async Task<List<TEntity>> GetAll()
        {
            return await context.Set<TEntity>().ToListAsync();
        }

        public async Task<TEntity> GetById(int id)
        {
            return await context.Set<TEntity>().FindAsync(id);
        }

        public virtual void Update(TEntity entity)
        {
            context.Entry(entity).State = EntityState.Modified;
            context.SaveChanges();
        }
    }
}