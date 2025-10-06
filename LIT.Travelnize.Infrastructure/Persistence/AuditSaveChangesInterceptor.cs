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
            var requestInfo = GetRequestInfo(currentUser);

            var entries = context.ChangeTracker.Entries().ToList();
            var parentLookup = BuildParentLookup(entries);

            foreach (var entry in entries)
            {
                if (entry.State is not (EntityState.Added or EntityState.Modified or EntityState.Deleted))
                    continue;

                if (entry.Entity is IAuditableEntity auditableEntity)
                {
                    var action = $"{entry.State}/{currentUser.HttpMethod ?? "?"}";
                    AddAuditLog(context, auditableEntity.AggregateId, correlationId, entry.Metadata.ClrType.Name, auditableEntity.Id, action, string.Empty, requestInfo, user);
                    continue;
                }

                if (!entry.Metadata.IsOwned())
                    continue;

                var ownership = entry.Metadata.FindOwnership();
                if (ownership is null || ownership.Properties.Count == 0)
                    continue;

                var fkProperty = ownership.Properties[0];
                var fkValue = entry.Property(fkProperty.Name).CurrentValue;
                var parentKey = (ownership.PrincipalEntityType.ClrType, fkValue);

                if (parentLookup.TryGetValue(parentKey, out var parentEntry) && parentEntry.Entity is IAuditableEntity parentAuditable)
                {
                    var details = entry.Metadata.Name.Split('.').LastOrDefault() ?? string.Empty;
                    var action = $"{EntityState.Modified}/{currentUser.HttpMethod ?? "?"}";
                    AddAuditLog(context, parentAuditable.AggregateId, correlationId,
                        parentEntry.Metadata.ClrType.Name,
                        parentAuditable.Id, action, details, requestInfo, user);
                }
            }
        }

        private static Dictionary<(Type, object?), Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry> BuildParentLookup(List<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry> entries)
        {
            var parentLookup = new Dictionary<(Type, object?), Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry>();
            foreach (var entry in entries)
            {
                if (entry.Entity is IAuditableEntity && entry.Metadata.FindPrimaryKey() is { } pk && entry.State != EntityState.Detached)
                {
                    var keyValue = entry.Property(pk.Properties[0].Name).CurrentValue;
                    parentLookup[(entry.Metadata.ClrType, keyValue)] = entry;
                }
            }

            return parentLookup;
        }

        private static void AddAuditLog(DbContext context, Guid aggregateId, Guid correlationId, string entityName, Guid entityId, string action, string details, string requestInfo, IUser user)
        {
            var log = new AuditLogEntry
            {
                CorrelationId = correlationId,
                AggregateId = aggregateId,
                EntityName = entityName,
                EntityId = entityId,
                Action = action,
                Details = details,
                UserId = user.Id,
                UserName = user.UserName,
                Timestamp = DateTime.UtcNow,
                RequestInfo = requestInfo
            };
            context.Add(log);
        }

        private static string GetRequestInfo(ICurrentUser user)
        {
            var requestInfo = new
            {
                user.RequestPath,
                user.HttpMethod,
                user.IpAddress,
                user.UserAgent,
                user.TraceIdentifier,
                user.Referer
            };
            return System.Text.Json.JsonSerializer.Serialize(requestInfo);
        }
    }
}