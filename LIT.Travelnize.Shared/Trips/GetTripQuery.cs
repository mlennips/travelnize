namespace LIT.Travelnize.Shared.Trips
{
    public record GetTripQuery(Guid TripId) : IQuery<GetTripDto>;
}
