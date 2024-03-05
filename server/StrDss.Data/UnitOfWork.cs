namespace StrDss.Data
{
    public interface IUnitOfWork
    {
        bool Commit();
    }

    public class UnitOfWork : IUnitOfWork
    {
        //private readonly AppDbContext _dbContext;

        //public UnitOfWork(AppDbContext dbContext)
        //{
        //    _dbContext = dbContext;
        //}

        public bool Commit()
        {
            return true;
            //return _dbContext.SaveChanges() >= 0;
        }
    }
}
