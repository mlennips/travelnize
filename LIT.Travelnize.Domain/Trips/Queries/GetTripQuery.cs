namespace LIT.Travelnize.Domain.Trips.Queries
{
    public record GetTripQuery(Guid TripId) : IQuery<GetTripDto>;
}
