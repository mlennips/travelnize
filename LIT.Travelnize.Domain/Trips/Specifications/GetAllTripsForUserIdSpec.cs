using System.Linq.Expressions;

namespace LIT.Travelnize.Domain.Trips.Specifications
{
    public class GetAllTripsForUserIdSpec(Guid userId) : ISpecification<Trip>
    {
        public Expression<Func<Trip, bool>> Criteria => x => x.UserId == userId;

        public IQueryable<Trip> FinalProcessing(IQueryable<Trip> filteredQueryable) 
            => filteredQueryable
                .OrderByDescending(x => x.Slot.Order);
    }
}
