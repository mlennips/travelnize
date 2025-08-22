using LIT.Travelnize.Domain.Base;
using LIT.Travelnize.Domain.Common;

namespace LIT.Travelnize.Domain.Trips
{
    public class Trip(Guid id, Guid userId, DateRange dateRange, string itinerary) : AggregateRoot
    {
        public override Guid Id { get; } = id;
        public Guid UserId { get; private set; } = userId;
        public DateRange DateRange { get; private set; } = dateRange;
        public string Itinerary { get; private set; } = itinerary;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public List<TripSegment> TripSegments { get; private set; } = [];
        public List<Participant> Participants { get; private set; } = [];

        public static Trip Create(Guid userId, DateTime startDate, DateTime endDate, string itinerary)
        {
            var dateRange = new DateRange(startDate, endDate);
            return new Trip(Guid.NewGuid(), userId, dateRange, itinerary);
        }

        public Result Update(DateTime startDate, DateTime endDate, string itinerary)
        {
            DateRange = new DateRange(startDate, endDate);
            Itinerary = itinerary;
            return Result.Success();
        }

        public Result<Guid> AddTripSegment(DateTime startDate, DateTime endDate, string description)
        {
            var newSegment = TripSegment.Create(Id, new DateRange(startDate, endDate), description);
            TripSegments.Add(newSegment);
            return newSegment.Id;
        }

        public Result RemoveTripSegment(Guid segmentId)
        {
            var segment = TripSegments.FirstOrDefault(s => s.Id == segmentId);
            if (segment == null)
            {
                return TripsErrors.TripSegmentNotFound;
            }

            TripSegments.Remove(segment);
            return Result.Success();
        }

        public Result UpdateTripSegment(Guid segmentId, DateTime startDate, DateTime endDate, string description)
        {
            var segment = TripSegments.FirstOrDefault(s => s.Id == segmentId);
            if (segment == null)
            {
                return TripsErrors.TripSegmentNotFound;
            }

            segment.Update(startDate, endDate, description);
            return Result.Success();
        }

        public Result<Guid> AddParticipant(Guid userId, string name, Email email, PermissionLevel permissionLevel)
        {
            var participant = Participant.Create(userId, name, email, permissionLevel);
            Participants.Add(participant);
            return participant.Id;
        }

        public Result RemoveParticipant(Guid participantId)
        {
            var participant = Participants.FirstOrDefault(p => p.Id == participantId);
            if (participant == null)
            {
                return TripsErrors.ParticipantNotFound;
            }

            Participants.Remove(participant);
            return Result.Success();
        }

        public Result UpdateParticipant(Guid participantId, string name, Email email, PermissionLevel permissionLevel)
        {
            var participant = Participants.FirstOrDefault(p => p.Id == participantId);
            if (participant == null)
            {
                return TripsErrors.ParticipantNotFound;
            }

            participant.Update(name, email, permissionLevel);
            return Result.Success();
        }
    }
}