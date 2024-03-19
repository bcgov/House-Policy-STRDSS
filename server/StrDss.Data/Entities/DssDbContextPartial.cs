using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using StrDss.Model;
using Microsoft.Extensions.Logging;

namespace StrDss.Data.Entities
{
    public partial class DssDbContext
    {
        private ICurrentUser _currentUser;
        private ILogger<DssDbContext> _logger;

        public DssDbContext(DbContextOptions<DssDbContext> options, ICurrentUser currentUser, ILogger<DssDbContext> logger)
            : base(options)
        {
            _currentUser = currentUser;
            _logger = logger;
        }

        public override int SaveChanges()
        {
            IEnumerable<EntityEntry> modifiedEntries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Modified);

            DateTime utcNow = DateTime.UtcNow;

            foreach (var entry in modifiedEntries)
            {
                if (entry.Members.Any(m => m.Metadata.Name == "UpdDtm"))
                {
                    CheckConcurrency(entry);

                    entry.Member("UpdDtm").CurrentValue = utcNow;
                    entry.Member("UpdUserGuid").CurrentValue = _currentUser.UserGuid;
                }
            }

            int result = 0;

            try
            {
                result = base.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogInformation(ex.Message);
                throw;
            }
            catch (Exception e)
            {

            }

            return result;
        }

        public void CheckConcurrency(EntityEntry entry)
        {
            var originalValues = entry.OriginalValues;
            var currentValues = entry.CurrentValues;

            var originalValue = (DateTime) (originalValues["UpdDtm"] ?? DateTime.MinValue);
            var currentValue = (DateTime) (currentValues["UpdDtm"] ?? DateTime.MinValue);

            if (originalValue != currentValue)
            {
                var entityType = Model.FindEntityType(entry.Entity.GetType());

                throw new DbUpdateConcurrencyException($"Update conflict detected when updating {entityType?.GetTableName()}!");
            }
        }
    }
}
