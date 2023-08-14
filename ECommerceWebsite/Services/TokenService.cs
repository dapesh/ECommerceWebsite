using ECommerceWebsite.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ECommerceWebsite.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _Key;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TokenService(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }
        public string CreateToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim("mobilephone", user.PhoneNumber),
                new Claim(ClaimTypes.NameIdentifier, user.Username)
            };
            var creds = new SigningCredentials(_Key, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            string encodedToken = tokenHandler.WriteToken(token);
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddHours(1), // Set the cookie expiration time
                HttpOnly = true, // The cookie can only be accessed by the server
                SameSite = SameSiteMode.Strict, // Configure same-site policy
                Secure = true // Only send the cookie over HTTPS
            };
            return encodedToken.ToString();
        }

        public string GetMobilePhoneFromToken()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            string storedToken = _httpContextAccessor.HttpContext.Session.GetString("EncodedToken");


            var authorizationHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();

            if (authorizationHeader != null && authorizationHeader.StartsWith("Bearer "))
            {
                var token = authorizationHeader.Substring("Bearer ".Length).Trim();
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _Key,
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

                try
                {
                    SecurityToken validatedToken;
                    var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

                    var mobilePhoneClaim = claimsPrincipal.FindFirst("mobilephone");
                    if (mobilePhoneClaim != null)
                    {
                        return mobilePhoneClaim.Value;
                    }
                }
                catch (Exception)
                {
                    // Token validation failed
                    // Handle the exception according to your requirements
                }
            }

            return null;
        }
    }
}
