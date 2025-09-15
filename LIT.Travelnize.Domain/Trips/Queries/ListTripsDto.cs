using LIT.Travelnize.Domain.Trips.ValueObjects;

namespace LIT.Travelnize.Domain.Trips.Queries
{
    public record ListTripsDto(Guid Id, string Name, string Description, PlanningSlot Slot, TripStatus Status, string UserRole);
}
