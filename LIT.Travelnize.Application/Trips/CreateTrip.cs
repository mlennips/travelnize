using LIT.Travelnize.Domain.Base;
using LIT.Travelnize.Domain.Common;
using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.UseCases.Base;

namespace LIT.Travelnize.UseCases.Trips
{
    public static class CreateTrip
    {
        public record CreateTripCommand(string Name, string Description, DateTime TravelStart, DateTime TravelEnd) : ICommand<Guid>;

        public class CreateTripHandler(IUnitOfWork uow) : ICommandHandler<CreateTripCommand, Guid>
        {
            public async Task<Result<Guid>> Handle(CreateTripCommand request, CancellationToken cancellationToken)
            {
                var user = await uow.GetUserAsync();
                var trip = Trip.Create(user, request.Name, request.Description, new DateRange(request.TravelStart, request.TravelEnd));
                trip.ChangeParticipantPermission(user.Id, PermissionLevel.Organisator);
                await uow.AddAsync(trip);
                return trip.Id;
            }
        }
    }
}
