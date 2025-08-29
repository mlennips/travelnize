using LIT.Travelnize.Domain.Base;
using LIT.Travelnize.Domain.Common;
using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.UseCases.Base;

namespace LIT.Travelnize.UseCases.Trips
{
    public static class GetTrip
    {
        public record GetTripQuery(Guid TripId) : IQuery<GetTripDTO>;

        public record GetTripDTO(Guid Id, string Name, string Description, DateRange TravelPeriod);

        public class GetTripQueryHandler(IReadOnlyRepository<Trip> tripRepository) : IQueryHandler<GetTripQuery, GetTripDTO>
        {
            public async Task<Result<GetTripDTO>> Handle(GetTripQuery request, CancellationToken cancellationToken)
            {
                var t1 = await tripRepository.GetByIdAsync(request.TripId);
                
                return t1 is null
                    ? new ErrorDetail("Trip.NotFound", "Trip not found")
                    : new GetTripDTO(t1.Id, t1.Name, t1.Description, t1.TravelPeriod);
            }
        }
    }
}
