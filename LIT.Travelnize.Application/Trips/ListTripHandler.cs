using LIT.Travelnize.Domain.Base;
using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.Domain.Trips.Specifications;
using LIT.Travelnize.Shared.Trips;

namespace LIT.Travelnize.UseCases.Trips
{
    public class ListTripHandler(IReadOnlyRepository<Trip> tripRepository) : IQueryHandler<ListTripsQuery, ListTripsDto[]>
    {
        public async Task<Result<ListTripsDto[]>> Handle(ListTripsQuery request, CancellationToken cancellationToken)
        {
            var trip = await tripRepository.FindByAsync(new GetAllTripsForUserIdSpec(request.UserId), Map);

            return trip is null
                ? TripErrors.TripNotFound
                : trip;
        }

        private static ListTripsDto Map(Trip trip) =>
            new(trip.Id, trip.Name, trip.Description, trip.Slot.DateRange?.Start, trip.Slot.DateRange?.End);
    }
}
