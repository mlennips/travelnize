namespace LIT.Travelnize.Domain.Trips.Commands
{
    public record AddActivityCommand(
        Guid TripId,
        Guid DestinationId,
        string Name,
        string Description,
        Location Location,
        DateTime? Date,
        TimeSpan? Duration
    ) : ICommand<Guid>;
}