using LIT.Travelnize.Domain.Base;
using LIT.Travelnize.Domain.Common;
using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.Domain.Trips.ValueObjects;

namespace LIT.Travelnize.DomainTests.Trips
{
    [TestClass]
    public class TripTests
    {
        private IUser _user = default!;
        private Trip _trip = default!;
        private PlanningSlot _tripSlot = default!;
        private Location _location = default!;
        private Email _email = default!;

        [TestInitialize]
        public void Initialize()
        {
            _user = new TestUser(Guid.NewGuid(), "TestUser", "Max", "Mustermann", "test@example.com");
            _tripSlot = PlanningSlot.Create(DateTime.Today, DateTime.Today.AddDays(14));
            _location = new Location(new Address("Musterstraße", "", "Musterstraße", "1", "12345", "Berlin", "Deutschland"), new Coordinates { Latitude = 52.52, Longitude = 13.405 });
            _email = new Email("test@example.com");
            _trip = Trip.Create(_user, "Reise", "Beschreibung");
        }

        [TestMethod]
        public void Create_ShouldInitializeTripAndAddUserAsParticipant()
        {
            // Arrange
            var user = new TestUser(Guid.NewGuid(), "User2", "Max", "Mustermann", "user2@example.com");

            // Act
            var trip = Trip.Create(user, "Urlaub", "Test");

            // Assert
            Assert.IsNotNull(trip);
            Assert.AreEqual("Urlaub", trip.Name);
            Assert.AreEqual("Test", trip.Description);
            Assert.AreEqual(user.Id, trip.UserId);
            Assert.AreEqual(1, trip.Participants.Count());
        }

        [TestMethod]
        public void Update_ShouldChangeTripProperties()
        {
            // Arrange

            // Act
            var result = _trip.Update("Neu", "NeuDesc");

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("Neu", _trip.Name);
            Assert.AreEqual("NeuDesc", _trip.Description);
        }

        [TestMethod]
        public void AddTravelSegment_ShouldAddSegment()
        {
            // Arrange

            // Act
            var result = _trip.AddTravelSegment("Segment", "Beschreibung", _tripSlot);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, _trip.TravelSegments.Count());
            Assert.AreEqual("Segment", _trip.TravelSegments.First().Name);
            Assert.AreEqual("Beschreibung", _trip.TravelSegments.First().Description);
            Assert.AreEqual(_tripSlot, _trip.TravelSegments.First().Slot);
        }

        [TestMethod]
        public void RemoveTravelSegment_ShouldRemoveSegment()
        {
            // Arrange
            var segment = _trip.AddTravelSegment("Segment", "", _tripSlot).Value!;

            // Act
            var result = _trip.RemoveTravelSegment(segment.Id);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(0, _trip.TravelSegments.Count());
        }

        [TestMethod]
        public void RemoveTravelSegment_ShouldReturnErrorIfNotFound()
        {
            // Arrange
            var unknownId = Guid.NewGuid();

            // Act
            var result = _trip.RemoveTravelSegment(unknownId);

            // Assert
            Assert.IsTrue(result.IsFailure);
        }

        [TestMethod]
        public void UpdateTravelSegment_ShouldUpdateSegment()
        {
            // Arrange
            var segment = _trip.AddTravelSegment("Segment", "", _tripSlot).Value!;
            var newSlot = PlanningSlot.Create(DateTime.Today.AddDays(2), DateTime.Today.AddDays(7));

            // Act
            var result = _trip.UpdateTravelSegment(segment.Id, "Neu", "Neu 2", newSlot); 

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(newSlot, segment.Slot);
            Assert.AreEqual("Neu", segment.Name);
            Assert.AreEqual("Neu 2", segment.Description);
        }

