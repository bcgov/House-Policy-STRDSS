//using Microsoft.EntityFrameworkCore.Diagnostics;
//using Microsoft.EntityFrameworkCore;
//using System.Data.Common;
//using System.Threading;
//using Npgsql;

//namespace StrDss.Data
//{
//    public class ConcurrencyInterceptor : IDbCommandInterceptor
//    {
//        private readonly DbContext _context;

//        public ConcurrencyInterceptor(DbContext context)
//        {
//            _context = context;
//        }

//        public async Task<int> ExecutingCommandAsync(DbContext context, DbCommand command, CancellationToken cancellationToken)
//        {
//            if (command.CommandText.StartsWith("UPDATE"))
//            {
//                var entityEntry = context.ChangeTracker.Entries().FirstOrDefault(e => e.State == EntityState.Modified);
//                if (entityEntry != null)
//                {
//                    var entityType = entityEntry.Entity.GetType();

//                    // Check if UpdDtm property exists
//                    var hasUpdDtmProperty = entityType.GetProperties().Any(p => p.Name == "UpdDtm");

//                    if (hasUpdDtmProperty)
//                    {
//                        var tableName = GetTableName(entityType);
//                        var propertyName = "UpdDtm"; // Assuming the property name is always "UpdDtm"

//                        var originalUpdDtm = await GetOriginalUpdDtmValue(entityEntry.Entity.Id, tableName, context);
//                        var entityUpdDtm = entityEntry.Property(propertyName).CurrentValue;

//                        if (originalUpdDtm != (DateTime?)entityUpdDtm) // Handle nullable upd_dtm
//                        {
//                            throw new DbUpdateConcurrencyException("Update conflict detected!");
//                        }
//                    }
//                }
//            }

//            return await command.ExecuteNonQueryAsync(cancellationToken);
//        }

//        private string GetTableName(Type entityType)
//        {
//            var entityMetadata = _context.Model.FindEntityType(entityType);
//            return entityMetadata.GetTableName();
//        }

//        private async Task<DateTime?> GetOriginalUpdDtmValue(object entityId, string tableName, DbContext context)
//        {
//            // Assuming your entity has an Id property
//            if (entityId == null)
//            {
//                throw new ArgumentNullException(nameof(entityId));
//            }

//            var parameters = new NpgsqlParameter[] { new NpgsqlParameter("@id", entityId) };
//            var sql = $"SELECT UpdDtm FROM {tableName} WHERE Id = @id";

//            return await context.Database.E.ExecuteSqlRawAsync<DateTime?>(sql, parameters);
//        }

//    }
//}
