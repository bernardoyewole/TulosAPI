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
        private readonly TulosDbContext _context;
        private readonly DbSet<T> _entity;

        public GenericRepository(TulosDbContext context)
        {
            _context = context;
            _entity = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _entity.ToListAsync();
        }

        public async Task<T> GetById(object id)
        {
            return await _entity.FindAsync(id);
        }

        public async Task<Task> Add(T obj)
        {
            if (obj != null)
            {
                _entity.Add(obj);
                await _context.SaveChangesAsync();
                return Task.CompletedTask;
            }

            return null;
        }

        public async Task<Task> Update(T obj)
        {
            if (obj != null)
            {
                _entity.Attach(obj);
                _context.Entry(obj).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Task.CompletedTask;
            }

            return null;
        }

        public async Task<bool> Delete(object id)
        {
            T existingObject = await _entity.FindAsync(id);

            if (existingObject != null)
            {
                _entity.Remove(existingObject);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public void Save()
        {
            _context.SaveChangesAsync();
        }
    }
}
