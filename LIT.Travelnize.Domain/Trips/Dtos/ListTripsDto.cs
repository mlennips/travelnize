using LIT.Travelnize.Domain.Trips.ValueObjects;

namespace LIT.Travelnize.Domain.Trips.Dtos
{
    public record ListTripsDto(Guid Id, string Name, string Description, PlanningSlot Slot, TripStatus Status, string UserRole);
}
