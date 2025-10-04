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
            new(p.Id, p.UserId, p.Name, p.Email?.Value, p.PermissionLevel.Value);

        public string Initials
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Name))
                    return string.Empty;

                var parts = Name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0)
                    return string.Empty;

                parts = parts.Select(x => new string(x.Where(char.IsLetterOrDigit).ToArray())).ToArray();

                string initials = parts.Length == 1
                    ? parts[0][..1]
                    : string.Concat(parts[0][0], parts[^1][0]);

                return initials.ToUpperInvariant();
            }
        }
    }
}
