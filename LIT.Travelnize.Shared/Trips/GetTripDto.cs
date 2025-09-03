namespace LIT.Travelnize.Shared.Trips
{
    public record GetTripDto(Guid Id, string Name, string Description, DateTime? TravelStart, DateTime? TravelEnd);
}
