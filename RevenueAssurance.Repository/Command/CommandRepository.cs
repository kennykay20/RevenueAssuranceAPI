using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using anythingGoodApi.AnythingGood.DATA;
using Microsoft.EntityFrameworkCore;
using RevAssuranceApi.RevenueAssurance.DATA.Models;

namespace RevAssuranceApi.RevenueAssurance.Repository.Command
{
         public class CommandRepository<TEntity> : ICommandRepository<TEntity> where TEntity : class
     {
        private ApplicationDbContext _context;
        private DbSet<TEntity> _dbSet;
        public CommandRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public void Insert(TEntity entity)
        {
            _dbSet.Add(entity);

           
        }

        public void InsertRange(List<TEntity> entity) 
        {
            _dbSet.AddRange(entity);
        }
        public void Update(TEntity entity)
        {
            if (entity != null)
            {
                _context.Entry(entity).State = EntityState.Detached;
            }
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            _dbSet.Remove(entity);
        }
        public void Delete(Expression<Func<TEntity, bool>> where)
        {
            if (where == null)
            {
                throw new ArgumentNullException("entity");
            }
            IEnumerable<TEntity> objects = _dbSet.Where<TEntity>(where).AsEnumerable();
            foreach (TEntity obj in objects)
                _dbSet.Remove(obj);
        }
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_context != null)
                {
                    _context.Dispose();
                    _context = null;
                }
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // public int Save()
        // {
        //    return _context.SaveChanges();
        // }
    }


}