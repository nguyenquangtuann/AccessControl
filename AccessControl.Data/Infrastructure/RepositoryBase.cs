using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Data.Infrastructure
{
    public abstract class RepositoryBase<T> : IRepository<T> where T : class
    {
        private ACSDBContext dataContext;

        protected RepositoryBase(ACSDBContext dbContext)
        {
            dataContext = dbContext;
        }

        // protected ProjectDbContext DbContext => dataContext ?? (dataContext = DbFactory());

        public async Task<T> AddASync(T entity)
        {
            dataContext.Set<T>().Add(entity);
            dataContext.Entry(entity).State = EntityState.Added;
            await dataContext.SaveChangesAsync();
            return entity;
        }
        public bool CheckContains(Expression<Func<T, bool>> predicate)
        {
            return dataContext.Set<T>().Count(predicate) > 0;
        }

        public async Task<bool> CheckContainsAsync(Expression<Func<T, bool>> predicate)
        {
            return await dataContext.Set<T>().CountAsync(predicate) > 0;
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> where)
        {
            return await dataContext.Set<T>().CountAsync(where);
        }

        public async Task<T> DeleteAsync(int id)
        {
            var entity = await dataContext.Set<T>().FindAsync(id);
            if (entity == null)
            {
                return entity;
            }

            dataContext.Set<T>().Remove(entity);
            dataContext.Entry(entity).State = EntityState.Deleted;
            await dataContext.SaveChangesAsync();

            return entity;
        }

        public async Task<T> DeleteAsync(string id)
        {
            var entity = await dataContext.Set<T>().FindAsync(id);
            if (entity == null)
            {
                return entity;
            }

            dataContext.Set<T>().Remove(entity);
            dataContext.Entry(entity).State = EntityState.Deleted;
            await dataContext.SaveChangesAsync();

            return entity;
        }

        public async Task DeleteMulti(Expression<Func<T, bool>> where)
        {
            IEnumerable<T> objects = dataContext.Set<T>().Where(where).AsEnumerable();
            foreach (T obj in objects)
                dataContext.Remove(obj);
            await dataContext.SaveChangesAsync();
        }

        public async Task<IQueryable<T>> GetAllAsync(string[] includes = null)
        {
            var query = await dataContext.Set<T>().ToListAsync();
            return query.AsQueryable();
        }

        public async Task<IQueryable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, string[] includes = null)
        {
            if (includes != null && includes.Count() > 0)
            {
                var query = dataContext.Set<T>().Include(includes.First());
                foreach (var include in includes.Skip(1))
                {
                    query = query.Include(include);
                }
                var query1 = await query.Where(predicate).ToListAsync();
                return query1.AsQueryable();
            }
            var query2 = await dataContext.Set<T>().Where(predicate).ToListAsync();
            return query2.AsQueryable();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await dataContext.Set<T>().FindAsync(id);
        }

        public async Task<T> GetByIdAsync(string id)
        {
            return await dataContext.Set<T>().FindAsync(id);
        }

        public async Task<T> GetSingleByConditionAsync(Expression<Func<T, bool>> expression, string[] includes = null)
        {
            if (includes != null && includes.Count() > 0)
            {
                var query = dataContext.Set<T>().Include(includes.First());
                foreach (var include in includes.Skip(1))
                    query = query.Include(include);
                return await query.FirstOrDefaultAsync(expression);
            }
            return await dataContext.Set<T>().FirstOrDefaultAsync(expression);
        }

        public async Task<T> UpdateASync(T entity)
        {
            dataContext.Set<T>().Attach(entity);
            dataContext.Entry(entity).State = EntityState.Modified;
            await dataContext.SaveChangesAsync();
            return entity;
        }
    }
}
