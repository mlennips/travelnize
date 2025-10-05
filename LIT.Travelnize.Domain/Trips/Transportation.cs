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
        public DateTime DepartureDate { get; private set; }
        public DateTime ArrivalDate { get; private set; }
        public ResourceReference? RouteWebsite { get; private set; } = default!;
        public TransportationType Type { get; private set; } = default!;
        public IEnumerable<Participant> Passengers => _passengers.AsReadOnly();

        internal static Transportation Create(Guid tripId, string name, string description, string identifier, Location departure, 
            Location arrival, DateTime departureDate, DateTime arrivalDate, ResourceReference? RouteWebsite, TransportationType type)
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
                DepartureDate = departureDate,
                ArrivalDate = arrivalDate,
                RouteWebsite = RouteWebsite,
                Type = type
            };
        }

        internal Result Update(string name, string description, string identifier, Location departure, 
            Location arrival, DateTime departureDate, DateTime arrivalDate, ResourceReference routeWebsite, 
            TransportationType type, Participant[] passengers)
        {
            if (departureDate >= arrivalDate)
            {
                return TripErrors.InvalidTransportationDates;
            }
            Name = name;
            Description = description;
            Identifier = identifier;
            Departure = departure;
            Arrival = arrival;
            DepartureDate = departureDate;
            ArrivalDate = arrivalDate;
            RouteWebsite = routeWebsite;
            Type = type;
            _passengers = passengers.ToList();
            return Result.Success();
        }
    }
}
