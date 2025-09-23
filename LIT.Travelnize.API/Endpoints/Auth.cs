using LIT.Travelnize.Domain.Auth;
using LIT.Travelnize.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LIT.Travelnize.API.Endpoints
{
    public static class Auth
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder routes)
        {
            RouteGroupBuilder api = routes.MapGroup("/auth")
                .WithTags("Auth");

            api.MapPost("/register", async (UserManager<User> userManager, RegisterCommand model) =>
            {
                var user = new User { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName };
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return Results.Ok();
                }
                return Results.BadRequest(result.Errors);
            });

            api.MapPost("/login", async (UserManager<User> userManager, IConfiguration configuration, LoginCommand model) =>
            {
                var user = await userManager.FindByEmailAsync(model.EmailOrUserName);
                user ??= await userManager.FindByNameAsync(model.EmailOrUserName);
                if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
                {
                    var token = GenerateJwtToken(user, configuration);
                    return Results.Ok(new LoginDto(token, user.Id, user.UserName!, user.FirstName, user.LastName));
                }
                return Results.Unauthorized();
            });

            api.MapPost("/refresh", async (UserManager<User> userManager, IConfiguration configuration, RefreshCommand model) =>
            {
                var user = await userManager.FindByIdAsync(model.UserId.ToString());
                if (user != null && IsTokenValid(model.Token, configuration))
                {
                    var token = GenerateJwtToken(user, configuration);
                    return Results.Ok(new LoginDto(token, user.Id, user.UserName!, user.FirstName, user.LastName));
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

        private static string GenerateJwtToken(IdentityUser<Guid> user, IConfiguration configuration)
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

            return new JwtSecurityTokenHandler() .WriteToken(token);
        }

        private static bool IsTokenValid(string token, IConfiguration configuration)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return validatedToken.ValidTo > DateTime.UtcNow;
            }
            catch
            {
                return false;
            }
        }
    }
}
