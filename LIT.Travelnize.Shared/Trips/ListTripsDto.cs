namespace LIT.Travelnize.Shared.Trips
{
    public record ListTripsDto(Guid Id, string Name, string Description, DateTime TravelStart, DateTime TravelEnd);
}
