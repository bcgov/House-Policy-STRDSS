using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StrDss.Data.Entities;
using StrDss.Model;

namespace StrDss.Data.Repositories
{
    public interface IEmailMessageRepository
    {
        Task<List<DropdownNumDto>> GetMessageReasons(string messageType);
        Task<DropdownNumDto?> GetMessageReasonByMessageTypeAndId(string messageType, long id);
    }
    public class EmailMessageRepository : RepositoryBase<DssEmailMessage>, IEmailMessageRepository
    {
        public EmailMessageRepository(DssDbContext dbContext, IMapper mapper, ICurrentUser currentUser) 
            : base(dbContext, mapper, currentUser)
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
    }
}
