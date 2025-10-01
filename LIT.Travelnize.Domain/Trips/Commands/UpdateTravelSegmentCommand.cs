namespace LIT.Travelnize.Domain.Trips.Commands
{
    public record UpdateTravelSegmentCommand(
        Guid TripId,
        Guid SegmentId,
        PlanningSlot Slot,
        string Name,
        string Description
    ) : ICommand;
}