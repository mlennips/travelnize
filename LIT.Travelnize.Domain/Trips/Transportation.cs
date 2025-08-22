using LIT.Travelnize.Domain.Base;
using LIT.Travelnize.Domain.Common;

namespace LIT.Travelnize.Domain.Trips
{
    public class Transportation(Guid id, Guid tripSegmentId, string description, Location departure, 
        Location arrival, DateTime departureDate, DateTime arrivalDate, ExternalUrl routeLink, 
        string transportationType, Participant[] passengers) : IEntity
    {
        public Guid Id { get; private set; } = id;
        public Guid TripSegmentId { get; private set; } = tripSegmentId;
        public string Description { get; private set; } = description;
        public Location Departure { get; private set; } = departure;
        public Location Arrival { get; private set; } = arrival;
        public DateTime DepartureDate { get; private set; } = departureDate;
        public DateTime ArrivalDate { get; private set; } = arrivalDate;
        public ExternalUrl RouteLink { get; private set; } = routeLink;
        public string TransportationType { get; private set; } = transportationType;
        public Participant[] Passengers { get; private set; } = passengers;

        public static Transportation Create(Guid tripSegmentId, string description, Location departure, 
            Location arrival, DateTime departureDate, DateTime arrivalDate, ExternalUrl routeLink, 
            string transportationType, Participant[]? passengers = null)
        {
            return new Transportation(Guid.NewGuid(), tripSegmentId, description, departure, arrival, 
                departureDate, arrivalDate, routeLink, transportationType, passengers ?? []);
        }
    }
}
