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
            var slot = PlanningSlot.Create(request.TravelStart, request.TravelEnd);
            var trip = Trip.Create(user, request.Name, request.Description, slot);
            await uow.AddAsync(trip);
            return trip.Id;
        }
    }
}
