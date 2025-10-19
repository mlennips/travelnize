

using LIT.Travelnize.Domain.Trips.ValueObjects;
using MudBlazor;

namespace LIT.Travelnize.Helpers
{
    public class IconHelper
    {
        public static string TripFilled => MudBlazor.Icons.Material.Filled.TravelExplore;
        public static string TripOutlined => MudBlazor.Icons.Material.Outlined.TravelExplore;
        public static string TripRounded => MudBlazor.Icons.Material.Rounded.TravelExplore;
        public static string TripSharp => MudBlazor.Icons.Material.Sharp.TravelExplore;
        public static string TripTwoTone => MudBlazor.Icons.Material.TwoTone.TravelExplore;

        public static string TravelSegmentFilled => MudBlazor.Icons.Material.Filled.Route;
        public static string TravelSegmentOutlined => MudBlazor.Icons.Material.Outlined.Route;
        public static string TravelSegmentRounded => MudBlazor.Icons.Material.Rounded.Route;
        public static string TravelSegmentSharp => MudBlazor.Icons.Material.Sharp.Route;
        public static string TravelSegmentTwoTone => MudBlazor.Icons.Material.TwoTone.Route;

        public static string DestinationFilled => MudBlazor.Icons.Material.Filled.LocationOn;
        public static string DestinationOutlined => MudBlazor.Icons.Material.Outlined.LocationOn;
        public static string DestinationRounded => MudBlazor.Icons.Material.Rounded.LocationOn;
        public static string DestinationSharp => MudBlazor.Icons.Material.Sharp.LocationOn;
        public static string DestinationTwoTone => MudBlazor.Icons.Material.TwoTone.LocationOn;

        public static string AccommodationFilled => MudBlazor.Icons.Material.Filled.Hotel;
        public static string AccommodationOutlined => MudBlazor.Icons.Material.Outlined.Hotel;
        public static string AccommodationRounded => MudBlazor.Icons.Material.Rounded.Hotel;
        public static string AccommodationSharp => MudBlazor.Icons.Material.Sharp.Hotel;
        public static string AccommodationTwoTone => MudBlazor.Icons.Material.TwoTone.Hotel;

        public static string ActivityFilled => MudBlazor.Icons.Material.Filled.Attractions;
        public static string ActivityOutlined => MudBlazor.Icons.Material.Outlined.Attractions;            
        public static string ActivityRounded => MudBlazor.Icons.Material.Rounded.Attractions;
        public static string ActivitySharp => MudBlazor.Icons.Material.Sharp.Attractions;
        public static string ActivityTwoTone => MudBlazor.Icons.Material.TwoTone.Attractions;

        public static string TransportationFilled => MudBlazor.Icons.Material.Filled.DirectionsTransit;
        public static string TransportationOutlined => MudBlazor.Icons.Material.Outlined.DirectionsTransit;
        public static string TransportationRounded => MudBlazor.Icons.Material.Rounded.DirectionsTransit;
        public static string TransportationSharp => MudBlazor.Icons.Material.Sharp.DirectionsTransit;
        public static string TransportationTwoTone => MudBlazor.Icons.Material.TwoTone.DirectionsTransit;

        public static string GetIconForTransportationType(TransportationType type)
        {
            var typeName = type.Value;
            return typeName switch
            {
                "Flight" => Icons.Material.Filled.Flight,
                "Train" => Icons.Material.Filled.Train,
                "Bus" => Icons.Material.Filled.DirectionsBus,
                "Car" => Icons.Material.Filled.DirectionsCar,
                "Boat" => Icons.Material.Filled.DirectionsBoat,
                "Bicycle" => Icons.Material.Filled.DirectionsBike,
                "Walk" => Icons.Material.Filled.DirectionsWalk,
                _ => Icons.Material.Filled.DirectionsTransit
            };
        }
    }
}
