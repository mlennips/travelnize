namespace LIT.Travelnize.Shared.Trips
{
    public record ListTripsQuery(Guid UserId) : IQuery<ListTripsDto[]>;
}
