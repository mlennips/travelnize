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
        public IReadOnlyCollection<TravelSegment> TravelSegments { get => _travelSegments.AsReadOnly(); init => _travelSegments = value.ToList(); }
        public IReadOnlyCollection<Participant> Participants { get => _participants.AsReadOnly(); init => _participants = value.ToList(); }
        public IReadOnlyCollection<Transportation> Transportations { get => _transportations.AsReadOnly(); init => _transportations = value.ToList(); }

        public static Trip Create(Guid userId, string name, string description, DateRange travelPeriod, string userName, Email userEmail)
        {
            var trip = new Trip()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = name,
                Description = description,
                TravelPeriod = travelPeriod,
                TravelSegments = [],
                Participants = [],
                Transportations = []
            };
            trip.AddParticipant(userId, userName, userEmail);

            return trip;
        }

        public Result Update(DateTime startDate, DateTime endDate)
        {
            TravelPeriod = new DateRange(startDate, endDate);
            return Result.Success();
        }


        public Result<TravelSegment> AddTravelSegment(DateTime startDate, DateTime endDate, string description)
        {
            var newSegment = TravelSegment.Create(Id, description, new DateRange(startDate, endDate));
            _travelSegments.Add(newSegment);
            return newSegment;
        }

        public Result RemoveTravelSegment(Guid segmentId)
        {
            var segment = TravelSegments.FirstOrDefault(s => s.Id == segmentId);
            if (segment == null)
            {
                return TripsErrors.TravelSegmentNotFound;
            }

            _travelSegments.Remove(segment);
            return Result.Success();
        }

        public Result UpdateTravelSegment(Guid segmentId, DateTime startDate, DateTime endDate, string description)
        {
            var segment = TravelSegments.FirstOrDefault(s => s.Id == segmentId);
            if (segment == null)
            {
                return TripsErrors.TravelSegmentNotFound;
            }

            segment.Update(startDate, endDate, description);
            return Result.Success();
        }


        public Result<Destination> AddDestinationToTravelSegment(Guid segmentId, string name, string description, Location location)
        {
            var segment = TravelSegments.FirstOrDefault(s => s.Id == segmentId);
            if (segment == null)
            {
                return TripsErrors.TravelSegmentNotFound;
            }

            var destination = Destination.Create(Id, segmentId, name, description, segment.DateRange, location);
            segment.AddDestination(destination);
            return destination;
        }

        public Result RemoveDestinationFromTravelSegment(Guid segmentId, Guid destinationId)
        {
            var segment = TravelSegments.FirstOrDefault(s => s.Id == segmentId);
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
            return Result.Success();
        }

        public Result UpdateDestinationInTravelSegment(Guid segmentId, Guid destinationId, string name, string description, Location location)
        {
            var segment = TravelSegments.FirstOrDefault(s => s.Id == segmentId);
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
            var participant = Participants.FirstOrDefault(p => p.Id == participantId);
            if (participant == null)
            {
                return TripsErrors.ParticipantNotFound;
            }

            _participants.Remove(participant);
            return Result.Success();
        }

        public Result UpdateParticipant(Guid participantId, string name, Email email)
        {
            var participant = Participants.FirstOrDefault(p => p.Id == participantId);
            if (participant == null)
            {
                return TripsErrors.ParticipantNotFound;
            }

            participant.Update(name, email);
            return Result.Success();
        }

        public Result ChangeParticipantPermission(Guid participantId, PermissionLevel newPermissionLevel)
        {
            var participant = Participants.FirstOrDefault(p => p.Id == participantId);
            if (participant == null)
            {
                return TripsErrors.ParticipantNotFound;
            }
            participant.ChangePermissionLevel(newPermissionLevel);
            if (!Participants.Any(p => p.PermissionLevel == PermissionLevel.Admin))
            {
                return TripsErrors.AtLeastOneAdminRequired;
            }
            return Result.Success();
        }


        public Result<Transportation> AddTransportation(string name, string description, string identifier, Location departure,
            Location arrival, DateTime departureDate, DateTime arrivalDate, ExternalUrl routeLink,
            TransportationType type, List<Participant>? passengers = null)
        {
            if (departureDate >= arrivalDate)
            {
                return TripsErrors.InvalidTransportationDates;
            }
            var transportation = Transportation.Create(Id, name, description, identifier, departure, arrival,
                departureDate, arrivalDate, routeLink, type, passengers);

            _transportations.Add(transportation);
            return transportation;
        }
    }
}