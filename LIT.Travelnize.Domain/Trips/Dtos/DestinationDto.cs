namespace LIT.Travelnize.Domain.Trips.Dtos
{
    public record DestinationDto(
        Guid Id,
        string Name,
        string Description,
        PlanningSlot Slot,
        Location Location,
        ResourceReference? Image,
        ResourceReference? Website,
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
                d.Image,
                d.Website,
                d.Accommodations.Select(AccommodationDto.From).ToList(),
                d.Activities.Select(ActivityDto.From).ToList()
            );
    }
}
