using LIT.Travelnize.Domain.Base;
using LIT.Travelnize.Domain.Common;

namespace LIT.Travelnize.Domain.Trips
{
    public class Transportation : IEntity
    {
        private List<Participant> _passengers = [];

        public Guid Id { get; init; }
        public Guid TripId { get; init; }

        public string Name { get; private set; } = default!;
        public string Description { get; private set; } = default!;
        public string Identifier { get; private set; } = default!;
        public Location Departure { get; private set; } = default!;
        public Location Arrival { get; private set; } = default!;
        public DateTime DepartureDate { get; private set; }
        public DateTime ArrivalDate { get; private set; }
        public ExternalUrl RouteLink { get; private set; } = default!;
        public TransportationType Type { get; private set; } = default!;
        public IEnumerable<Participant> Passengers => _passengers.AsReadOnly();

        internal static Transportation Create(Guid tripId, string name, string description, string identifier, Location departure, 
            Location arrival, DateTime departureDate, DateTime arrivalDate, ExternalUrl routeLink, TransportationType type)
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
                RouteLink = routeLink,
                Type = type
            };
        }

        internal Result Update(string name, string description, string identifier, Location departure, 
            Location arrival, DateTime departureDate, DateTime arrivalDate, ExternalUrl routeLink, 
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
            RouteLink = routeLink;
            Type = type;
            _passengers = passengers.ToList();
            return Result.Success();
        }
    }
}
