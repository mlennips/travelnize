using LIT.Travelnize.Domain.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LIT.Travelnize.Infrastructure.Persistence
{
    public class AuditSaveChangesInterceptor(ICurrentUser currentUser) : SaveChangesInterceptor
    {
        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;
            if (context == null) return await base.SavingChangesAsync(eventData, result, cancellationToken);
            await BuildLogAsync(context);
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private async Task BuildLogAsync(DbContext context)
        {
            var correlationId = Guid.NewGuid();
            var user = await currentUser.GetUserAsync();

            // ChangeTracker-Einträge als Snapshot, um Collection-Änderungen zu vermeiden
            var entries = context.ChangeTracker.Entries().ToList();

            foreach (var entry in entries)
            {
                if (entry.State is not (EntityState.Added or EntityState.Modified or EntityState.Deleted))
                    continue;

                if (entry.Entity is IAuditableEntity auditableEntity)
                {
                    var details = "";
                    AddAuditLog(context, auditableEntity.AggregateId, correlationId, entry.Metadata.ClrType.Name, auditableEntity.Id, entry.State.ToString(), details, user);
                }
                else if (entry.Metadata.IsOwned())
                {
                    var ownership = entry.Metadata.FindOwnership();
                    if (ownership is { Properties.Count: > 0 })
                    {
                        var fkProperty = ownership.Properties[0];
                        var fkValue = entry.Property(fkProperty.Name).CurrentValue;

                        var parentEntry = entries.FirstOrDefault(e =>
                            e.Metadata == ownership.PrincipalEntityType &&
                            e.Property(ownership.PrincipalKey.Properties[0].Name).CurrentValue?.Equals(fkValue) == true);

                        if (parentEntry?.Entity is IAuditableEntity parentAuditable)
                        {
                            string details = entry.Metadata.Name.Split('.').LastOrDefault() ?? "";
                            AddAuditLog(context, parentAuditable.AggregateId, correlationId,
                                parentEntry.Metadata.ClrType.Name,
                                parentAuditable.Id, EntityState.Modified.ToString(), details, user);
                        }
                    }
                }
            }
        }

        private static void AddAuditLog(DbContext context, Guid aggregateId, Guid correlationId, string entityName, Guid entityId, string action, string details, IUser user)
        {
            var log = new AuditLogEntry
            {
                AggregateId = aggregateId,
                CorrelationId = correlationId,
                EntityName = entityName,
                EntityId = entityId,
                Action = action,
                Details = details,
                UserId = user.Id,
                UserName = user.UserName,
                Timestamp = DateTime.UtcNow
            };
            context.Add(log);
        }
    }
}