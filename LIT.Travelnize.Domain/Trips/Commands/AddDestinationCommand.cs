namespace LIT.Travelnize.Domain.Trips.Commands
{
    public record AddDestinationCommand(
        Guid TripId,
        Guid SegmentId,
        string Name,
        string Description,
        Location Location,
        PlanningSlot Slot
    ) : ICommand<Guid>;
}