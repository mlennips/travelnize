namespace LIT.Travelnize.API.Endpoints
{
    public static class Users
    {
        public static void RegisterUsersEndpoints(this IEndpointRouteBuilder routes)
        {
            RouteGroupBuilder api = routes.MapGroup("/users")
                .WithTags(["Users"])
                .RequireAuthorization();
        }
    }
}
