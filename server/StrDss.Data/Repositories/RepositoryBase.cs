﻿using AutoMapper;
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
        Task<PagedDto<TOutput>> Page<TInput, TOutput>(IQueryable<TInput> list, int pageSize, int pageNumber, string orderBy, string direction, string extraSort = "", bool count = true);
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

        public async Task<PagedDto<TOutput>> Page<TInput, TOutput>(IQueryable<TInput> list, int pageSize, int pageNumber, string orderBy, string direction = "", string extraSort = "", bool count = true)
        {
            var stopwatch = Stopwatch.StartNew();

            var totalRecords = await list.CountAsync();

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

            stopwatch.Stop();

            _logger.LogDebug($"Get Grouped Listings (group) - Counting groups. Page Size: {pageSize}, Page Number: {pageNumber}, Time: {stopwatch.Elapsed.TotalSeconds} seconds");

            stopwatch.Restart();

            var result = await pagedList.ToListAsync();

            stopwatch.Stop();

            _logger.LogDebug($"Get Grouped Listings (group) - Getting groups. Time: {stopwatch.Elapsed.TotalSeconds} seconds");

            stopwatch.Restart();

            IEnumerable<TOutput> outputList;

            if (typeof(TOutput) != typeof(TInput))
                outputList = _mapper.Map<IEnumerable<TInput>, IEnumerable<TOutput>>(result);
            else
                outputList = (IEnumerable<TOutput>)result;

            var pagedDTO = new PagedDto<TOutput>
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

            stopwatch.Stop();

            _logger.LogDebug($"Get Grouped Listings (group) - Mapping groups to DTO. Time: {stopwatch.Elapsed.TotalSeconds} seconds");

            return pagedDTO;
        }
    }
}
