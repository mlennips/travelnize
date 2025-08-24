using LIT.Travelnize.Domain.Base;
using LIT.Travelnize.Domain.Common;

namespace LIT.Travelnize.Domain.Trips
{
    public class Transportation : IEntity
    {
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
        public IReadOnlyCollection<Participant> Passengers { get; private set; } = [];

        internal static Transportation Create(Guid tripId, string name, string description, string identifier, Location departure, 
            Location arrival, DateTime departureDate, DateTime arrivalDate, ExternalUrl routeLink, 
            TransportationType type, List<Participant>? passengers = null)
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
                Type = type,
                Passengers = passengers ?? []
            };
        }

        internal Result Update(string name, string description, string identifier, Location departure, 
            Location arrival, DateTime departureDate, DateTime arrivalDate, ExternalUrl routeLink, 
            TransportationType type, Participant[]? passengers = null)
        {
            if (departureDate >= arrivalDate)
            {
                return TripsErrors.InvalidTransportationDates;
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
            Passengers = passengers ?? [];
            return Result.Success();
        }
    }
}
