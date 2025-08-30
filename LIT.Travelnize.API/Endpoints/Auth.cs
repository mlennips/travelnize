using LIT.Travelnize.Shared.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LIT.Travelnize.API.Endpoints
{
    public static class Auth
    {
        public static void RegisterAuthEndpoints(this IEndpointRouteBuilder routes)
        {
            RouteGroupBuilder api = routes.MapGroup("/auth")
                .WithTags("Auth");

            api.MapPost("/register", async (UserManager<IdentityUser> userManager, RegisterCommand model) =>
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return Results.Ok();
                }
                return Results.BadRequest(result.Errors);
            });

            api.MapPost("/login", async (UserManager<IdentityUser> userManager, IConfiguration configuration, LoginCommand model) =>
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
                {
                    var token = GenerateJwtToken(user, configuration);
                    var userId = Guid.Parse(user.Id);
                    return Results.Ok(new LoginDto(token, userId, user.UserName ?? "?"));
                }
                return Results.Unauthorized();
            });

            api.MapPost("/logout", () =>
            {
                // For JWT, logout is typically handled on the client side by deleting the token.
                return Results.Ok();
            }).RequireAuthorization();

            api.MapPost("/forgot-password", () =>
            {
                // Implement forgot password logic, e.g., send reset email.
                return Results.Ok();
            });
        }

        private static string GenerateJwtToken(IdentityUser user, IConfiguration configuration)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.Now.AddDays(1);

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: expiry,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
