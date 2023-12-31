using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using be_scrapping_service.Context;
using be_scrapping_service.Service;
using be_scrapping_service.Entity;
namespace be_scrapping_service.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
  private readonly UserManager<User> _userManager;
  private readonly ScrapingContext _context;
  private readonly TokenService _tokenService;

  public AuthController(UserManager<User> userManager, ScrapingContext context, TokenService tokenService)
  {
    _userManager = userManager;
    _context = context;
    _tokenService = tokenService;
  }

  [HttpPost]
  [Route("register")]
  public async Task<IActionResult> Register(RegistrationRequest request)
  {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    var result = await _userManager.CreateAsync(
      new User
      {
        UserName = request.Username,
        Email = request.Email,
        FirstName = request.FirstName,
        LastName = request.LastName
      },
      request.Password
    );

    request.Password = "";

    if (result.Succeeded)
      return CreatedAtAction(nameof(Register), new
      {
        email = request.Email
      }, request);


    foreach (var error in result.Errors)
      ModelState.AddModelError(error.Code, error.Description);
  
    return BadRequest(ModelState);
  }

  [HttpPost]
  [Route("login")]
  public async Task<ActionResult<AuthResponse>> Authenticate([FromBody] AuthRequest request)
  {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    var managedUser = await _userManager.FindByEmailAsync(request.Email);

    if (managedUser == null)
      return BadRequest("Bad credentials");


    var isPasswordValid = await _userManager.CheckPasswordAsync(managedUser, request.Password);

    if (!isPasswordValid)
      return BadRequest("Bad credentials");

    var userInDb = _context.Users.FirstOrDefault(u => u.Email == request.Email);
    if (userInDb is null)
      return Unauthorized();

    var accessToken = _tokenService.CreateToken(userInDb);
    await _context.SaveChangesAsync();

    // TODO: is not resilent

    ArgumentNullException.ThrowIfNull(userInDb.UserName);
    ArgumentNullException.ThrowIfNull(userInDb.Email);

    return Ok(new AuthResponse
    {
      Username = userInDb.UserName,
      Email = userInDb.Email,
      Token = accessToken,
    });
  }
}
