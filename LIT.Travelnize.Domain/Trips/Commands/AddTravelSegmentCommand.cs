namespace LIT.Travelnize.Domain.Trips.Commands
{
    public record AddTravelSegmentCommand(
        Guid TripId,
        DateTime Start,
        DateTime End,
        string Description
    ) : ICommand<Guid>;
}