using LIT.Travelnize.Domain.Trips.Commands;
using LIT.Travelnize.Domain.Trips.Queries;

namespace LIT.Travelnize.Store.Trips
{
    // ListTrips
    public record ListTripsAction(ListTripsQuery Query);
    public record ListTripsResultAction(ListTripsDto[] Trips);

    // GetTrip
    public record GetTripAction(GetTripQuery Query);
    public record GetTripResultAction(GetTripDto Trip);

    // CreateTrip
    public record CreateTripAction(CreateTripCommand Command);
    public record CreateTripResultAction(Guid TripId);

    // UpdateTrip
    public record UpdateTripAction(UpdateTripCommand Command);
    public record UpdateTripResultAction();

    // DeleteTrip
    public record DeleteTripAction(DeleteTripCommand Command);
    public record DeleteTripResultAction();

    // Fehlerbehandlung
    public record TripErrorAction(string ErrorMessage);
}