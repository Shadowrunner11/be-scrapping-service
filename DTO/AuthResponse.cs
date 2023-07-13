namespace be_scrapping_service;

public class AuthResponse
{
  public string Username { get; set; } = null!;
  public string Email { get; set; } = null!;
  public string Token { get; set; } = null!;
  public string UserId { get; set; } = null!;
  public string FirstName { get; set; } = null!;
  public string LastName { get; set; } = null!;
  public int ExpiresIn {get; set;} = 60 * 24 * 7;
}
