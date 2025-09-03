namespace LIT.Travelnize.Domain.Trips.Queries
{
    public record GetTripDto(Guid Id, string Name, string Description, DateTime? TravelStart, DateTime? TravelEnd);
}
