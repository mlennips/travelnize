namespace LIT.Travelnize.Domain.Trips.Dtos
{
    public record ParticipantDto(
        Guid Id,
        Guid? UserId,
        string Name,
        string? Email,
        string PermissionLevel
    )
    {
        public static ParticipantDto From(Participant p) =>
            new(p.Id, p.UserId, p.Name, p.Email?.Value, p.PermissionLevel.ToString());

        public string Initials
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Name))
                    return string.Empty;

                var parts = Name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0)
                    return string.Empty;

                if (parts.Length == 1)
                    return parts[0][..1].ToUpperInvariant();

                return string.Concat(parts[0][0], parts[^1][0]).ToUpperInvariant();
            }
        }
    }
}
