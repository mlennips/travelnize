using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.Domain.Trips.Dtos;
using LIT.Travelnize.Domain.Trips.Queries;

namespace LIT.Travelnize.UseCases.Trips
{
    public class GetTripHandler(IReadOnlyRepository<Trip> tripRepository) : IQueryHandler<GetTripQuery, GetTripDto>
    {
        public async Task<Result<GetTripDto>> Handle(GetTripQuery request, CancellationToken cancellationToken)
        {
            var trip = await tripRepository.GetByIdAsync(request.TripId);

            return trip is null
                ? TripErrors.TripNotFound
                : Result.Success(GetTripDto.From(trip));
        }
    }
}
