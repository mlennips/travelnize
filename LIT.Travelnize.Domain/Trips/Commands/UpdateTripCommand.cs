namespace LIT.Travelnize.Domain.Trips.Commands
{
    public record UpdateTripCommand(Guid TripId, string Name, string Description, PlanningSlot TravelSlot) : ICommand;
}
