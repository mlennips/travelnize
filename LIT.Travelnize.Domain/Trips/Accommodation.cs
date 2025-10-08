using System.ComponentModel.DataAnnotations.Schema;

namespace LIT.Travelnize.Domain.Trips
{
    public class Accommodation : IAuditableEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; init; }
        public Guid TripId { get; init; }
        public Guid TravelSegmentId { get; init; }
        public Guid DestinationId { get; init; }
        public Guid AggregateId => TripId;

        public string Name { get; private set; } = default!;
        public AccommodationType Type { get; private set; } = default!;
        public Address Address { get; private set; } = default!;
        public PlanningSlot CheckInOut { get; private set; } = default!;
        public BookingInfo BookingInfo { get; private set; } = default!;

        internal static Accommodation Create(Guid tripId, Guid travelSegmentId, Guid destinationId, string name, AccommodationType type, 
            Address address, PlanningSlot checkInOut)
        {
            return new Accommodation()
            {
                Id = Guid.NewGuid(),
                TripId = tripId,
                TravelSegmentId = travelSegmentId,
                DestinationId = destinationId,
                Name = name,
                Type = type,
                Address = address,
                CheckInOut = checkInOut,
                BookingInfo = BookingInfo.Empty
            };
        }

        internal Result Update(string name, AccommodationType type, Address address, PlanningSlot checkInOut)
        {
            Name = name;
            Type = type;
            Address = address;
            CheckInOut = checkInOut;

            return Result.Success();
        }

        internal Result SetBookingInfo(BookingInfo bookingInfo)
        {
            BookingInfo = bookingInfo;
            return Result.Success();
        }

        internal Result UpdateBookingInfo(BookingInfo bookingInfo, PlanningSlot checkInOut)
        {
            if (bookingInfo.BookingDate != null && !checkInOut.HasStartAndEnd)
            {
                return Result.Failure(TripErrors.CheckInOutDatesRequiredWhenBooked);
            }

            BookingInfo = bookingInfo;
            CheckInOut = checkInOut;

            return Result.Success();
        }

        internal Result RemoveBookingInfo()
        {
            BookingInfo = default!;

            return Result.Success();
        }
    }
}