        [TestMethod]
        public void AddDestinationToTravelSegment_ShouldAddDestination()
        {
            // Arrange
            var segment = _trip.AddTravelSegment("Segment", "", _tripSlot).Value!;

            // Act
            var result = _trip.AddDestinationToTravelSegment(segment.Id, "Berlin", "Beschreibung", _location);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, segment.Destinations.Count());
        }

        [TestMethod]
        public void RemoveDestinationFromTravelSegment_ShouldRemoveDestination()
        {
            // Arrange
            var segment= _trip.AddTravelSegment("Segment", "", _tripSlot).Value!;
            var destination = _trip.AddDestinationToTravelSegment(segment.Id, "Berlin", "Beschreibung", _location).Value!;

            // Act
            var result = _trip.RemoveDestinationFromTravelSegment(segment.Id, destination.Id);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(0, segment.Destinations.Count());
        }

        [TestMethod]
        public void UpdateDestinationInTravelSegment_ShouldUpdateDestination()
        {
            // Arrange
            var segment = _trip.AddTravelSegment("Segment", "", _tripSlot).Value!;
            var destination = _trip.AddDestinationToTravelSegment(segment.Id, "Berlin", "Beschreibung", _location).Value!;

            // Act
            var result = _trip.UpdateDestinationInTravelSegment(segment.Id, destination.Id, "Paris", "Neu", _location, null, null);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("Paris", destination.Name);
            Assert.AreEqual("Neu", destination.Description);
        }

        [TestMethod]
        public void AddParticipant_AsUser_ShouldAddParticipant()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var result = _trip.AddParticipant(userId, "Max", _email);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(2, _trip.Participants.Count());
        }

        [TestMethod]
        public void AddParticipant_AsGuest_ShouldAddParticipant()
        {
            // Arrange

            // Act
            var result = _trip.AddParticipantAsGuest("Gast", _email);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(2, _trip.Participants.Count());
        }

        [TestMethod]
        public void RemoveParticipant_ShouldRemoveParticipant()
        {
            // Arrange
            var participant = _trip.AddParticipantAsGuest("Gast", _email).Value!;

            // Act
            var result = _trip.RemoveParticipant(participant.Id);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, _trip.Participants.Count());
        }

        [TestMethod]
        public void UpdateParticipant_ShouldUpdateParticipant()
        {
            // Arrange
            var participant = _trip.AddParticipantAsGuest("Gast", _email).Value!;

            // Act
            var result = _trip.UpdateParticipant(participant.Id, "Neuer Name", _email);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public void ChangeParticipantPermission_ShouldChangePermission()
        {
            // Arrange
            var participant = _trip.Participants.First();

            // Act
            var result = _trip.ChangeParticipantPermission(participant.Id, PermissionLevel.Organisator);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public void AddTransportation_ShouldAddTransportation()
        {
            // Arrange
            var departure = _location;
            var arrival = new Location(new Address("Hauptstraße", "", "Hauptstraße", "2", "20095", "Hamburg", "Deutschland"), new Coordinates{ Latitude = 53.55, Longitude = 10.0 });
            var departureDate = DateTime.Today;
            var arrivalDate = DateTime.Today.AddHours(2);
            var routeLink = ResourceReference.FromUrl("DB", "https://bahn.de");

            // Act
            var result = _trip.AddTransportation("ICE", "Schnellzug", "123", departure, arrival,
                departureDate, arrivalDate, routeLink, TransportationType.Train);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, _trip.Transportations.Count());
        }

        [TestMethod]
        public void AddTransportation_ShouldFailOnInvalidDates()
        {
            // Arrange
            var departure = _location;
            var arrival = new Location(new Address("Hauptstraße", "", "Hauptstraße", "2", "20095", "Hamburg", "Deutschland"), new Coordinates{ Latitude = 53.55, Longitude = 10.0 });
            var departureDate = DateTime.Today.AddHours(2);
            var arrivalDate = DateTime.Today;
            var routeLink = ResourceReference.FromUrl("DB", "https://bahn.de");

            // Act
            var result = _trip.AddTransportation("ICE", "Schnellzug", "123", departure, arrival,
                departureDate, arrivalDate, routeLink, TransportationType.Train);

            // Assert
            Assert.IsFalse(result.IsSuccess);
        }
    }

    public class TestUser(Guid id, string? userName, string firstName, string lastName, string? email) : IUser
    {
        public Guid Id { get; } = id;
        public string? UserName { get; } = userName;
        public string FirstName { get; } = firstName;
        public string LastName { get; } = lastName;
        public string? Email { get; } = email;
    }
}