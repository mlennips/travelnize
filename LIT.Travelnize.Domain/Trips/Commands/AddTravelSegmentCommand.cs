namespace LIT.Travelnize.Domain.Trips.Commands
{
    public record AddTravelSegmentCommand(
        Guid TripId,
        string Name,
        string Description,
        PlanningSlot Slot
    ) : ICommand<Guid>;
}