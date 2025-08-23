using LIT.Travelnize.Domain.Base;
using LIT.Travelnize.Domain.Common;

namespace LIT.Travelnize.Domain.Trips
{
    public class Transportation(Guid id, Guid tripId, string name, string description, string identifier, Location? departure, 
        Location? arrival, DateTime departureDate, DateTime arrivalDate, ExternalUrl? routeLink,
        TransportationType type, List<Participant> passengers) : IEntity
    {
        public Guid Id { get; } = id;
        public Guid TripId { get; } = tripId;

        public string Name { get; private set; } = name;
        public string Description { get; private set; } = description;
        public string Identifier { get; private set; } = identifier;
        public Location? Departure { get; private set; } = departure;
        public Location? Arrival { get; private set; } = arrival;
        public DateTime DepartureDate { get; private set; } = departureDate;
        public DateTime ArrivalDate { get; private set; } = arrivalDate;
        public ExternalUrl? RouteLink { get; private set; } = routeLink;
        public TransportationType Type { get; private set; } = type;
        public IReadOnlyCollection<Participant> Passengers { get; private set; } = passengers.AsReadOnly();

        internal static Transportation Create(Guid tripId, string name, string description, string identifier, Location departure, 
            Location arrival, DateTime departureDate, DateTime arrivalDate, ExternalUrl routeLink, 
            TransportationType type, List<Participant>? passengers = null)
        {
            return new Transportation(Guid.NewGuid(), tripId, name, description, identifier, departure, arrival, 
                departureDate, arrivalDate, routeLink, type, passengers ?? []);
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
