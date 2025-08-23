using LIT.Travelnize.Domain.Base;
using LIT.Travelnize.Domain.Common;

namespace LIT.Travelnize.Domain.Trips
{
    public class Trip(Guid id, Guid userId, string name, string description, DateRange travelPeriod,
        List<TravelSegment> travelSegments, List<Participant> participants, List<Transportation> transportations) : AggregateRoot
    {
        public override Guid Id { get; } = id;
        public Guid UserId { get; private set; } = userId;

        public string Name { get; private set; } = name;
        public string Description { get; private set; } = description;
        public DateRange TravelPeriod { get; private set; } = travelPeriod;
        public IReadOnlyCollection<TravelSegment> TravelSegments => travelSegments.AsReadOnly();
        public IReadOnlyCollection<Participant> Participants => participants.AsReadOnly();
        public IReadOnlyCollection<Transportation> Transportations => transportations.AsReadOnly();

        public static Trip Create(Guid userId, string name, string description, DateTime startDate, DateTime endDate)
        {
            var dateRange = new DateRange(startDate, endDate);
            var trip = new Trip(Guid.NewGuid(), userId, name, description, dateRange, [], [], []);
            trip.AddParticipant(userId, "", null, PermissionLevel.Admin);
            return trip;
        }

        public Result Update(DateTime startDate, DateTime endDate)
        {
            TravelPeriod = new DateRange(startDate, endDate);
            return Result.Success();
        }


        public Result<Guid> AddTravelSegment(DateTime startDate, DateTime endDate, string description)
        {
            var newSegment = TravelSegment.Create(Id, description, new DateRange(startDate, endDate));
            travelSegments.Add(newSegment);
            return newSegment.Id;
        }

        public Result RemoveTravelSegment(Guid segmentId)
        {
            var segment = TravelSegments.FirstOrDefault(s => s.Id == segmentId);
            if (segment == null)
            {
                return TripsErrors.TravelSegmentNotFound;
            }

            travelSegments.Remove(segment);
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


        public Result<Guid> AddDestinationToTravelSegment(Guid segmentId, string name, string description, Location location)
        {
            var segment = TravelSegments.FirstOrDefault(s => s.Id == segmentId);
            if (segment == null)
            {
                return TripsErrors.TravelSegmentNotFound;
            }

            var destination = Destination.Create(Id, segmentId, name, description, segment.DateRange, location);
            segment.AddDestination(destination);
            return destination.Id;
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


        public Result<Guid> AddParticipant(Guid userId, string name, Email? email, PermissionLevel permissionLevel)
        {
            var participant = Participant.Create(Id, userId, name, email, permissionLevel);
            participants.Add(participant);
            return participant.Id;
        }

        public Result<Guid> AddParticipant(string name, Email email, PermissionLevel permissionLevel)
        {
            var participant = Participant.Create(Id, name, email, permissionLevel);
            participants.Add(participant);
            return participant.Id;
        }

        public Result RemoveParticipant(Guid participantId)
        {
            var participant = Participants.FirstOrDefault(p => p.Id == participantId);
            if (participant == null)
            {
                return TripsErrors.ParticipantNotFound;
            }

            participants.Remove(participant);
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


        public Result<Guid> AddTransportation(string name, string description, string identifier, Location departure,
            Location arrival, DateTime departureDate, DateTime arrivalDate, ExternalUrl routeLink,
            TransportationType type, List<Participant>? passengers = null)
        {
            if (departureDate >= arrivalDate)
            {
                return TripsErrors.InvalidTransportationDates;
            }
            var transportation = Transportation.Create(Id, name, description, identifier, departure, arrival,
                departureDate, arrivalDate, routeLink, type, passengers);

            transportations.Add(transportation);
            return transportation.Id;
        }
    }
}