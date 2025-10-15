using System.ComponentModel.DataAnnotations.Schema;

namespace LIT.Travelnize.Domain.Trips
{
    public class Trip : AggregateRoot
    {
        private readonly List<TravelSegment> _travelSegments = [];
        private readonly List<Participant> _participants = [];
        private readonly List<Transportation> _transportations = [];

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override Guid Id { get; init; }
        public Guid UserId { get; init; }

        public string Name { get; private set; } = default!;
        public string Description { get; private set; } = default!;
        public TripStatus Status => TripStatus.Create(Slot.Start, Slot.End);
        public PlanningSlot Slot { get; private set; } = PlanningSlot.Empty;

        public IEnumerable<TravelSegment> TravelSegments => _travelSegments.OrderBy(x => x.Slot);
        public IEnumerable<Participant> Participants => _participants.AsReadOnly();
        public IEnumerable<Transportation> Transportations => _transportations.AsReadOnly();

        public static Trip Create(IUser user, string name, string description)
        {
            var trip = new Trip()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Name = name,
                Description = description,
            };
            var participant = trip.AddParticipant(user.Id, user.UserName!, new Email(user.Email!)).Value!;
            trip.ChangeParticipantPermission(participant.Id, PermissionLevel.Organisator);

            return trip;
        }

        public Result Update(string name, string description)
        {
            Name = name;
            Description = description;
            return Result.Success();
        }

        public Result<TravelSegment> AddTravelSegment(string name, string description, PlanningSlot slot)
        {
            var newSegment = TravelSegment.Create(Id, name, description, slot);
            _travelSegments.Add(newSegment);
            RefreshSlot();
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
            RefreshSlot();

            return Result.Success();
        }

        public Result UpdateTravelSegment(Guid segmentId, string name, string description, PlanningSlot slot)
        {
            var segment = _travelSegments.FirstOrDefault(s => s.Id == segmentId);
            if (segment == null)
            {
                return TripErrors.TravelSegmentNotFound;
            }

            segment.Update(name, description, slot);
            RefreshSlot();

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

        public Result UpdateDestinationInTravelSegment(Guid segmentId, Guid destinationId, string name, string description,
            Location location, ResourceReference? image, ResourceReference? website)
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
            destination.Update(name, description, location, image, website);

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
            Location arrival, DateTime departureDate, DateTime arrivalDate, ResourceReference routeWebsite,
            TransportationType type)
        {
            if (departureDate >= arrivalDate)
            {
                return TripErrors.InvalidTransportationDates;
            }
            var transportation = Transportation.Create(Id, name, description, identifier, departure, arrival,
                departureDate, arrivalDate, routeWebsite, type);

            _transportations.Add(transportation);

            return transportation;
        }

        public Result UpdateTransportation(Guid transportationId, string name, string description, string identifier,
            Location departure, Location arrival, DateTime departureDate, DateTime arrivalDate,
            ResourceReference routeWebsite, TransportationType type, Guid[] passengerIds)
        {
            var transportation = _transportations.FirstOrDefault(t => t.Id == transportationId);
            if (transportation is null) return TripErrors.TransportationNotFound;

            var passengers = passengerIds.Select(id => _participants.FirstOrDefault(p => p.Id == id))
                                         .ToArray();
            if (passengers.Any(p => p is null))
            {
                return TripErrors.ParticipantNotFound;
            }

            return transportation.Update(name, description, identifier, departure, arrival,
                departureDate, arrivalDate, routeWebsite, type, passengers!);
        }

        public Result RemoveTransportation(Guid transportationId)
        {
            var transportation = _transportations.FirstOrDefault(t => t.Id == transportationId);
            if (transportation is null) return TripErrors.TransportationNotFound;

            _transportations.Remove(transportation);
            return Result.Success();
        }

        public Result<Accommodation> AddAccommodationToDestination(Guid destinationId, string name, AccommodationType accommodationType,
            Address address, PlanningSlot checkInOut)
        {
            var destination = _travelSegments.SelectMany(s => s.Destinations).FirstOrDefault(d => d.Id == destinationId);
            if (destination == null)
            {
                return TripErrors.DestinationNotFound;
            }

            var accommodation = Accommodation.Create(Id, destination.TravelSegmentId, destinationId, name, accommodationType,
                address, checkInOut);

            var result = destination.AddAccommodation(accommodation);

            return result.IsSuccess ? accommodation : result.Error;
        }

        public Result UpdateAccommodation(Guid destinationId, Guid accommodationId, string name,
            AccommodationType type, Address address, PlanningSlot checkInOut, BookingInfo bookingInfo)
        {
            var destination = _travelSegments
                .SelectMany(s => s.Destinations)
                .FirstOrDefault(d => d.Id == destinationId);
            if (destination is null) return TripErrors.DestinationNotFound;

            var accommodation = destination.Accommodations.FirstOrDefault(a => a.Id == accommodationId);
            if (accommodation is null) return TripErrors.AccommodationNotFound;

            var result = accommodation.Update(name, type, address, checkInOut);
            if (result.IsSuccess)
            {
                result = accommodation.UpdateBookingInfo(bookingInfo, checkInOut);
            }

            return result;
        }

        public Result RemoveAccommodation(Guid destinationId, Guid accommodationId)
        {
            var destination = TravelSegments.SelectMany(s => s.Destinations).FirstOrDefault(d => d.Id == destinationId);
            if (destination is null) return TripErrors.DestinationNotFound;

            var result = destination.RemoveAccommodation(accommodationId);
            return result;
        }

        public Result<Activity> AddActivity(Guid destinationId, string name, string description, Location location, DateTime? date, TimeSpan? duration)
        {
            var destination = TravelSegments.SelectMany(s => s.Destinations).FirstOrDefault(d => d.Id == destinationId);
            if (destination == null)
            {
                return TripErrors.DestinationNotFound;
            }
            var activity = Activity.Create(Id, destination.TravelSegmentId, destinationId, name, description, date, duration, location);
            var result = destination.AddActivity(activity);
            return result.IsSuccess ? activity : result.Error;
        }

        public Result UpdateActivity(Guid destinationId, Guid activityId, string name, string description, Location location, DateTime? date, TimeSpan? duration)
        {
            var destination = TravelSegments.SelectMany(s => s.Destinations).FirstOrDefault(d => d.Id == destinationId);
            if (destination == null)
            {
                return TripErrors.DestinationNotFound;
            }
            var activity = destination.Activities.FirstOrDefault(a => a.Id == activityId);
            if (activity == null)
            {
                return TripErrors.ActivityNotFound;
            }
            activity.Update(name, description, location, date, duration);
            return Result.Success();
        }

        public Result RemoveActivity(Guid destinationId, Guid activityId)
        {
            var destination = TravelSegments.SelectMany(s => s.Destinations).FirstOrDefault(d => d.Id == destinationId);
            if (destination is null) return TripErrors.DestinationNotFound;

            var result = destination.RemoveActivity(activityId);
            return result;
        }

        private void RefreshSlot()
        {
            Slot = PlanningSlot.Create(
                _travelSegments.Count == 0 ? null : _travelSegments.Min(s => s.Slot.Start),
                _travelSegments.Count == 0 ? null : _travelSegments.Max(s => s.Slot.End));
        }
    }
}