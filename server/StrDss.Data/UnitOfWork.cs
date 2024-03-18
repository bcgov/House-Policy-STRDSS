using StrDss.Data.Entities;

namespace StrDss.Data
{
    public interface IUnitOfWork
    {
        bool Commit();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly DssDbContext _dbContext;

        public UnitOfWork(DssDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool Commit()
        {
            return _dbContext.SaveChanges() >= 0;
        }
    }
}
