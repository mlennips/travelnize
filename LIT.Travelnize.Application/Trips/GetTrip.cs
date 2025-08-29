using LIT.Travelnize.Domain.Base;
using LIT.Travelnize.Domain.Common;
using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.UseCases.Base;

namespace LIT.Travelnize.UseCases.Trips
{
    public static class GetTrip
    {
        public record GetTripQuery(Guid TripId) : IQuery<GetTripDTO>;

        public record GetTripDTO(Guid Id, Guid UserId, string Name, string Description, DateRange TravelPeriod);

        public class GetTripQueryHandler(IReadOnlyRepository<Trip> tripRepository) : IQueryHandler<GetTripQuery, GetTripDTO>
        {
            public async Task<Result<GetTripDTO>> Handle(GetTripQuery request, CancellationToken cancellationToken)
            {
                var trip = await tripRepository.GetByIdAsync(request.TripId);
                
                return trip is null
                    ? new ErrorDetail("Trip.NotFound", "Trip not found")
                    : new GetTripDTO(trip.Id, trip.UserId, trip.Name, trip.Description, trip.TravelPeriod);
            }
        }
    }
}
