using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.Domain.Trips.Commands;
using LIT.Travelnize.Domain.Trips.ValueObjects;

namespace LIT.Travelnize.UseCases.Trips
{
    public class CreateTripHandler(IUnitOfWork uow) : ICommandHandler<CreateTripCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(CreateTripCommand request, CancellationToken cancellationToken)
        {
            var user = await uow.GetUserAsync();
            var trip = Trip.Create(user, request.Name, request.Description);

            if (!string.IsNullOrWhiteSpace(request.DefaultTravelSegmentTitle))
            {
                var travelSegment = trip.AddTravelSegment(request.DefaultTravelSegmentTitle, "", PlanningSlot.Empty).Value!;
                if(!string.IsNullOrWhiteSpace(request.DefaultDestinationTitle))
                {
                    trip.AddDestinationToTravelSegment(travelSegment.Id, request.DefaultDestinationTitle, "", Location.Empty);
                }
            }

            await uow.AddAsync(trip);
            return trip.Id;
        }
    }
}
