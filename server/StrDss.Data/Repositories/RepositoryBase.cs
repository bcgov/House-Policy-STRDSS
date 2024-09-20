using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data.Entities;
using StrDss.Model;
using System.Diagnostics;

namespace StrDss.Data.Repositories
{
    public interface IRepositoryBase<TEntity>
        where TEntity : class
    {
        Task<PagedDto<TOutput>> Page<TInput, TOutput>(IQueryable<TInput> list, int pageSize, int pageNumber, string orderBy, string direction, string extraSort = "");
    }
    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity>
        where TEntity : class
    {
        protected DbSet<TEntity> _dbSet { get; private set; }

        protected ICurrentUser _currentUser;
        protected ILogger<StrDssLogger> _logger;

        protected DssDbContext _dbContext { get; private set; }

        protected IMapper _mapper { get; private set; }

        public RepositoryBase(DssDbContext dbContext, IMapper mapper, ICurrentUser currentUser, ILogger<StrDssLogger> logger)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<TEntity>();
            _currentUser = currentUser;
            _logger = logger;
        }

        public async Task<PagedDto<TOutput>> Page<TInput, TOutput>(IQueryable<TInput> list, int pageSize, int pageNumber, string orderBy, string direction = "", string extraSort = "")
        {
            var stopwatch = Stopwatch.StartNew();

            var sort = string.IsNullOrEmpty(extraSort)
                ? $"{orderBy} {direction}"
                : $"{orderBy} {direction}, {extraSort}";

            var pagedList = list.DynamicOrderBy(sort) as IQueryable<TInput>;

            if (pageSize > 0)
            {
                var skipRecordCount = (pageNumber - 1) * pageSize;
                pagedList = pagedList.Skip(skipRecordCount).Take(pageSize);
            }

            // Run the counting and fetching data in parallel
            var countTask = list.CountAsync();
            var fetchTask = pagedList.ToListAsync();

            // Await both tasks in parallel
            await Task.WhenAll(countTask, fetchTask);

            var totalRecords = countTask.Result;
            var result = fetchTask.Result;

            stopwatch.Stop();
            _logger.LogDebug($"Mapping groups to DTO. Time: {stopwatch.Elapsed.TotalSeconds} seconds");

            IEnumerable<TOutput> outputList;

            // Map the result if necessary
            if (typeof(TOutput) != typeof(TInput))
                outputList = _mapper.Map<IEnumerable<TInput>, IEnumerable<TOutput>>(result);
            else
                outputList = (IEnumerable<TOutput>)result;

            return new PagedDto<TOutput>
            {
                SourceList = outputList,
                PageInfo = new PageInfo
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalRecords,
                    OrderBy = orderBy,
                    Direction = direction,
                    ItemCount = outputList.Count()
                }
            };
        }

    }
}
