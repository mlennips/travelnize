namespace LIT.Travelnize.Domain.Trips.Dtos
{
    public record GetTripDto(
        Guid Id,
        Guid UserId,
        string Name,
        string Description,
        PlanningSlotDto Slot,
        string Status,
        IReadOnlyList<TravelSegmentDto> TravelSegments,
        IReadOnlyList<ParticipantDto> Participants,
        IReadOnlyList<TransportationDto> Transportations
    )
    {
        public static GetTripDto From(Trip trip)
        {
            return new GetTripDto(
                trip.Id,
                trip.UserId,
                trip.Name,
                trip.Description,
                PlanningSlotDto.From(trip.Slot),
                trip.Status.Value,
                trip.TravelSegments.OrderBy(x => x.Slot.Start).Select(TravelSegmentDto.From).ToList(),
                trip.Participants.Select(ParticipantDto.From).ToList(),
                trip.Transportations.Select(TransportationDto.From).ToList()
            );
        }
    }
}
