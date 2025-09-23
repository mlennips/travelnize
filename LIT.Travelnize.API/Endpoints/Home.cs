namespace LIT.Travelnize.API.Endpoints
{
    public static class Home
    {
        public static void MapHomeEndpoints(this IEndpointRouteBuilder routes)
        {
            RouteGroupBuilder api = routes.MapGroup("")
                .WithTags("");

            api.MapGet("welcome", () => "Welcome to the Travelnize API!")
                .WithName("GetWelcomeMessage")
                .Produces<string>(StatusCodes.Status200OK);
        }
    }
}
