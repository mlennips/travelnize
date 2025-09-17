using LIT.Travelnize.Domain.Trips.ValueObjects;

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
        public PlanningSlot Slot { get; private set; } = default!;
        public TripStatus Status => TripStatus.Create(Slot.Start, Slot.End);

        public IEnumerable<TravelSegment> TravelSegments => _travelSegments.OrderBy(x => x.Slot);
        public IEnumerable<Participant> Participants => _participants.AsReadOnly();
        public IEnumerable<Transportation> Transportations => _transportations.AsReadOnly();

        public static Trip Create(IUser user, string name, string description, PlanningSlot slot)
        {
            var trip = new Trip()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Name = name,
                Description = description,
                Slot = slot
            };
            var participant = trip.AddParticipant(user.Id, user.UserName!, new Email(user.Email!)).Value!;
            trip.ChangeParticipantPermission(participant.Id, PermissionLevel.Organisator);

            return trip;
        }

        public Result Update(string name, string description, PlanningSlot slot)
        {
            Name = name;
            Description = description;
            Slot = slot;
            return Result.Success();
        }

        public Result<TravelSegment> AddTravelSegment(PlanningSlot slot, string description)
        {
            var newSegment = TravelSegment.Create(Id, description, slot);
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

        public Result UpdateTravelSegment(Guid segmentId, PlanningSlot slot, string description)
        {
            var segment = _travelSegments.FirstOrDefault(s => s.Id == segmentId);
            if (segment == null)
            {
                return TripErrors.TravelSegmentNotFound;
            }

            segment.Update(slot, description);

            return Result.Success();
        }

        public Result<Destination> AddDestinationToTravelSegment(Guid segmentId, string name, string description, Location location, PlanningSlot? slot = null)
        {
            var segment = _travelSegments.FirstOrDefault(s => s.Id == segmentId);
            if (segment == null)
            {
                return TripErrors.TravelSegmentNotFound;
            }

            slot ??= PlanningSlot.Create(segment.Slot.Start, segment.Slot.End);
            var destination = Destination.Create(Id, segmentId, name, description, slot, location);
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

        public Result<Participant> AddParticipantAsGuest(string name, Email email)
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
            if (participant.UserId == null)
            {
                return TripErrors.CannotChangePermissionOfGuestParticipant;
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

        public Result<Accommodation> AddAccommodationToDestination(Guid destinationId, string name, AccommodationType accommodationType,
            Address address, DateTime? checkIn, DateTime? checkOut)
        {
            var destination = _travelSegments.SelectMany(s => s.Destinations).FirstOrDefault(d => d.Id == destinationId);   
            if (destination == null)
            {
                return TripErrors.DestinationNotFound;
            }

            var accommodation = Accommodation.Create(Id, destinationId, name, accommodationType, 
                address, checkIn, checkOut);

            var result = destination.AddAccommodation(accommodation);

            return result.IsSuccess ? accommodation : result.Error;
        }
    }
}