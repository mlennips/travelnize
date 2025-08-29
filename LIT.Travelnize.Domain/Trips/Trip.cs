using LIT.Travelnize.Domain.Base;
using LIT.Travelnize.Domain.Common;

namespace LIT.Travelnize.Domain.Trips
{
    public class Trip : AggregateRoot
    {
        private readonly List<TravelSegment> _travelSegments = [];
        private readonly List<Participant> _participants = [];
        private readonly List<Transportation> _transportations = [];

        public override Guid Id { get; init; }
        public Guid UserId { get; init; }

        public string Name { get; private set; } = default!;
        public string Description { get; private set; } = default!;
        public DateRange TravelPeriod { get; private set; } = default!;

        public IEnumerable<TravelSegment> TravelSegments => _travelSegments.AsReadOnly();
        public IEnumerable<Participant> Participants => _participants.AsReadOnly();
        public IEnumerable<Transportation> Transportations => _transportations.AsReadOnly();

        public static Trip Create(IUser user, string name, string description, DateRange travelPeriod)
        {
            var trip = new Trip()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Name = name,
                Description = description,
                TravelPeriod = travelPeriod
            };
            trip.AddParticipant(user.Id, user.Name, user.Email);

            return trip;
        }

        public Result Update(DateRange travelPeriod)
        {
            TravelPeriod = travelPeriod;
            return Result.Success();
        }

        public Result<TravelSegment> AddTravelSegment(DateRange dateRange, string description)
        {
            var newSegment = TravelSegment.Create(Id, description, dateRange);
            _travelSegments.Add(newSegment);
            // Raise TravelSegmentAdded event (if using event sourcing)
            // RaiseEvent(new TravelSegmentAdded(newSegment.Id, Id));

            return newSegment;
        }

        public Result RemoveTravelSegment(Guid segmentId)
        {
            var segment = _travelSegments.FirstOrDefault(s => s.Id == segmentId);
            if (segment == null)
            {
                return TripsErrors.TravelSegmentNotFound;
            }

            _travelSegments.Remove(segment);
            // Raise TravelSegmentRemoved event (if using event sourcing)
            // RaiseEvent(new TravelSegmentRemoved(segmentId));

            return Result.Success();
        }

        public Result UpdateTravelSegment(Guid segmentId, DateRange dateRange, string description)
        {
            var segment = _travelSegments.FirstOrDefault(s => s.Id == segmentId);
            if (segment == null)
            {
                return TripsErrors.TravelSegmentNotFound;
            }

            segment.Update(dateRange, description);
            // Raise TravelSegmentUpdated event (if using event sourcing)
            // RaiseEvent(new TravelSegmentUpdated(segmentId));

            return Result.Success();
        }

        public Result<Destination> AddDestinationToTravelSegment(Guid segmentId, string name, string description, Location location)
        {
            var segment = _travelSegments.FirstOrDefault(s => s.Id == segmentId);
            if (segment == null)
            {
                return TripsErrors.TravelSegmentNotFound;
            }

            var destination = Destination.Create(Id, segmentId, name, description, segment.DateRange, location);
            segment.AddDestination(destination);
            // Raise DestinationAdded event (if using event sourcing)
            // RaiseEvent(new DestinationAdded(destination.Id, segmentId));

            return destination;
        }

        public Result RemoveDestinationFromTravelSegment(Guid segmentId, Guid destinationId)
        {
            var segment = _travelSegments.FirstOrDefault(s => s.Id == segmentId);
            if (segment == null)
            {
                return TripsErrors.TravelSegmentNotFound;
            }
            var destination = segment.Destinations.FirstOrDefault(d => d.Id == destinationId);
            if (destination == null)
            {
                return TripsErrors.DestinationNotFound;
            }
            segment.RemoveDestination(destinationId);
            // Raise DestinationRemoved event (if using event sourcing)
            // RaiseEvent(new DestinationRemoved(destinationId));

            return Result.Success();
        }

        public Result UpdateDestinationInTravelSegment(Guid segmentId, Guid destinationId, string name, string description, Location location)
        {
            var segment = _travelSegments.FirstOrDefault(s => s.Id == segmentId);
            if (segment == null)
            {
                return TripsErrors.TravelSegmentNotFound;
            }
            var destination = segment.Destinations.FirstOrDefault(d => d.Id == destinationId);
            if (destination == null)
            {
                return TripsErrors.DestinationNotFound;
            }
            destination.Update(name, description, location);
            // Raise DestinationUpdated event (if using event sourcing)
            // RaiseEvent(new DestinationUpdated(destinationId));

            return Result.Success();
        }

        public Result<Participant> AddParticipant(Guid userId, string name, Email email)
        {
            var participant = Participant.CreateAsUser(Id, userId, name, email);
            _participants.Add(participant);
            // Raise ParticipantAdded event (if using event sourcing)
            // RaiseEvent(new ParticipantAdded(participant.Id, Id));

            return participant;
        }

        public Result<Participant> AddParticipant(string name, Email email)
        {
            var participant = Participant.CreateAsGuest(Id, name, email);
            _participants.Add(participant);
            // Raise ParticipantAdded event (if using event sourcing)
            // RaiseEvent(new ParticipantAdded(participant.Id, Id));

            return participant;
        }

        public Result RemoveParticipant(Guid participantId)
        {
            var participant = _participants.FirstOrDefault(p => p.Id == participantId);
            if (participant == null)
            {
                return TripsErrors.ParticipantNotFound;
            }

            _participants.Remove(participant);
            // Raise ParticipantRemoved event (if using event sourcing)
            // RaiseEvent(new ParticipantRemoved(participantId));

            return Result.Success();
        }

        public Result UpdateParticipant(Guid participantId, string name, Email email)
        {
            var participant = _participants.FirstOrDefault(p => p.Id == participantId);
            if (participant == null)
            {
                return TripsErrors.ParticipantNotFound;
            }

            participant.Update(name, email);
            // Raise ParticipantUpdated event (if using event sourcing)
            // RaiseEvent(new ParticipantUpdated(participantId));

            return Result.Success();
        }

        public Result ChangeParticipantPermission(Guid participantId, PermissionLevel newPermissionLevel)
        {
            var participant = _participants.FirstOrDefault(p => p.Id == participantId);
            if (participant == null)
            {
                return TripsErrors.ParticipantNotFound;
            }
            participant.ChangePermissionLevel(newPermissionLevel);
            if (!_participants.Any(p => p.PermissionLevel == PermissionLevel.Organisator))
            {
                return TripsErrors.AtLeastOneOrganisatorRequired;
            }
            return Result.Success();
        }

        public Result<Transportation> AddTransportation(string name, string description, string identifier, Location departure,
            Location arrival, DateTime departureDate, DateTime arrivalDate, ExternalUrl routeLink,
            TransportationType type)
        {
            if (departureDate >= arrivalDate)
            {
                return TripsErrors.InvalidTransportationDates;
            }
            var transportation = Transportation.Create(Id, name, description, identifier, departure, arrival,
                departureDate, arrivalDate, routeLink, type);

            _transportations.Add(transportation);
            // Raise TransportationAdded event (if using event sourcing)
            // RaiseEvent(new TransportationAdded(transportation.Id, Id));

            return transportation;
        }
    }
}