namespace LIT.Travelnize.Domain.Trips.Dtos
{
    public record TravelSegmentDto(
        Guid Id,
        string Description,
        PlanningSlotDto Slot,
        IReadOnlyList<DestinationDto> Destinations
    )
    {
        public static TravelSegmentDto From(TravelSegment segment) =>
            new(segment.Id, segment.Description, PlanningSlotDto.From(segment.Slot), segment.Destinations.Select(DestinationDto.From).ToList());
    }
}
