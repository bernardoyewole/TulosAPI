using Entities.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private TulosDbContext context;
        private DbSet<T> entity;

        public GenericRepository()
        {
            context = new TulosDbContext();
            entity = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await entity.ToListAsync();
        }

        public async Task<T> GetById(object id)
        {
            return await entity.FindAsync(id);
        }

        public Task Add(T obj)
        {
            if (obj != null)
            {
                entity.Add(obj);
                context.SaveChangesAsync();
                return Task.CompletedTask;
            }

            return null;
        }

        public Task Update(T obj)
        {
            if (obj != null)
            {
                entity.Attach(obj);
                context.Entry(obj).State = EntityState.Modified;
                context.SaveChangesAsync();
                return Task.CompletedTask;
            }

            return null;
        }

        public async Task<bool> Delete(object id)
        {
            T existingObject = await entity.FindAsync(id);

            if (existingObject != null)
            {
                entity.Remove(existingObject);
                context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public void Save()
        {
            context.SaveChangesAsync();
        }
    }
}
