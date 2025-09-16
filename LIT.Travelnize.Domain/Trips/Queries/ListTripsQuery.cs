using LIT.Travelnize.Domain.Trips.Dtos;

namespace LIT.Travelnize.Domain.Trips.Queries
{
    public record ListTripsQuery(Guid UserId) : IQuery<ListTripsDto[]>;
}
