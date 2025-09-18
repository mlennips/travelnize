using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.Domain.Trips.Queries;
using LIT.Travelnize.Domain.Trips.Specifications;

namespace LIT.Travelnize.UseCases.Trips
{
    public class ListTripsHandler(IReadOnlyRepository<Trip> tripRepository) : IQueryHandler<ListTripsQuery, ListTripsResponse[]>
    {
        public async Task<Result<ListTripsResponse[]>> Handle(ListTripsQuery request, CancellationToken cancellationToken)
        {
            var trip = await tripRepository.FindByAsync(new GetAllTripsForUserIdSpec(request.UserId), 
                (x) => Map(x, request.UserId));

            return trip is null
                ? TripErrors.TripNotFound
                : trip;
        }

        private static ListTripsResponse Map(Trip trip, Guid userId) =>
            new(trip.Id, trip.Name, trip.Description, trip.Slot, trip.Status, trip.Participants.First(x => x.UserId == userId).PermissionLevel);
    }
}
