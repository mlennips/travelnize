using LIT.Travelnize.Domain.Base;
using LIT.Travelnize.Domain.Common;
using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.Domain.Trips.Specifications;
using LIT.Travelnize.UseCases.Base;

namespace LIT.Travelnize.UseCases.Trips
{
    public static class ListTrips
    {
        public record ListTripsQuery(Guid UserId) : IQuery<ListTripsDTO[]>;

        public record ListTripsDTO(Guid Id, string Name, string Description, DateRange TravelPeriod)
        {
            internal static ListTripsDTO Create(Trip trip) =>
                new(trip.Id, trip.Name, trip.Description, trip.TravelPeriod);
        }

        public class ListTripQueryHandler(IReadOnlyRepository<Trip> tripRepository) : IQueryHandler<ListTripsQuery, ListTripsDTO[]>
        {
            public async Task<Result<ListTripsDTO[]>> Handle(ListTripsQuery request, CancellationToken cancellationToken)
            {
                var trip = await tripRepository.FindByAsync(new GetAllTripsForUserIdSpec(request.UserId), ListTripsDTO.Create);

                return trip is null
                    ? TripErrors.TripNotFound
                    : trip;
            }
        }
    }
}
