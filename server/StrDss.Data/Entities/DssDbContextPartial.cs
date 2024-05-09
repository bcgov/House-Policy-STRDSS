using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using StrDss.Model;
using Microsoft.Extensions.Logging;
using StrDss.Common;

namespace StrDss.Data.Entities
{
    public partial class DssDbContext
    {
        private ICurrentUser _currentUser = null!;
        private ILogger<StrDssLogger> _logger = null!;

        public DssDbContext(DbContextOptions<DssDbContext> options, ICurrentUser currentUser, ILogger<StrDssLogger> logger)
            : base(options)
        {
            _currentUser = currentUser;
            _logger = logger;
        }

        public override int SaveChanges()
        {
            var modifiedEntries = ChangeTracker.Entries()
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

            var addedEntries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added);

            foreach (var entry in addedEntries)
            {
                if (entry.Members.Any(m => m.Metadata.Name == "UpdDtm"))
                {
                    entry.Member("UpdDtm").CurrentValue = utcNow;
                    entry.Member("UpdUserGuid").CurrentValue = _currentUser.UserGuid;
                }
            }

            int result = 0;

            result = base.SaveChanges();

            return result;
        }

        /// <summary>
        /// Checks for concurrency conflicts in the entity being updated.
        /// This method is used as a workaround for the limitations of the database-first approach,
        /// where the ConcurrencyCheck annotation is wiped out during scaffolding and must be manually added.
        /// While this implementation has its limitations, it can handle most cases.
        /// </summary>
        /// <param name="entry">The EntityEntry representing the entity being updated.</param>
        /// <exception cref="DbUpdateConcurrencyException">
        /// Thrown when a concurrency conflict is detected during the update operation.
        /// </exception>

        private void CheckConcurrency(EntityEntry entry)
        {
            // values from the database before the values of the DTO is applied
            var originalValues = entry.OriginalValues;

            // values from the DTO. UpdDtm is not supposed be updated and remain as it was retrieved.
            var currentValues = entry.CurrentValues;

            var originalValue = (DateTime) (originalValues["UpdDtm"] ?? DateTime.MinValue);
            var currentValue = (DateTime) (currentValues["UpdDtm"] ?? DateTime.MinValue);

            if (originalValue != currentValue)
            {
                var entityName = Model.FindEntityType(entry.Entity.GetType())?.ShortName();
                if (entityName != null && entityName.StartsWith("Dss") )
                { 
                    entityName = entityName.Substring(3);
                }
                else
                {
                    entityName = "?";
                }

                var message = $"Update conflict detected when updating {entityName}!";
                _logger.LogInformation(message);

                throw new DbUpdateConcurrencyException(message);
            }
        }
    }
}
