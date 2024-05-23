using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StrDss.Common;
using StrDss.Data.Entities;
using StrDss.Model;

namespace StrDss.Data.Repositories
{
    public interface IEmailMessageRepository
    {
        Task AddEmailMessage(DssEmailMessage message);
        Task<List<DssEmailMessage>> GetTakedownRequestEmailsToBatch();
    }
    public class EmailMessageRepository : RepositoryBase<DssEmailMessage>, IEmailMessageRepository
    {
        public EmailMessageRepository(DssDbContext dbContext, IMapper mapper, ICurrentUser currentUser, ILogger<StrDssLogger> logger) 
            : base(dbContext, mapper, currentUser, logger)
        {
        }

        public async Task AddEmailMessage(DssEmailMessage message)
        {
            await _dbSet.AddAsync(message);
        }

        public async Task<List<DssEmailMessage>> GetTakedownRequestEmailsToBatch()
        {
            return await _dbSet
                .Where(x => x.EmailMessageType == EmailMessageTypes.TakedownRequest && x.BatchingEmailMessageId == null)
                .Include(x => x.RequestingOrganization)
                .ToListAsync();
        }
    }
}
