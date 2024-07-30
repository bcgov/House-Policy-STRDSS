
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace DataBase.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private DbContext _Context;
        internal DbSet<TEntity> _DbSet;

        internal DbContext Context { get => _Context; }

        public GenericRepository(DbContext context)
        {
            _Context = context;
            _DbSet = context.Set<TEntity>();
        }

        public virtual IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = _DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null) return orderBy(query).ToList();

            List<TEntity> list = new List<TEntity>();

            list = query.ToList();

            return (list);
        }

        public virtual TEntity GetOrInsert(TEntity entity, bool updateReferenceData, Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            IEnumerable<TEntity> query = null;

            try
            {
                query = Get(filter, orderBy, includeProperties);
            }
            catch (Exception ex)
            {
                throw new Exception("Query to retrieve item from database threw an exception", ex);
            }


            if (query.Count() == 0)
            {
                if (updateReferenceData)
                {
                    try
                    {
                        return (Insert(entity));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Cannot insert row into table", ex);
                    }
                }
                else
                {
                    //serial entity to xml and use instead of filter in error message
                    throw new ApplicationException("Not updating data for current table and cannot get row from database for query (" + filter.ToString() + ")");
                }

            }

            return query.Single();
        }


        public virtual TEntity GetByID(object id)
        {
            TEntity result = null;
            try
            {
                result = _DbSet.Find(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return (result);
        }

        public virtual TEntity Insert(TEntity entity)
        {
            return (_DbSet.Add(entity).Entity);
        }

        public virtual void Delete(object id)
        {
            var entityToDelete = _DbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            //Don't change state, just load entity
            //if (_Context.Entry(entityToDelete).State == EntityState.Detached)
            //    _DbSet.Attach(entityToDelete);

            _DbSet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            _DbSet.Attach(entityToUpdate);
            _Context.Entry(entityToUpdate).State = EntityState.Modified;
        }
    }
}
