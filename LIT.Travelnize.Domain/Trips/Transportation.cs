using System.ComponentModel.DataAnnotations.Schema;

namespace LIT.Travelnize.Domain.Trips
{
    public class Transportation : IAuditableEntity
    {
        private List<Participant> _passengers = [];

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; init; }
        public Guid TripId { get; init; }
        public Guid AggregateId => TripId;

        public string Name { get; private set; } = default!;
        public string Description { get; private set; } = default!;
        public string Identifier { get; private set; } = default!;
        public Location Departure { get; private set; } = default!;
        public Location Arrival { get; private set; } = default!;
        public PlanningSlot Traveltime { get; private set; } = default!;
        public ResourceReference? RouteWebsite { get; private set; } = default!;
        public TransportationType Type { get; private set; } = default!;
        public IEnumerable<Participant> Passengers { get => _passengers.AsReadOnly(); init => _passengers = value.ToList(); }
        public EntityReference? TargetReference { get; private set; } = default!;

        internal static Transportation Create(Guid tripId, string name, string description, string identifier, Location departure, 
            Location arrival, PlanningSlot traveltime, ResourceReference? RouteWebsite, TransportationType type,
            EntityReference? targetReference)
        {
            return new Transportation()
            {
                Id = Guid.NewGuid(),
                TripId = tripId,
                Name = name,
                Description = description,
                Identifier = identifier,
                Departure = departure,
                Arrival = arrival,
                Traveltime = traveltime,
                RouteWebsite = RouteWebsite,
                Type = type,
                TargetReference = targetReference,
            };
        }

        internal Result Update(string name, string description, string identifier, Location departure, 
            Location arrival, PlanningSlot traveltime, ResourceReference? routeWebsite, 
            TransportationType type, Participant[] passengers, EntityReference? targetReference)
        {
            if (traveltime.IsEmpty)
            {
                return TripErrors.TransportationDatesMayNotBeEmpty;
            }
            Name = name;
            Description = description;
            Identifier = identifier;
            Departure = departure;
            Arrival = arrival;
            Traveltime = traveltime;
            RouteWebsite = routeWebsite;
            Type = type;
            TargetReference = targetReference;
            _passengers = passengers.ToList();
            return Result.Success();
        }
    }
}
