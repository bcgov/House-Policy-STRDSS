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
        Task<List<DropdownNumDto>> GetMessageReasons(string messageType);
        Task<DropdownNumDto?> GetMessageReasonByMessageTypeAndId(string messageType, long id);
        Task AddEmailMessage(DssEmailMessage message);
        Task<List<DssEmailMessage>> GetTakedownRequestEmailsToBatch();
    }
    public class EmailMessageRepository : RepositoryBase<DssEmailMessage>, IEmailMessageRepository
    {
        public EmailMessageRepository(DssDbContext dbContext, IMapper mapper, ICurrentUser currentUser, ILogger<StrDssLogger> logger) 
            : base(dbContext, mapper, currentUser, logger)
        {
        }

        public async Task<List<DropdownNumDto>> GetMessageReasons(string messageType)
        {
            var reasons = await _dbContext.DssMessageReasons.AsNoTracking()
                .Where(x => x.EmailMessageType == messageType)
                .Select(x => new DropdownNumDto { Id = x.MessageReasonId, Description = x.MessageReasonDsc })
                .ToListAsync();

            return reasons;
        }

        public async Task<DropdownNumDto?> GetMessageReasonByMessageTypeAndId(string messageType, long id)
        {
            var reason = await _dbContext.DssMessageReasons.AsNoTracking()
                .Where(x => x.EmailMessageType == messageType && x.MessageReasonId == id)
                .Select(x => new DropdownNumDto { Id = x.MessageReasonId, Description = x.MessageReasonDsc })
                .FirstOrDefaultAsync();

            return reason;
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
