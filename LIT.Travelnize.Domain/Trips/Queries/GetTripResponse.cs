using LIT.Travelnize.Domain.Trips.Dtos;

namespace LIT.Travelnize.Domain.Trips.Queries
{
    public record GetTripResponse(
        Guid Id,
        Guid UserId,
        string Name,
        string Description,
        PlanningSlot Slot,
        TripStatus Status,
        IReadOnlyList<TravelSegmentDto> TravelSegments,
        IReadOnlyList<ParticipantDto> Participants,
        IReadOnlyList<TransportationDto> Transportations
    )
    {
        public static GetTripResponse From(Trip trip)
        {
            return new GetTripResponse(
                trip.Id,
                trip.UserId,
                trip.Name,
                trip.Description,
                trip.Slot,
                trip.Status,
                trip.TravelSegments.Select(TravelSegmentDto.From).ToList(),
                trip.Participants.Select(ParticipantDto.From).ToList(),
                trip.Transportations.Select(TransportationDto.From).ToList()
            );
        }
    }
}
