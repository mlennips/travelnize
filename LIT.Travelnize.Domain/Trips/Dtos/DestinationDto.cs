namespace LIT.Travelnize.Domain.Trips.Dtos
{
    public record DestinationDto(
        Guid Id,
        string Name,
        string Description,
        PlanningSlot Slot,
        Location Location,
        string? ImageUrl,
        string? Website,
        IReadOnlyList<AccommodationDto> Accommodations,
        IReadOnlyList<ActivityDto> Activities
    )
    {
        public static DestinationDto From(Destination d) =>
            new(
                d.Id,
                d.Name,
                d.Description,
                d.Slot,
                d.Location,
                d.ImageUrl?.Value,
                d.Website?.Value,
                d.Accommodations.Select(AccommodationDto.From).ToList(),
                d.Activities.Select(ActivityDto.From).ToList()
            );
    }
}
