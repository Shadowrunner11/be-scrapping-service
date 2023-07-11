using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace be_scrapping_service.Service
{
  public class TokenService
  {
    private const int ExpirationMinutes = 60 * 24 * 7;
    public string CreateToken(IdentityUser user)
    {
      var token = CreateJwtToken(
          CreateClaims(user),
          CreateSigningCredentials(),
          DateTime.UtcNow.AddMinutes(ExpirationMinutes)
      );

      return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private JwtSecurityToken CreateJwtToken(
      List<Claim> claims,
      SigningCredentials credentials,
      DateTime expiration
    ) =>
      new(
        "be_scrapping_service",
        "be_scrapping_service",
        claims,
        expires: expiration,
        signingCredentials: credentials
      );

    private List<Claim> CreateClaims(IdentityUser user)
    {
    
      ArgumentNullException.ThrowIfNull(user.UserName);

      ArgumentNullException.ThrowIfNull(user.Email);

      var claims = new List<Claim>
        {
          new Claim(ClaimTypes.NameIdentifier, user.Id),
          new Claim(JwtRegisteredClaimNames.Sub, user.Id),
          new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
          new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)),
          new Claim(ClaimTypes.Name, user.UserName),
          new Claim(ClaimTypes.Email, user.Email)
        };
      return claims; 
    }

    private SigningCredentials CreateSigningCredentials() => new SigningCredentials(
      new SymmetricSecurityKey(
          Encoding.UTF8.GetBytes("a5e5a0e4bc817ef2774d841c0d300847b3212d96a49a3fe849ecf65d51dadb11a5e5a0e4bc817ef2774d841c0d300847b3212d96a49a3fe849ecf65d51dadb11a5e5a0e4bc817ef2774d841c0d300847b3212d96a49a3fe849ecf65d51dadb11")
      ),
      SecurityAlgorithms.HmacSha256
    );
  }
}
