using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
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
                .WithTags(["Auth"]);

            api.MapPost("/register", async (UserManager<IdentityUser> userManager, RegisterModel model) =>
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return Results.Ok();
                }
                return Results.BadRequest(result.Errors);
            });

            api.MapPost("/login", async (UserManager<IdentityUser> userManager, IConfiguration configuration, LoginModel model) =>
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
                {
                    var token = GenerateJwtToken(user, configuration);
                    return Results.Ok(new { token });
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

        public record LoginModel
        {
            public LoginModel(string email, string password)
            {
                Email = email;
                Password = password;
            }

            [Required(ErrorMessage = "Email ist erforderlich.")]
            [EmailAddress(ErrorMessage = "Ungültige E-Mail-Adresse.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Passwort ist erforderlich.")]
            public string Password { get; set; }
        }

        public record RegisterModel
        {
            public RegisterModel(string email, string password, string confirmPassword)
            {
                Email = email;
                Password = password;
                ConfirmPassword = confirmPassword;
            }

            [Required(ErrorMessage = "Email ist erforderlich.")]
            [EmailAddress(ErrorMessage = "Ungültige E-Mail-Adresse.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Passwort ist erforderlich.")]
            [StringLength(100, ErrorMessage = "Das Passwort muss mindestens {2} und maximal {1} Zeichen lang sein.", MinimumLength = 8)]
            public string Password { get; set; }

            [Compare("Password", ErrorMessage = "Das Passwort und die Bestätigung stimmen nicht überein.")]
            public string ConfirmPassword { get; set; }
        }
    }
}
