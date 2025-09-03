namespace LIT.Travelnize.Domain.Trips.Queries
{
    public record ListTripsDto(Guid Id, string Name, string Description, DateTime? TravelStart, DateTime? TravelEnd);
}
