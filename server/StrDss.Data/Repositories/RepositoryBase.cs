﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data.Entities;
using StrDss.Model;

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
            var totalRecords = list.Count();

            if (pageNumber <= 0) pageNumber = 1;

            var sort = "";

            if (extraSort.IsEmpty())
            {
                sort = $"{orderBy} {direction}";
            }
            else if (orderBy.IsNotEmpty())
            {
                sort = $"{orderBy} {direction}, {extraSort}";
            }
            else 
            {
                sort = $"{extraSort}";
            }

            var pagedList = list.DynamicOrderBy($"{sort}") as IQueryable<TInput>;

            if (pageSize > 0)
            {
                var skipRecordCount = (pageNumber - 1) * pageSize;
                pagedList = pagedList.Skip(skipRecordCount)
                    .Take(pageSize);
            }

            var result = await pagedList.ToListAsync();

            IEnumerable<TOutput> outputList;

            if (typeof(TOutput) != typeof(TInput))
                outputList = _mapper.Map<IEnumerable<TInput>, IEnumerable<TOutput>>(result);
            else
                outputList = (IEnumerable<TOutput>)result;

            var pagedDTO = new PagedDto<TOutput>
            {
                SourceList = outputList,
                PageInfo = new PageInfo {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalRecords,
                    OrderBy = orderBy,
                    Direction = direction,
                    ItemCount = outputList.Count()
                }
            };

            return pagedDTO;
        }
    }
}
