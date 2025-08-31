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
            trip.AddParticipant(user.Id, user.UserName!, new Email(user.Email!));

            return trip;
        }

        public Result Update(string name, string description, DateRange travelPeriod)
        {
            Name = name;
            Description = description;
            TravelPeriod = travelPeriod;
            return Result.Success();
        }

        public Result<TravelSegment> AddTravelSegment(DateRange dateRange, string description)
        {
            var newSegment = TravelSegment.Create(Id, description, dateRange);
            _travelSegments.Add(newSegment);

            return newSegment;
        }

        public Result RemoveTravelSegment(Guid segmentId)
        {
            var segment = _travelSegments.FirstOrDefault(s => s.Id == segmentId);
            if (segment == null)
            {
                return TripErrors.TravelSegmentNotFound;
            }

            _travelSegments.Remove(segment);

            return Result.Success();
        }

        public Result UpdateTravelSegment(Guid segmentId, DateRange dateRange, string description)
        {
            var segment = _travelSegments.FirstOrDefault(s => s.Id == segmentId);
            if (segment == null)
            {
                return TripErrors.TravelSegmentNotFound;
            }

            segment.Update(dateRange, description);

            return Result.Success();
        }

        public Result<Destination> AddDestinationToTravelSegment(Guid segmentId, string name, string description, Location location)
        {
            var segment = _travelSegments.FirstOrDefault(s => s.Id == segmentId);
            if (segment == null)
            {
                return TripErrors.TravelSegmentNotFound;
            }

            var destination = Destination.Create(Id, segmentId, name, description, segment.DateRange, location);
            var result = segment.AddDestination(destination);

            return result.IsSuccess ? destination : result.Error;
        }

        public Result RemoveDestinationFromTravelSegment(Guid segmentId, Guid destinationId)
        {
            var segment = _travelSegments.FirstOrDefault(s => s.Id == segmentId);
            if (segment == null)
            {
                return TripErrors.TravelSegmentNotFound;
            }
            var destination = segment.Destinations.FirstOrDefault(d => d.Id == destinationId);
            if (destination == null)
            {
                return TripErrors.DestinationNotFound;
            }
            segment.RemoveDestination(destinationId);

            return Result.Success();
        }

        public Result UpdateDestinationInTravelSegment(Guid segmentId, Guid destinationId, string name, string description, Location location)
        {
            var segment = _travelSegments.FirstOrDefault(s => s.Id == segmentId);
            if (segment == null)
            {
                return TripErrors.TravelSegmentNotFound;
            }
            var destination = segment.Destinations.FirstOrDefault(d => d.Id == destinationId);
            if (destination == null)
            {
                return TripErrors.DestinationNotFound;
            }
            destination.Update(name, description, location);

            return Result.Success();
        }

        public Result<Participant> AddParticipant(Guid userId, string name, Email email)
        {
            var participant = Participant.CreateAsUser(Id, userId, name, email);
            _participants.Add(participant);

            return participant;
        }

        public Result<Participant> AddParticipant(string name, Email email)
        {
            var participant = Participant.CreateAsGuest(Id, name, email);
            _participants.Add(participant);

            return participant;
        }

        public Result RemoveParticipant(Guid participantId)
        {
            var participant = _participants.FirstOrDefault(p => p.Id == participantId);
            if (participant == null)
            {
                return TripErrors.ParticipantNotFound;
            }

            _participants.Remove(participant);

            return Result.Success();
        }

        public Result UpdateParticipant(Guid participantId, string name, Email email)
        {
            var participant = _participants.FirstOrDefault(p => p.Id == participantId);
            if (participant == null)
            {
                return TripErrors.ParticipantNotFound;
            }

            participant.Update(name, email);

            return Result.Success();
        }

        public Result ChangeParticipantPermission(Guid participantId, PermissionLevel newPermissionLevel)
        {
            var participant = _participants.FirstOrDefault(p => p.Id == participantId);
            if (participant == null)
            {
                return TripErrors.ParticipantNotFound;
            }
            participant.ChangePermissionLevel(newPermissionLevel);
            if (!_participants.Any(p => p.PermissionLevel == PermissionLevel.Organisator))
            {
                return TripErrors.AtLeastOneOrganisatorRequired;
            }
            return Result.Success();
        }

        public Result<Transportation> AddTransportation(string name, string description, string identifier, Location departure,
            Location arrival, DateTime departureDate, DateTime arrivalDate, ExternalUrl routeLink,
            TransportationType type)
        {
            if (departureDate >= arrivalDate)
            {
                return TripErrors.InvalidTransportationDates;
            }
            var transportation = Transportation.Create(Id, name, description, identifier, departure, arrival,
                departureDate, arrivalDate, routeLink, type);

            _transportations.Add(transportation);

            return transportation;
        }
    }
}